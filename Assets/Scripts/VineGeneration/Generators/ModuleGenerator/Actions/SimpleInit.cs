using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;
namespace VineGeneration.Generators.ModuleGenerator.Actions
{
	[CreateAssetMenu(fileName = "SimpleInit", menuName = "VineGeneration/Actions/Init/SimpleInit")]
	public class SimpleInit : BaseGrowAction
	{
		public override bool Execute(ref BranchQueryInfo info)
		{
			var normal = info.normal;
			var randomAngle = container.Rng.Range(0, 360);
			var randomDir = Quaternion.AngleAxis(randomAngle, normal) * Vector3.forward;
			var projected = Vector3.ProjectOnPlane(randomDir, normal);
			var newPoint = info.point + projected * info.ivyParams.stepSize;
			GrowUtil.AddNode(info.branch, newPoint, normal,false, true);
			info.point = newPoint;
			info.direction = projected;
			info.branch.mainGrowDirection = projected;
			return true;
		}
	}
}
