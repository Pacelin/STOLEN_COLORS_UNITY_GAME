using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Localization;

namespace Audio.Gameplay.PointsGrid
{
    [System.Serializable]
    public class IncreaseAttackSpeedAction : IGridPointAction
    {
        [SerializeField] private LocalizedString _description;
        [SerializeField] private float _increaseConstantAttackSpeedPercent = 0.01f;
        
        public ReactiveLocalizedString GetDescription() => new (_description);
        
        public void ApplyAction(
            AlliesSpawner.SpawnCount momentModifiers,
            SpawnModifiers constantModifiers,
            WarriorsCollection warriors)
        {
            constantModifiers.AttackSpeedMultiplier += _increaseConstantAttackSpeedPercent;
        }
    }
}