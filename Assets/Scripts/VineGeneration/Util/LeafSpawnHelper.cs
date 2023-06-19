using System.Collections.Generic;
using General.MeshBuilding;
using ScriptableObjectPools;
using UnityEngine;
using Util;
using VineGeneration.Core;

namespace VineGeneration.Util
{
	public static class LeafSpawnHelper
	{
		private static int leaveCounter;

		public static void SpawnLeaves(ref GameObject[] leaves, int leaveAmount, float leaveSize, Vector2 leaveAngle, BranchQueryInfo info, RandomState rng, bool useVertLimit)
		{
			var prevNode = info.branch[^2];
			var lastNode = info.branch.LastNode;
			var prevPos = info.branch[^2].position;
			var lastPos = info.branch.LastNode.position;
			var ivyParams = info.ivyParams;
			var dir = (lastPos - prevPos).normalized;
			var dist = Vector3.Distance(prevPos, lastPos);
			var rightStart = Quaternion.AngleAxis(90, prevNode.surfaceNormal) * dir;
			var rightEnd = Quaternion.AngleAxis(90, lastNode.surfaceNormal) * lastNode.growDirection;
			var minThickness = ivyParams.branchThickness * 0.4f;
			var step = dist / leaveAmount;
			var stepVariation = step * 0.2f;

			int spawns = 0;

			void spawnLeaf(int i, bool right, GameObject leaf)
			{
				var r = right ? 1 : -1;
				var randomDist = step * i + rng.Range(-stepVariation, stepVariation);
				var normalLerp = Vector3.Lerp(-prevNode.surfaceNormal, -lastNode.surfaceNormal, randomDist / dist);
				var rightLerp = Vector3.Lerp(rightStart, rightEnd, randomDist / dist) * r;
				var randomThickness = rng.Range(minThickness, ivyParams.branchThickness);
				var leavePos = prevPos + dir * randomDist + rightLerp * randomThickness + normalLerp * 0.01f;
				var leaveRot = Quaternion.AngleAxis(rng.Range(leaveAngle.x, leaveAngle.y) * r, normalLerp) * rightLerp;
				var meshParams = ivyParams.meshParameters;
				leaf.GetComponentInChildren<MeshRenderer>().sharedMaterial = meshParams.leafMaterial;
				leaf.transform.position = leavePos;
				leaf.transform.forward = normalLerp;
				leaf.transform.rotation = Quaternion.LookRotation(leaveRot, normalLerp);
				leaf.transform.localScale = leaveSize * (1 - rng.Range(0, 0.08f)) * Vector3.one;
				spawns++;
			}

			for (int i = 0; i < leaveAmount; i += 2) {
				spawnLeaf(i, true, leaves[i]);
				spawnLeaf(i, false, leaves[i + 1]);
			}

			CombineLeaves(info.branch, leaves, spawns, useVertLimit);
		}

		private static void CombineLeaves(Branch branch, GameObject[] leaves, int leaveAmount, bool useVertLimit)
		{
			var meshPool = ObjectPoolCollection.current.GetBasePool<MeshObjectPool>("Mesh");
			var sharedMaterial = leaves[0].GetComponentInChildren<MeshRenderer>().sharedMaterial;
			if (branch.leafMesh.Count == 0) {
				var obj = CreateLeafObject(branch, sharedMaterial);
				branch.leafMesh.Add(obj);
			}

			var lastMeshCol = branch.leafMesh[^1];

			if (useVertLimit && lastMeshCol.meshFilter.sharedMesh.vertexCount > 850) {
				var obj = CreateLeafObject(branch, sharedMaterial);
				branch.leafMesh.Add(obj);
				lastMeshCol = obj;
			}

			var combineInstances = new List<CombineInstance>();
			for (int i = 0; i < leaveAmount; i++) {
				var filter = leaves[i].GetComponentInChildren<MeshFilter>();
				var mesh = filter.sharedMesh;
				var instance = new CombineInstance();
				instance.mesh = mesh;
				instance.transform = filter.transform.localToWorldMatrix;
				combineInstances.Add(instance);
			}
			var groupMesh = lastMeshCol.meshFilter.sharedMesh;
			combineInstances.Add(new CombineInstance() {
					mesh = groupMesh,
					transform = lastMeshCol.objRef.transform.localToWorldMatrix
				}
			);

			var newMesh = meshPool.GetObject();
			newMesh.CombineMeshes(combineInstances.ToArray(), true);
			GrowUtil.ReleaseMesh(lastMeshCol.meshFilter);
			lastMeshCol.meshFilter.sharedMesh = newMesh;
		}

		private static MeshCollection CreateLeafObject(Branch branch, Material mat)
		{
			var meshCollection = GrowUtil.CreateMeshCollection($"LeaveNode_{leaveCounter++}");
			// leaveParent.transform.parent = GlobalVineSettings.current.GetLeaveParent().transform;
			meshCollection.meshRenderer.sharedMaterial = mat;
			return meshCollection;
		}
	}
}
