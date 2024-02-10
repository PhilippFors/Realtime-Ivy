using UnityEngine;

namespace IvyGenerator
{
    public class IvyBranch : ScriptableObject
    {
        public BaseIvyBranchCondition[] conditions;
        public BaseIvyBehaviour[] behaviours;
    }
}