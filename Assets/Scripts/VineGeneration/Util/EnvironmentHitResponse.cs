using UnityEngine;

namespace VineGeneration.Util
{
    public struct EnvironmentHitResponse
    {
        public Vector3 newPoint;
        public Vector3 newNormal;
        public Vector3 newDirection;
        public bool hit;
    }
}