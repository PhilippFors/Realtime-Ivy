using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.AsyncActions
{
	[CreateAssetMenu(fileName = "BranchRemoveAsyncAction", menuName = "VineGeneration/Async Actions/BranchDespawn AsyncAction")]
	public class BranchDespawnAsyncAction : BaseAsyncAction
	{
		private bool run;

		public override void Initialize(ModularGenerator gen, GeneratorContainer cont)
		{
			base.Initialize(gen, cont);
			run = false;
		}

		protected override UniTask ExecuteInternal()
		{
			if (generator.seed.seedActive || run || token.IsCancellationRequested) {
				return UniTask.CompletedTask;
			}
			
			run = true;

			for (int i = 0; i < branches.Count; i++) {
				var br = branches[i];
				if (!br.hasParent) {
					br.alive = false;
					DeSpawnBranch(br);
					branches.Remove(br);
					i--;
				}
			}

			if (generator.combinedBranchMesh != null) {
				for (int i = 0; i < generator.combinedBranchMesh.Count; i++) {
					var go = generator.combinedBranchMesh[i];
					var renderer = go.meshRenderer;
					var mat = renderer.material;
					mat.DOFloat(0, "_Alpha", 0.4f).onComplete += () => {
						GrowUtil.ReleaseMesh(go.meshFilter);
						GrowUtil.ReleaseMeshRenderer(renderer);
						Destroy(mat);
					};
				}
				generator.combinedBranchMesh.Clear();
			}

			if (generator.combinedLeafMesh != null) {
				for (int i = 0; i < generator.combinedLeafMesh.Count; i++) {
					var go = generator.combinedLeafMesh[i];
					var renderer = go.meshRenderer;
					var mat = renderer.material;
					mat.DOFloat(0, "_Alpha", 0.4f).onComplete += () => {
						GrowUtil.ReleaseMesh(go.meshFilter);
						GrowUtil.ReleaseMeshRenderer(renderer);
						Destroy(mat);
					};
				}
				generator.combinedLeafMesh.Clear();
			}

			if (generator.combinedClimbNodeMesh != null) {
				for (int i = 0; i < generator.combinedClimbNodeMesh.Count; i++) {
					var go = generator.combinedClimbNodeMesh[i];
					var renderer = go.meshRenderer;
					GrowUtil.ReleaseMesh(go.meshFilter);
					GrowUtil.ReleaseMeshRenderer(renderer);
				}
				generator.combinedClimbNodeMesh.Clear();
			}

			branches.Clear();

			return UniTask.CompletedTask;
		}

		private void DeSpawnBranch(Branch branch)
		{
			if (branch.children.Count > 0) {
				DeSpawnAllBranches(branch);
				branch.children.Clear();
			}

			DestroyNodes(branch);

			foreach (var leaf in branch.leafMesh) {
				if (leaf.objRef) {
					var leaveMat = leaf.meshRenderer.material;
					leaveMat.DOFloat(0, "_Alpha", 0.4f).onComplete += () => {
						GrowUtil.ReleaseMeshCollection(leaf);
						Destroy(leaveMat);
					};
				}
			}

			foreach (var mesh in branch.branchMesh) {
				if (mesh.objRef) {
					var branchMat = mesh.meshRenderer.material;
					branchMat.DOFloat(0, "_Alpha", 0.5f).onComplete += () => {
						GrowUtil.ReleaseMeshCollection(mesh);
						Destroy(branchMat);
					};
				}
			}

			branch.branchMesh.Clear();
			branch.leafMesh.Clear();
		}

		private void DeSpawnAllBranches(Branch br)
		{
			foreach (var b in br.children) {
				foreach (var mesh in b.branchMesh) {
					if (mesh.objRef) {
						GrowUtil.ReleaseMeshCollection(mesh);
					}
				}

				foreach (var leaf in b.leafMesh) {
					if (leaf.objRef) {
						GrowUtil.ReleaseMeshCollection(leaf);
					}
				}

				DestroyNodes(b);

				if (b.children.Count > 0) {
					DeSpawnAllBranches(b);
				}

				b.children.Clear();
				b.branchMesh.Clear();
				b.leafMesh.Clear();
			}

			generator.Branches.Remove(br);
		}

		private void DestroyNodes(Branch b)
		{
			if (b.climbNodeMesh.objRef != null) {
				var mat = b.climbNodeMesh.meshRenderer.material;
				mat.DOFloat(0, "_Alpha", 0.3f).onComplete += () => {
					GrowUtil.ReleaseMeshCollection(b.climbNodeMesh);
					Destroy(mat);
				};
			}

			for (int i = 0; i < b.Count; i++) {
				var n = b[i];
				b.RemoveNode(n);
				if (n.climbNode != null) {
					n.Destroy();
				}
				i--;
			}
		}

		public override void ResetModule()
		{
			run = false;
		}
	}
}
