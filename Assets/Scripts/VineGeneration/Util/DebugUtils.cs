using UnityEngine;

namespace VineGeneration.Util
{
    public static class DebugUtils
    {
        public static void DrawDebugLines(Vector3 startPos, float distance, Vector3[] directions, Color mainColor,
            Color secondaryColor,
            float duration)
        {
            Debug.DrawLine(startPos, startPos + directions[0] * distance, mainColor, duration);
            for (int i = 1; i < directions.Length; i++)
            {
                Debug.DrawLine(startPos, startPos + directions[i] * distance, secondaryColor, duration);
            }
        }
    }
}