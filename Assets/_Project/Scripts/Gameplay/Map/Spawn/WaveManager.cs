using System;
using UniRx;

namespace Gameplay.Map.Spawn
{
    public class WaveManager
    {
        public IReadOnlyReactiveProperty<bool> ShowWavePanels =>
            _waveIsInProgress.Merge(_hideWavePanels)
                .Select(_ => !_waveIsInProgress.Value && !_hideWavePanels.Value)
                .DistinctUntilChanged()
                .ToReactiveProperty();
        public IReadOnlyReactiveProperty<bool> WaveIsInProgress => _waveIsInProgress;

        private readonly WarriorsSpawner _warriorsSpawner;
        private readonly WarriorsCollection _warriors;
        private readonly CastlesCollection _castles;
        private readonly ReactiveProperty<bool> _waveIsInProgress;
        private readonly ReactiveProperty<bool> _hideWavePanels;

        public WaveManager(WarriorsSpawner warriorsSpawner, CastlesCollection castles, WarriorsCollection warriors)
        {
            _warriorsSpawner = warriorsSpawner;
            _warriors = warriors;
            _castles = castles;
            _waveIsInProgress = new(false);
            _hideWavePanels = new ReactiveProperty<bool>(false);
        }

        public void SetHidePanels(bool hide) => _hideWavePanels.Value = hide;
        
        public void PrepareEnemiesWave() => _warriorsSpawner.PrepareEnemiesWave();
        
        public void StartWave()
        {
            _warriorsSpawner.SpawnEnemiesWave();
            _castles.StartWaveCastles();
            _waveIsInProgress.Value = true;
        }

        public void CaptureCastle(Castle castle, Warrior warrior)
        {
            castle.SetOwner(warrior.Side);
            _castles.SnapCastles();
            _warriorsSpawner.StopWave();

            if (warrior.Side == EBattleSide.Ally)
                foreach (var ally in _warriors.Allies)
                    castle.AddUnit(ally);
            else
                foreach (var enemy in _warriors.Enemies)
                    castle.AddUnit(enemy);
            
            _waveIsInProgress.Value = false;
        }
    }
}