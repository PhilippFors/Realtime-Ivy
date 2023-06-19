using General.LSystem;
using ScriptableObjectPools;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.LSystemGenerator.Rules
{
	[CreateAssetMenu(fileName = "ForwardRule", menuName = "LSystem/Rules/ForwardRule")]
	public class ForwardRule : BaseRule
	{
		[SerializeField] private BaseRule addBranchRule;
		[SerializeField] private int minLeaves = 2;
		[SerializeField] private int maxLeaves = 4;
		[SerializeField] private float leaveSize = 0.08f;
		[SerializeField] private Vector2 leaveAngle = new Vector2(30, 45);
		[SerializeField] private TransformObjectPool leafPool;

		private GameObject[] leaves;

		public override bool ApplyRule(LSystemData data, LSystemInterpreter interpreter, LSystemGenerator generator)
		{
			leaves = new GameObject[maxLeaves * 2];
			var branch = data.branch;
			var ivyParams = generator.ivyParams;
			var lastNode = branch.LastNode;
			var newDir = branch.mainGrowDirection;
			var newPos = lastNode.position + newDir * ivyParams.stepSize;
			var query = new BranchQueryInfo() {
				branch = branch,
				point = newPos,
				direction = newDir,
				normal = -lastNode.surfaceNormal,
				stepLength = ivyParams.stepSize,
				ivyParams = ivyParams
			};

			var leaveAmount = generator.Container.Rng.Range(minLeaves, maxLeaves + 1);
			for (int i = 0; i < leaves.Length; i++) {
				leaves[i] = leafPool.GetObject(false).gameObject;
			}
			if (EnvironmentHelper.EnvironmentQuery(query, false, false, false, out var response)) {
				branch.mainGrowDirection = response.newDirection;
				GrowUtil.AddNode(data.branch, response.newPoint, response.newNormal, true, true);
				if (branch.Count > 1) {
					LeafSpawnHelper.SpawnLeaves(ref leaves, leaveAmount, leaveSize, leaveAngle, query, generator.Container.Rng, false);
				}
			}

			for (int i = 0; i < leaves.Length; i++) {
				leafPool.ReleaseObject(leaves[i].transform);
			}

			return false;
		}

		public override BaseRule[] Parse(LSystemGenerator generator)
		{
			var paramsData = generator.ivyParams;
			var rng = generator.Container.Rng.Value;

			if (rng <= paramsData.branchProbability) {
				return new[] { this, addBranchRule };
			}

			return base.Parse(generator);
		}
	}
}
