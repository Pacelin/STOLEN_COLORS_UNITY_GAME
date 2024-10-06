namespace Gameplay.Map.Spawn
{
    public class WaveManager
    {
        private readonly WarriorsSpawner _warriors;
        private readonly CastlesCollection _castles;
        
        public WaveManager(WarriorsSpawner warriors, CastlesCollection castles)
        {
            _warriors = warriors;
            _castles = castles;
        }

        public void StartWave()
        {
            _castles.ReleaseCastles();
            _warriors.SpawnEnemiesWave();
        }

        public void CaptureCastle(Castle castle, Warrior warrior)
        {
            castle.SetOwner(warrior.Side);
            _castles.SnapCastles();
            _warriors.StopWave();
        }
    }
}