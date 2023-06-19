using UnityEngine;

namespace VineGeneration.Parameters
{
    [CreateAssetMenu(menuName = "VineGeneration/Parameters/Mesh Parameters")]
    public class MeshParameters : ScriptableObject
    {
        public Material branchMaterial;
        public Material leafMaterial;
        
        public Material sharedBranchMaterial;
        public Material sharedLeafMaterial;
        
        public Material sharedClimbNodeMaterial;
    }
}