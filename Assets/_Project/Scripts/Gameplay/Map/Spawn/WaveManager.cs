using UniRx;

namespace Gameplay.Map.Spawn
{
    public class WaveManager
    {
        public IReadOnlyReactiveProperty<bool> WaveIsInProgress => 
            _waveIsInProgress.Select(b => _bossFight || b).ToReactiveProperty();

        private readonly WarriorsSpawner _warriors;
        private readonly CastlesCollection _castles;
        private readonly ReactiveProperty<bool> _waveIsInProgress;
        private bool _bossFight;

        public WaveManager(WarriorsSpawner warriors, CastlesCollection castles)
        {
            _warriors = warriors;
            _castles = castles;
            _waveIsInProgress = new(false);
            _bossFight = false;
        }

        public void SetBossFight()
        {
            _bossFight = true;
        }
        
        public void StartWave()
        {
            _castles.ReleaseCastles();
            _warriors.SpawnEnemiesWave();
            _waveIsInProgress.Value = true;
        }

        public void CaptureCastle(Castle castle, Warrior warrior)
        {
            castle.SetOwner(warrior.Side);
            _castles.SetCapturingCastle(castle);
            _castles.SnapCastles();
            _warriors.StopWave();
            _waveIsInProgress.Value = false;
        }
    }
}