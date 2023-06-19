using UnityEngine;
using UnityEngine.Serialization;

namespace VineGeneration.Parameters
{
    [CreateAssetMenu(menuName = "VineGeneration/Parameters/Grow Parameters")]
    public class GrowParameters : ScriptableObject
    {
        public float stepSize = 0.15f;
        public float maxBranchLength = 4f;
        public int maxSubBranchDepth = 2;
        public float minDistanceToSurface = 0.01f;
        public float maxDistanceToSurface = 0.04f;
		public float branchProbability = 0.2f;
        public float directionFrequency = 1f;
        public float dirRandomWeight = 0.5f;
        public float directionAmplitude = 25f;
        public float grabProbabilityOnFall = 1f;

        [Header("Lights")] public bool lightSensitive = true;
        [Tooltip("The higher, the harder it is to grow in low light conditions")]
        public float minLightIntensity = 0.8f;

        [FormerlySerializedAs("branchMat")] [Header("Mesh Generation")]
        // public bool halfGeometry;
        public MeshParameters meshParameters;
        public float branchThickness = 0.05f;

        [Header("Climbing")] public bool climbable = true;
        public GameObject climbNodePrefab;
        [Tooltip("How many climb points can be within the specified radius")]
        public int maxClimbNodesInRadius = 0;
         public float climbNodeProbability = 0.5f;
        public float minClimbPointDistance = 0.2f;
        public float maxClimbPointDistance = 0.5f;

        [Header("Corruption behaviour")] public bool hatesCorruption = true;
    }
}