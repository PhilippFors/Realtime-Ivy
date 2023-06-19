using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;
namespace VineGeneration.Generators.ModuleGenerator.Actions.PostNodeSpawnActions
{
    [CreateAssetMenu(menuName = "VineGeneration/Actions/Try Add Subbranch")]
    public class TryAddSubbranchAction : BaseGrowAction
    {
        public override bool Execute(ref BranchQueryInfo info)
        {
            var ivyParams = info.ivyParams;
            var branch = info.branch;
            
            if (!branch.hasChanged || container.Rng.Value < (1 - ivyParams.branchProbability) /
                (branch.branchDepth == 0 ? 1 : branch.branchDepth) ||
                branch.branchDepth >= ivyParams.maxSubBranchDepth)
            {
                return false;
            }
            
            var newBr = GrowUtil.CreateChildBranch(container.Rng, branch, branch.LastNode);
            branch.lastBranchedOffLength = branch.totalLength;
            container.AddBranch(GeneratorType.Sub, newBr);
            return true;
        }
    }
}