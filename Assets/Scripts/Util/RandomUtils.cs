using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Util
{
    public static class RandomUtils
    {
        public static double GetPseudoDoubleWithinRange(double lowerBound, double upperBound)
        {
            var random = new Random();
            var rDouble = random.NextDouble();
            var rRangeDouble = rDouble * (upperBound - lowerBound) + lowerBound;
            return rRangeDouble;
        }

        public static List<Vector3> GetNRandomPointsWithinCircle(int n, bool exact)
        {
            List<Vector3> returnList = new List<Vector3>();
        
            for (int i = 0; i < n; i++)
            {
                double x = GetPseudoDoubleWithinRange(-1, 1);
                double y = GetPseudoDoubleWithinRange(-1, 1);
                if (x * x + y * y < 1)
                    returnList.Add(new Vector3((float) x, 0f, (float) y));
                else
                {
                    if (exact) i--;
                }
            }

            return returnList;
        }
    }
}