using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace Gameplay.Map.Spawn
{
    [CreateAssetMenu]
    public class WarriorsConfig : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<EWarriorClass, Warrior> _allies;
        [SerializeField] private SerializedDictionary<EWarriorClass, Warrior> _enemies;

        public Warrior GetAlly(EWarriorClass @class) => _allies[@class];
        public Warrior GetEnemy(EWarriorClass @class) => _enemies[@class];
    }
}