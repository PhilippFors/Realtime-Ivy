using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using General.MeshBuilding;
using UnityEngine;

namespace VineGeneration.Generators.ModuleGenerator.AsyncActions
{
	[CreateAssetMenu(fileName = "StartGrowShaderAsyncAction", menuName = "VineGeneration/Async Actions/StartGrowShaderAsyncAction")]
	public class StartGrowShaderAsyncAction : BaseAsyncAction
	{
		[SerializeField] private int maxBuilds = 5;
		[SerializeField] private Material climbMaterial;
		[SerializeField] private Material leafMaterial;

		private HashSet<int> grownBranches = new HashSet<int>();
		private int counter;

		protected override async UniTask ExecuteInternal()
		{
			for (int i = 0; i < branches.Count; i++) {
				var b = branches[i];
				if (b.alive || grownBranches.Contains(b.GetHashCode()) || !generator.seed.seedActive) {
					continue;
				}

				grownBranches.Add(b.GetHashCode());
				BMeshBuilder.BuildUVs(b);
				BMeshBuilder.SetInMeshFilter(b);
				var sequence = DOTween.Sequence();

				foreach (var mesh in b.branchMesh) {
					if (!mesh.objRef) {
						continue;
					}
					var renderer = mesh.meshRenderer;
					var branchMat = renderer.material;
					sequence.Append(branchMat.DOFloat(1, "_Grow", 0.3f / b.branchMesh.Count));
				}

				foreach (var leaf in b.leafMesh) {
					if (!leaf.objRef) {
						continue;
					}
					var rend = leaf.meshRenderer;
					var leafMat = rend.material;
					sequence.Append(leafMat.DOFloat(1, "_Alpha", 0.2f)).onComplete += () => {
						rend.sharedMaterial = leafMaterial;
						Destroy(leafMat);
					};
				}

				if (b.climbNodeMesh != null && b.climbNodeMesh.objRef) {
					var climbRend = b.climbNodeMesh.meshRenderer;
					var climbMat = climbRend.material;
					sequence.Append(climbMat.DOFloat(1, "_Alpha", 0.2f)).onComplete += () => {
						climbMaterial.SetFloat("_Alpha", 1);
						climbRend.sharedMaterial = climbMaterial;
						b.climbNodeMesh.isStatic = true;
						Destroy(climbMat);
					};
				}

				sequence.AppendCallback(() => {
					foreach (var mesh in b.branchMesh) {
						mesh.isStatic = true;
					}
					foreach (var leaf in b.leafMesh) {
						leaf.isStatic = true;
					}
				});

				sequence.Play();

				if (++counter >= maxBuilds) {
					counter = 0;
					await UniTask.Yield();
				}
			}
		}

		public override void ResetModule()
		{
			grownBranches.Clear();
		}
	}
}
