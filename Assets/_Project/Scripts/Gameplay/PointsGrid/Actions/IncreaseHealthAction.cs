using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Localization;
using Zenject;

namespace Audio.Gameplay.PointsGrid
{
    [System.Serializable]
    public class IncreaseHealthAction : IGridPointAction
    {
        [SerializeField] private LocalizedString _description;
        [SerializeField] private float _increaseConstantHealthPercent = 0.01f;
        [SerializeField] private float _increaseMomentHealthPercent = 0.1f;

        public ReactiveLocalizedString GetDescription() => new(_description);

        public void ApplyAction(
            SpawnModifiers momentModifiers,
            SpawnModifiers constantModifiers,
            WarriorsCollection warriors)
        {
            momentModifiers.HealthMultiplier += _increaseMomentHealthPercent;
            constantModifiers.HealthMultiplier += _increaseConstantHealthPercent;
        }
    }
}