using System.Collections.Generic;
using ScriptableObjectPools;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.Actions.PostNodeSpawnActions
{
	[CreateAssetMenu(menuName = "VineGeneration/Actions/Try Place Climbnode")]
	public class TryPlaceClimbNodeAction : BaseGrowAction
	{
		public Material transparentMat;
		public Material opaqueMat;
		public bool setMatOpaque;

		public override bool Execute(ref BranchQueryInfo info)
		{
			if (!info.branch.hasChanged) {
				return false;
			}

			if (GrowUtil.TryAddClimbNode(container.Rng, info.branch.LastNode, info.ivyParams)) {
				CombineClimbNodes(info.branch);
			}
			return true;
		}

		private void CombineClimbNodes(Branch branch)
		{
			var combineInstances = new List<CombineInstance>();
			if (branch.climbNodeMesh.objRef == null) {
				var newMeshCol = GrowUtil.CreateMeshCollection("climb_node");
				// newMeshCol.objRef.transform.SetParent(GlobalVineSettings.current.GetClimbPointParent().transform);
				branch.climbNodeMesh = newMeshCol;
				
				if (setMatOpaque) {
					newMeshCol.meshRenderer.sharedMaterial = opaqueMat;
				}
				else {
					newMeshCol.meshRenderer.sharedMaterial  = transparentMat;
				}
			}

			foreach (var n in branch.nodes) {
				if (!n.climbNode) {
					continue;
				}

				var meshRend = n.climbNode.GetComponentInChildren<MeshRenderer>(true);
				if (!meshRend || !meshRend.gameObject.activeSelf) {
					continue;
				}

				var combineInstance = new CombineInstance();
				var filter = meshRend.GetComponent<MeshFilter>();
				combineInstance.mesh = filter.sharedMesh;
				combineInstance.transform = meshRend.transform.localToWorldMatrix;
				combineInstances.Add(combineInstance);
				meshRend.enabled = false;
			}

			combineInstances.Add(new CombineInstance {
					mesh = branch.climbNodeMesh.meshFilter.sharedMesh,
					transform = branch.climbNodeMesh.objRef.transform.localToWorldMatrix
				}
			);

			var meshPool = ObjectPoolCollection.current.GetBasePool<MeshObjectPool>("Mesh");
			var newMesh = meshPool.GetObject();
			newMesh.CombineMeshes(combineInstances.ToArray());
			newMesh.RecalculateNormals();
			newMesh.RecalculateBounds();
			GrowUtil.ReleaseMesh(branch.climbNodeMesh.meshFilter);
			branch.climbNodeMesh.meshFilter.sharedMesh = newMesh;
		}
	}
}
