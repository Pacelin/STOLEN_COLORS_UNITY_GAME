using Gameplay.Map.Bosses;
using Gameplay.Map.Spawn;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class WarriorWalkToCastleState : WarriorState
    {
        [Inject] private CastlesCollection _castles;
        [Inject] private WaveManager _waveManager;
        [Inject] private WarriorsCollection _warriors;
        [Inject] private BossReference _boss;

        private CompositeDisposable _disposables;
        private const float SNAP_CASTLE_DISTANCE = 1.5f;
        private const float FIND_ENEMY_DISTANCE = 5;
        
        public override void Enter()
        {
            _disposables = new();
            var attackCastle = _castles.GetCastle(1 - _warrior.Side);
            if (!attackCastle)
            {
                if (_warrior.Side == EBattleSide.Ally && _boss.BossIsAlive)
                    _warrior.SetAttack(_boss.Boss);
                else
                    _warrior.SetWinner();
                return;
            }
            
            var destination = attackCastle.GetWarriorDestination(_warrior);

            _warrior.Agent.stoppingDistance = 0;
            _warrior.Agent.SetDestination(destination);
            Observable.EveryFixedUpdate()
                .First(_ => Vector3.Distance(destination, _warrior.Position) <= SNAP_CASTLE_DISTANCE &&
                            !_warriors.HasEnemies)
                .Subscribe(_ =>
                {
                    _waveManager.CaptureCastle(attackCastle, _warrior);
                    attackCastle.AddUnit(_warrior);
                }).AddTo(_disposables);
            Observable.EveryFixedUpdate()
                .Subscribe(_ =>
                {
                    var warrior = _warriors.GetNearestEnemyFor(_warrior, out var distance);
                    if (warrior != null && distance <= FIND_ENEMY_DISTANCE)
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