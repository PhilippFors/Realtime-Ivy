using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.GrowHeuristics
{
	[CreateAssetMenu(menuName = "VineGeneration/Grow Heuristics/Environment Heuristic")]
	public class EnvironmentHeuristic : BaseGrowHeuristic
	{
		[SerializeField] private bool buildMesh = true;
		[SerializeField] private bool vertLimit = true;

		public override bool Execute(ref BranchQueryInfo info)
		{
			if (EnvironmentHelper.EnvironmentQuery(info, true, vertLimit, buildMesh, out var response)) {
				info.direction = response.newDirection;
				info.normal = response.newNormal;
				info.point = response.newPoint;
				return true;
			}

			return false;
		}
	}
}
