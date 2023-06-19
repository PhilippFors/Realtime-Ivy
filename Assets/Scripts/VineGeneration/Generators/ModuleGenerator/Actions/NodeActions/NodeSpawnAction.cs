using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;
namespace VineGeneration.Generators.ModuleGenerator.Actions.NodeActions
{
	[CreateAssetMenu(fileName = "NodeSpawnAction", menuName = "VineGeneration/Actions/Node Actions/NodeSpawnAction")]
	public class NodeSpawnAction : BaseGrowAction
	{
		[SerializeField] private bool buildMesh = true;
		[SerializeField] private bool vertLimit = true;
		
		public override bool Execute(ref BranchQueryInfo info)
		{
			var newPoint = info.point + info.direction * info.stepLength;
			var newNormal = info.normal;
			Physics.Raycast(newPoint, -info.normal, out var hit, info.ivyParams.maxDistanceToSurface * 3, GlobalVineSettings.current.environmentMask);
			newPoint = hit.point + newNormal * info.ivyParams.maxDistanceToSurface;
			GrowUtil.AddNode(info.branch, newPoint, newNormal, vertLimit, buildMesh);
			return true;
		}
	}
}
