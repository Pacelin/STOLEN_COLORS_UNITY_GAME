namespace Gameplay.Map.Allies
{
    [System.Serializable]
    public class SpawnModifiers
    {
        public float AttackSpeedMultiplier;
        public float MagesAttackRange;
        public float DamageMultiplier;
        public float HealthMultiplier;
        public float WalkSpeed;

        public int MagesCount;
        public int SoldiersCount;
        public int TanksCount;
    }
}