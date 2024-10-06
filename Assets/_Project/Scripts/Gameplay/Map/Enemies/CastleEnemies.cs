using UnityEngine;

namespace Gameplay.Map.Enemies
{
    [CreateAssetMenu]
    public class CastleEnemies : ScriptableObject
    {
        public WarriorsWave[] FirstWaveVariations => _firstWaveVariations;
        public WarriorsWave[] ReinforcementWaves => _reinforcementWaves;
        public float WaveCooldown => _waveCooldown;
        
        [SerializeField] private WarriorsWave[] _firstWaveVariations;
        [SerializeField] private WarriorsWave[] _reinforcementWaves;
        [SerializeField] private float _waveCooldown;
    }
}