using General;
using UnityEngine;

namespace VineGeneration.Util
{
    [DefaultExecutionOrder(-10)]
    public class GlobalVineSettings : MonoBehaviour
    {
        public static GlobalVineSettings current;

        public float maximumPaintDistance = 100;
        public float breadCrumbRadius = 0.45f;
        public float breadCrumbAliveTime = 3;
        public LayerMask environmentMask;
        public LayerMask corruptionMask;
        public LayerMask generalMask;
        private GameObject climbPointParent;
        private GameObject branchParent;
        private GameObject leaveParent;

        private void Awake()
        {
            current = this;
        }

        public GameObject GetBranchParent()
        {
            if (!branchParent)
            {
                branchParent = new GameObject(nameof(branchParent));
            }

            return branchParent;
        }
        
        public GameObject GetClimbPointParent()
        {
            if (!climbPointParent)
            {
                climbPointParent = new GameObject(nameof(climbPointParent));
            }

            return climbPointParent;
        }
        
        public GameObject GetLeaveParent()
        {
            if (!leaveParent)
            {
                leaveParent = new GameObject(nameof(leaveParent));
            }

            return leaveParent;
        }
    }
}