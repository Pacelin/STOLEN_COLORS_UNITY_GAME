namespace Audio.Gameplay.PointsGrid
{
    [System.Serializable]
    public class GridStatisticsUnit
    {
        public float TanksSpawn = 0;
        public float WarriorsSpawn = 0;
        public float MagesSpawn = 0;
        
        public float Heal = 0;
        public float Health = 0;
        public float AttackSpeed = 0;
        public float MoveSpeed = 0;
        public float AttackDamage = 0;
        public float Range = 0;
        
        public float EmptyPoints = 0;
        public float Locks = 0;

        public float Iterations;
        
        public void Add(GridStatisticsUnit unit)
        {
            TanksSpawn += unit.TanksSpawn;
            WarriorsSpawn += unit.WarriorsSpawn;
            MagesSpawn += unit.MagesSpawn;
            Heal += unit.Heal;
            Health += unit.Health;
            AttackSpeed += unit.AttackSpeed;
            MoveSpeed += unit.MoveSpeed;
            AttackDamage += unit.AttackDamage;
            Range += unit.Range;
            EmptyPoints += unit.EmptyPoints;
            Locks += unit.Locks;
            Iterations++;
        }

        public void Average(GridStatisticsUnit unit)
        {
            if (Iterations == 0)
            {
                Add(unit);
                return;
            }

            TanksSpawn = (TanksSpawn * Iterations + unit.TanksSpawn) / (Iterations + 1);
            WarriorsSpawn = (WarriorsSpawn * Iterations + unit.WarriorsSpawn) / (Iterations + 1);
            MagesSpawn = (MagesSpawn * Iterations + unit.MagesSpawn) / (Iterations + 1);
            Heal = (Heal * Iterations + unit.Heal) / (Iterations + 1);
            Health = (Health * Iterations + unit.Health) / (Iterations + 1);
            AttackSpeed = (AttackSpeed * Iterations + unit.AttackSpeed) / (Iterations + 1);
            MoveSpeed = (MoveSpeed * Iterations + unit.MoveSpeed) / (Iterations + 1);
            AttackDamage = (AttackDamage * Iterations + unit.AttackDamage) / (Iterations + 1);
            Range = (Range * Iterations + unit.Range) / (Iterations + 1);
            EmptyPoints = (EmptyPoints * Iterations + unit.EmptyPoints) / (Iterations + 1);
            Locks = (Locks * Iterations + unit.Locks) / (Iterations + 1);
            Iterations++;
        }
    }
}