using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;
namespace VineGeneration.Generators.ModuleGenerator.Actions.PostNodeSpawnActions
{
    [CreateAssetMenu(menuName = "VineGeneration/Actions/Add Subbranch")]
    public class AddSubbranchAtDistanceAction : BaseGrowAction
    {
        [SerializeField] private float minBranchLength = 0.3f;
        [SerializeField] private float maxBranchLength = 0.8f;
        
        public override bool Execute(ref BranchQueryInfo info)
        {
            var br = info.branch;
            var length = container.Rng.Range(minBranchLength, maxBranchLength);
            if (br.totalLength - br.lastBranchedOffLength >= length)
            {
                var newBr = GrowUtil.CreateChildBranch(container.Rng, br, br.LastNode);
                br.lastBranchedOffLength = br.totalLength;
                container.AddBranch(GeneratorType.Sub, newBr);
                return true;
            }

            return false;
        }
    }
}