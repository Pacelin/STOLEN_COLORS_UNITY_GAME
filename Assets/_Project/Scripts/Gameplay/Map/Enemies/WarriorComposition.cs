using Gameplay.Map.Allies;

namespace Gameplay.Map.Enemies
{
    [System.Serializable]
    public struct WarriorComposition
    {
        public EWarriorClass Class;
        public SpawnModifiers Modifiers;
        public int Count;
    }
}