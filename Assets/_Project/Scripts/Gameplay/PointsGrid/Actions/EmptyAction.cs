using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Localization;

namespace Audio.Gameplay.PointsGrid
{
    [System.Serializable]
    public class EmptyAction : IGridPointAction
    {
        [SerializeField] private LocalizedString _description;
        
        public ReactiveLocalizedString GetDescription() => new (_description);
        
        public void ApplyAction(AlliesSpawner.SpawnCount count,
            SpawnModifiers constantModifiers,
            WarriorsCollection warriors)
        {
            
        }
    }
}