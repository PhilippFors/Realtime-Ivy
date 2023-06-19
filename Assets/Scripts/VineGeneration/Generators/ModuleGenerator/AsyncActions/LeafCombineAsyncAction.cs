using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using General.MeshBuilding;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.AsyncActions
{
	[CreateAssetMenu(fileName = "LeafCombineAsyncAction", menuName = "VineGeneration/Async Actions/LeafCombine AsyncAction")]
	public class LeafCombineAsyncAction : BaseAsyncAction
	{
		[SerializeField] private int maxCombinesPerFrame = 1;

		public override void Initialize(ModularGenerator gen, GeneratorContainer cont)
		{
			base.Initialize(gen, cont);
			var obj = GrowUtil.CreateMeshCollection("Combined_leafPers");
			obj.meshRenderer.sharedMaterial = gen.ivyParams.meshParameters.sharedLeafMaterial;
			gen.combinedLeafMesh = new List<MeshCollection> { obj };
		}

		protected override async UniTask ExecuteInternal()
		{
			for (int i = 0; i < branches.Count; i++) {
				var branch = branches[i];
				if (branch.alive) {
					continue;
				}

				int counter = 0;
				for (int j = 0; j < branch.leafMesh.Count; j++) {
					var leaf = branch.leafMesh[j];
					if (leaf.meshFilter.sharedMesh == null) {
						continue;
					}

					if (!leaf.isCombined && leaf.isStatic && leaf.objRef) {
						if (BMeshBuilder.AddToCombinedMesh("combined_leaf", ref generator.combinedLeafMesh, leaf.meshFilter.sharedMesh, leaf.objRef.transform, true, 4000)) {
							leaf.isCombined = true;
							GrowUtil.ReleaseMeshCollection(leaf);
							branch.leafMesh.Remove(leaf);
							j--;
							counter++;
							// if (generator.combinedLeafMesh[^1].transform.parent == null) {
							// 	generator.combinedLeafMesh[^1].transform.SetParent(GlobalVineSettings.current.GetLeaveParent().transform);
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
