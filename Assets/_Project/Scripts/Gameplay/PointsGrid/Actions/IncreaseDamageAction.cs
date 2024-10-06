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
        [SerializeField] private float _increaseMomentDamagePercent = 0.1f;
        
        public ReactiveLocalizedString GetDescription() => new (_description);
        
        public void ApplyAction(
            SpawnModifiers momentModifiers,
            SpawnModifiers constantModifiers,
            WarriorsCollection warriors)
        {
            momentModifiers.DamageMultiplier += _increaseMomentDamagePercent;
            constantModifiers.DamageMultiplier += _increaseConstantDamagePercent;
        }
    }
}