using General.LSystem;
using UnityEngine;
using VineGeneration.Util;
namespace VineGeneration.Generators.LSystemGenerator.Rules
{
	[CreateAssetMenu(fileName = "AddBranchRule", menuName = "LSystem/Rules/AddBranchRule")]
	public class AddBranchRule : BaseRule
	{
		public override bool ApplyRule(LSystemData data, LSystemInterpreter interpreter, LSystemGenerator generator)
		{
			if(data.branch.branchDepth > generator.ivyParams.maxSubBranchDepth)
			{
				return true;
			}
			
			var newBr = GrowUtil.CreateChildBranch(generator.Container.Rng, data.branch, data.branch.LastNode);
			generator.AddBranch(newBr);
			return true;
		}
	}
}
