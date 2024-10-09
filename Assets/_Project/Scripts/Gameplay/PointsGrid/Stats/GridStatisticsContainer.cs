using UnityEngine;

namespace Audio.Gameplay.PointsGrid
{
    [CreateAssetMenu]
    public class GridStatisticsContainer : ScriptableObject
    {
        public GridStatisticsUnit OverallCount = new();
        public GridStatisticsUnit PerGridCount = new();
        public GridStatisticsUnit PerWaveCount = new();
        [Space]
        public GridStatisticsUnit OverallActivationsCount = new();
        public GridStatisticsUnit PerGridActivationsCount = new();
        public GridStatisticsUnit PerWaveActivationsCount = new();
    }
}