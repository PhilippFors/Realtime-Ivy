using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.Constraints
{
    [CreateAssetMenu(menuName = "VineGeneration/Constraints/Corruption Constraint")]
    public class CorruptionConstraint : BaseGrowConstraint
    {
        public override bool Execute(ref BranchQueryInfo info)
        {
            var point = info.point;
            return !CorruptionHelper.RaycastCorruption(point, info.direction, info.stepLength, info.ivyParams);
        }
    }
}