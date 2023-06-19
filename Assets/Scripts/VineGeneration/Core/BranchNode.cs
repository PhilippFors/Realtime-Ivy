using UnityEngine;
using VineGeneration.Util;

namespace VineGeneration.Core
{
    public class BranchNode
    {
        public int index;
        public Vector3 position;
        public Vector3 surfaceNormal;
        public Vector3 growDirection;
        public Transform climbNode;

        public void Destroy()
        {
            if (climbNode)
            {
                GrowUtil.climbNodes.Remove(climbNode.transform);
                GameObject.Destroy(climbNode.gameObject);
            }
        }
    }
}