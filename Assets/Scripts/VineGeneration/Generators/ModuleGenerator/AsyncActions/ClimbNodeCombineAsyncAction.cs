using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using General.MeshBuilding;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;
namespace VineGeneration.Generators.ModuleGenerator.AsyncActions
{
	[CreateAssetMenu(fileName = "ClimbNodeCombineAsyncAction", menuName = "VineGeneration/Async Actions/ClimbNodeCombine AsyncAction")]
	public class ClimbNodeCombineAsyncAction : BaseAsyncAction
	{
		[SerializeField] private int maxCombinesPerFrame = 1;

		public override void Initialize(ModularGenerator gen, GeneratorContainer cont)
		{
			base.Initialize(gen, cont);
			var obj = GrowUtil.CreateMeshCollection("CombinedClimbNode");
			obj.meshRenderer.sharedMaterial = gen.ivyParams.meshParameters.sharedClimbNodeMaterial;
			gen.combinedClimbNodeMesh = new List<MeshCollection> { obj };
		}
		
		protected override async UniTask ExecuteInternal()
		{
			if (token.IsCancellationRequested) {
				return;
			}
			int counter = 0;
			for (int i = 0; i < branches.Count; i++) {
				var branch = branches[i];
				if (branch.alive) {
					continue;
				}

				var node = branch.climbNodeMesh;
				if (node == null || !node.meshFilter || node.meshFilter.sharedMesh == null) {
					continue;
				}

				if (!node.isCombined && node.isStatic && node.objRef) {
					if (BMeshBuilder.AddToCombinedMesh("combined_climbNode", ref generator.combinedClimbNodeMesh, node.meshFilter.sharedMesh, node.objRef.transform, true, 5000)) {
						node.isCombined = true;
						GrowUtil.ReleaseMeshCollection(node);
						counter++;
						// if (generator.combinedClimbNodeMesh[^1].transform.parent == null) {
						// 	generator.combinedClimbNodeMesh[^1].transform.SetParent(GlobalVineSettings.current.GetClimbPointParent().transform);
						// }
					}
				}

				if (counter >= maxCombinesPerFrame) {
					counter = 0;
					await UniTask.Yield();
				}
			}

		}
	}
}
