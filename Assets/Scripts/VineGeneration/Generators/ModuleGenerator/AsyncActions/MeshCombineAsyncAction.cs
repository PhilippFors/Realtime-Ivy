using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using General.MeshBuilding;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.AsyncActions
{
	[CreateAssetMenu(fileName = "MeshCombineAsyncAction", menuName = "VineGeneration/Async Actions/MeshCombine AsyncAction")]
	public class MeshCombineAsyncAction : BaseAsyncAction
	{
		[SerializeField] private int maxCombinesPerFrame = 5;
		
		public override void Initialize(ModularGenerator gen, GeneratorContainer cont)
		{
			base.Initialize(gen, cont);
			var obj = GrowUtil.CreateMeshCollection("CombinedBranch");
			obj.meshRenderer.sharedMaterial = gen.ivyParams.meshParameters.sharedBranchMaterial;
			gen.combinedBranchMesh = new List<MeshCollection> { obj };
		}

		protected override async UniTask ExecuteInternal()
		{
			for (int i = 0; i < branches.Count; i++) {
				var branch = branches[i];
				if (branch.alive) {
					continue;
				}

				int counter = 0;

				for (int j = 0; j < branch.branchMesh.Count; j++) {
					var branchM = branch.branchMesh[j];
					if (branchM.meshFilter.sharedMesh == null) {
						continue;
					}
					if (!branchM.isCombined && branchM.isStatic && branchM.objRef) {
						if (BMeshBuilder.AddToCombinedMesh("combined_branch", ref generator.combinedBranchMesh, branchM.meshFilter.sharedMesh, branchM.objRef.transform, true, 5000)) {
							branchM.isCombined = true;
							GrowUtil.ReleaseMeshCollection(branchM);
							branch.branchMesh.Remove(branchM);
							j--;
							counter++;
							// if (generator.combinedBranchMesh[^1].transform.parent == null) {
							// 	generator.combinedBranchMesh[^1].transform.SetParent(GlobalVineSettings.current.GetBranchParent().transform);
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
}
