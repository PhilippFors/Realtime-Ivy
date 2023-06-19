using UnityEngine;
using VineGeneration.Core;

namespace VineGeneration.Generators.ModuleGenerator.Constraints
{
    [CreateAssetMenu(menuName = "VineGeneration/Constraints/Grow length Constraint")]
    public class GrowLengthConstraint : BaseGrowConstraint
    {
        public override bool Execute(ref BranchQueryInfo info)
        {
            if (info.branch.totalLength >= info.ivyParams.maxBranchLength /
                (info.branch.branchDepth == 0 ? 1 : info.branch.branchDepth))
            {
                return false;
            }

            return true;
        }
    }
}