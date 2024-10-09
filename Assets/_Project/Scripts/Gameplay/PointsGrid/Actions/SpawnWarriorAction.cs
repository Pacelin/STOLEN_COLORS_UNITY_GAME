using Gameplay.Map;
using Gameplay.Map.Allies;
using Gameplay.Map.Spawn;
using Runtime.Utils;
using UnityEngine;
using UnityEngine.Localization;

namespace Audio.Gameplay.PointsGrid
{
    [System.Serializable]
    public class SpawnWarriorAction : IGridPointAction
    {
        public EWarriorClass Class => _class;
        [SerializeField] private LocalizedString _description;
        [SerializeField] private EWarriorClass _class;
        public ReactiveLocalizedString GetDescription() => new (_description);
        
        public void ApplyAction(AlliesSpawner.SpawnCount count,
            SpawnModifiers constantModifiers, WarriorsCollection warriors)
        {
            if (_class == EWarriorClass.Mage)
                count.MagesCount++;
            else if (_class == EWarriorClass.Soldier)
                count.WarriorsCount++;
            else if (_class == EWarriorClass.Tank)
                count.TanksCount++;
        }
    }
}