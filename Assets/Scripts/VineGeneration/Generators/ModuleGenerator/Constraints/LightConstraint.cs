using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.Constraints
{
    [CreateAssetMenu(menuName = "VineGeneration/Constraints/Light Constraint")]
    public class LightConstraint : BaseGrowConstraint
    {
        public override bool Execute(ref BranchQueryInfo info)
        {
            var ivyParams = info.ivyParams;
            var point = info.point;
            return LightIntensityHelper.IsInLight(ivyParams, point);
        }
    }
}