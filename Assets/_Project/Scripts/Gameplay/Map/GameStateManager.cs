using Gameplay.Map.Bosses;
using Gameplay.Map.Spawn;
using UniRx;
using UnityEngine;
using Zenject;

namespace Gameplay.Map
{
    public class GameStateManager : MonoBehaviour
    {
        public enum EState
        {
            Play,
            Win,
            Lose
        }

        public IReadOnlyReactiveProperty<EState> State => _state;

        [Inject] private WarriorsCollection _warriors;
        [Inject] private CastlesCollection _castles;
        [Inject] private BossReference _boss;

        private ReactiveProperty<EState> _state = new(EState.Play);

        private void FixedUpdate()
        {
            if (_state.Value != EState.Play)
                return;
            if (!_castles.HasAllyCastle)
                _state.Value = EState.Lose;
            else if (_boss.Boss.IsActivated && _warriors.Allies.Count == 0)
                _state.Value = EState.Lose;
            else if (!_boss.BossIsAlive)
                _state.Value = EState.Win;
        }
    }
}