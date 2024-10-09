using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio.Gameplay.PointsGrid
{
    public static class GridPointsUtility
    {
        public static GridPoint GetRandomWeight(this GridPoint[] allPoints,
            Dictionary<GridPoint, float> additionalWeights,
            bool ignoreBadPoints)
        {
            float GetAdditional(GridPoint p)
            {
                if (additionalWeights.ContainsKey(p))
                    return additionalWeights[p];
                return 0;
            }

            void AddAdditional(GridPoint p)
            {
                if (additionalWeights.ContainsKey(p))
                    additionalWeights[p] += p.AdditionalWeight;
                else
                    additionalWeights[p] = p.AdditionalWeight;
            }

            GridPoint[] points;
            if (ignoreBadPoints)
                points = allPoints
                    .Where(p => p.Model.CanConnectThrough && !p.IsEmptyPoint)
                    .ToArray();
            else
                points = allPoints;
            
            var weightSum = points.Sum(p => p.Weight + GetAdditional(p));
            var r = Random.Range(0, weightSum);
            var accumulate = 0f;
            GridPoint result = null;
            
            foreach (var point in points)
            {
                if (result == null)
                {
                    accumulate += point.Weight + GetAdditional(point);
                    if (accumulate >= r)
                        result = point;
                }
                AddAdditional(point);
            }

            additionalWeights[result] = 0;
            return result;
        }
        
        public static void AddActivations(this GridPoint[] connections)
        {
            if (connections[0].ActivationsCount <= 0)
                connections[0].AddActivation();
            if (connections[^1].ActivationsCount <= 0)
                connections[^1].AddActivation();
            for (int i = 1; i < connections.Length - 1; i++)
                connections[i].AddActivation();
        }
        
        public static bool CanConnect(this GridPoint[] connections)
        {
            if (connections[^1].ActivationsCount > 0)
                return false;
            foreach (var connection in connections)
                if (!connection.Model.CanConnectThrough)
                    return false;
            return true;
        }

        public static GridPoint[] GetConnectionsWith(this GridPoint first, GridPoint last)
        {
            var result = new List<GridPoint>();
            result.Add(first);
            
            var offset = last.Coordinates - first.Coordinates;
            int nod;
            var x = Mathf.Abs(offset.x);
            var y = Mathf.Abs(offset.y);
            if ((x == 0 || y == 0) ||
                (x != y && x % 4 == 0) ||
                (y % 4 == 0 && x % 4 != 0))
                nod = NOD(offset.x / 2, offset.y / 2);
            else
                nod = NOD(offset.x, offset.y);
            
            if (nod > 1)
            {
                var microOffset = offset / nod;
                var innerPointCoord = first.Coordinates + microOffset;
                while (innerPointCoord != last.Coordinates)
                {
                    var point = first.Layout.Get(innerPointCoord.x, innerPointCoord.y);
                    result.Add(point);
                    innerPointCoord += microOffset;
                }
            }
            result.Add(last);
            return result.ToArray();
        }

        private static int NOD(int a, int b)
        {
            a = Mathf.Abs(a);
            b = Mathf.Abs(b);
            if (b > a)
                (b, a) = (a, b);
            for (int i = a; i > 0; i--)
                if (a % i == 0)
                    if (b % i == 0)
                        return i;
            return 0;
        }
    }
}