using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Localization;

namespace Audio.Gameplay.PointsGrid
{
    [System.Serializable]
    public class IncreaseDamageAction : IGridPointAction
    {
        [SerializeField] private LocalizedString _description;
        [SerializeField] private float _increaseConstantDamagePercent = 0.01f;
        
        public ReactiveLocalizedString GetDescription() => new (_description);
        
        public void ApplyAction(
            AlliesSpawner.SpawnCount momentModifiers,
            SpawnModifiers constantModifiers,
            WarriorsCollection warriors)
        {
            constantModifiers.DamageMultiplier += _increaseConstantDamagePercent;
        }
    }
}