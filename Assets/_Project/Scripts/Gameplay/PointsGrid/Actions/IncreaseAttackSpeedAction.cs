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
        [SerializeField] private float _increaseMomentAttackSpeedPercent = 0.1f;
        
        public ReactiveLocalizedString GetDescription() => new (_description);
        
        public void ApplyAction(
            SpawnModifiers momentModifiers,
            SpawnModifiers constantModifiers,
            WarriorsCollection warriors)
        {
            momentModifiers.AttackSpeedMultiplier += _increaseMomentAttackSpeedPercent;
            constantModifiers.AttackSpeedMultiplier += _increaseConstantAttackSpeedPercent;
        }
    }
}