using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Localization;

namespace Audio.Gameplay.PointsGrid
{
    [System.Serializable]
    public class HealAction : IGridPointAction
    {
        [SerializeField] private LocalizedString _description;
        [SerializeField] private float _healingAmount = 5f;
        
        public ReactiveLocalizedString GetDescription() => new (_description);

        public void ApplyAction(
            AlliesSpawner.SpawnCount momentModifiers,
            SpawnModifiers constantModifiers,
            WarriorsCollection warriors)
        {
            foreach (var ally in warriors.Allies)
                ally.Model.Heal(_healingAmount);
        }
    }
}