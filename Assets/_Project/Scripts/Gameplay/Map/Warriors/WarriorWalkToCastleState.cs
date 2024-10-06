using Gameplay.Map.Spawn;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class WarriorWalkToCastleState : WarriorState
    {
        [Inject] private CastlesCollection _castles;
        [Inject] private WarriorsCollection _warriors;
        
        private CompositeDisposable _disposables;
        private const float SNAP_CASTLE_DISTANCE = 3;
        private const float FIND_ENEMY_DISTANCE = 7;
        
        public override void Enter()
        {
            _disposables = new();
            var attackCastle = _castles.GetCastle(1 - _warrior.Side);
            var destination = attackCastle.GetWarriorDestination(_warrior);

            _warrior.Agent.stoppingDistance = 0;
            _warrior.Agent.SetDestination(destination);
            Observable.EveryFixedUpdate()
                .First(_ => Vector3.Distance(destination, _warrior.Position) <= SNAP_CASTLE_DISTANCE)
                .Subscribe(_ =>
                {
                    attackCastle.SnapUnits();
                    attackCastle.SetOwner(_warrior.Side);
                    attackCastle.AddUnit(_warrior);
                }).AddTo(_disposables);
            Observable.EveryFixedUpdate()
                .Subscribe(_ =>
                {
                    var warrior = _warriors.GetNearestEnemyFor(_warrior, out var distance);
                    if (distance <= FIND_ENEMY_DISTANCE)
                        _warrior.SetAttack(warrior);
                }).AddTo(_disposables);
        }

        public override void Exit()
        {
            if (_warrior.Agent.isOnNavMesh && _warrior.Agent.isActiveAndEnabled)
                _warrior.Agent.ResetPath();
            _disposables.Dispose();
        }
    }
}