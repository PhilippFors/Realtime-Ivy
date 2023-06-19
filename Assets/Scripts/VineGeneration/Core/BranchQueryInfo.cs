using UnityEngine;
using VineGeneration.Parameters;

namespace VineGeneration.Core
{
    public struct BranchQueryInfo
    {
        public Branch branch;
        public Vector3 point;
        public Vector3 direction;
        public Vector3 normal;
        public float stepLength;
        public GrowParameters ivyParams;
    }
}