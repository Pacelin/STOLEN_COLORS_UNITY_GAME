using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Localization;

namespace Audio.Gameplay.PointsGrid
{
    [System.Serializable]
    public class IncreaseHealthAction : IGridPointAction
    {
        [SerializeField] private LocalizedString _description;
        [SerializeField] private float _increaseConstantHealthPercent = 0.01f;

        public ReactiveLocalizedString GetDescription() => new(_description);
       
        public void ApplyAction(
            AlliesSpawner.SpawnCount count,
            SpawnModifiers constantModifiers,
            WarriorsCollection warriors)
        {
            constantModifiers.HealthMultiplier += _increaseConstantHealthPercent;
        }
    }
}