using UnityEngine;

namespace IvyGenerator
{
    public abstract class BaseIvyBranchCondition : ScriptableObject
    {
        public abstract bool IsValid();
    }
}