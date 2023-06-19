using System.Collections.Generic;
using UnityEngine;

namespace VineGeneration.Util
{
    /// <summary>
    /// Using whack math to generate directions in a spherical shape.
    /// </summary>
    public static class RayDirectionGenerator
    {
        private static Dictionary<int, Vector3[]> directionCache = new();

        public static Vector3[] GetSphereDirections(int amount = 25)
        {
            if (directionCache.ContainsKey(amount))
            {
                return directionCache[amount];
            }

            var dirs = new Vector3[amount];

            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
            float angleIncrement = Mathf.PI * 2 * goldenRatio;

            for (int i = 0; i < amount; i++)
            {
                float t = (float) i / amount;
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = angleIncrement * i;

                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);
                dirs[i] = new Vector3(x, y, z);
            }

            directionCache.Add(amount, dirs);
            return dirs;
        }

        public static int GetSphereDirectionsNonAlloc(ref Vector3[] dirs, int amount = 25)
        {
            int a = 0;
            if (directionCache.ContainsKey(amount))
            {
                var cache = directionCache[amount];
                for (int i = 0; i < dirs.Length || i < cache.Length; i++)
                {
                    a++;
                    dirs[i] = cache[i];
                }

                return a;
            }

            float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
            float angleIncrement = Mathf.PI * 2 * goldenRatio;

            for (int i = 0; i < amount || i < dirs.Length; i++)
            {
                a++;
                float t = (float) i / amount;
                float inclination = Mathf.Acos(1 - 2 * t);
                float azimuth = angleIncrement * i;

                float x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
                float y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
                float z = Mathf.Cos(inclination);
                dirs[i] = new Vector3(x, y, z);
            }

            directionCache.Add(amount, dirs);
            return a;
        }

        public static Vector3[] GetForwardDirections(Vector3 mainDirection, Vector3 normal, int amount, float maxAngle)
        {
            var turnAngle = maxAngle / amount;
            Vector3[] directions = new Vector3[amount];
            directions[0] = mainDirection;
            int j = 1;
            for (int i = 1; i < amount; i++)
            {
                var angle = turnAngle * i;
                if (i % 2 == 0)
                {
                    angle *= -1;
                }

                var newDirection = Quaternion.AngleAxis(angle, normal) * mainDirection;
                if (Vector3.Angle(newDirection, mainDirection) < 5)
                {
                    continue;
                }

                directions[j] = newDirection.normalized;
                j++;
            }

            return directions;
        }

        public static void GetForwardDirectionsNonAlloc(ref Vector3[] directions, Vector3 mainDirection, Vector3 normal, float maxAngle)
        {
            var turnAngle = maxAngle / directions.Length;
            directions[0] = mainDirection;
            int j = 1;
            for (int i = 1; i < directions.Length; i++)
            {
                var angle = turnAngle * i;
                if (i % 2 == 0)
                {
                    angle *= -1;
                }

                var newDirection = Quaternion.AngleAxis(angle, normal) * mainDirection;

                directions[j] = newDirection.normalized;
                j++;
            }
        }
    }
}