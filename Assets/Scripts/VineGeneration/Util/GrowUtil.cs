using System.Collections.Generic;
using System.Linq;
using General.MeshBuilding;
using ScriptableObjectPools;
using UnityEngine;
using UnityEngine.Rendering;
using Util;
using VineGeneration.Core;
using VineGeneration.Parameters;

namespace VineGeneration.Util
{
	public static class GrowUtil
	{
		private static int lastNumberAssigned;
		private static int[] results = new int[10];
		public static readonly List<Transform> climbNodes = new();
		
		public static void AddNode(Branch branch, Vector3 point, Vector3 normal, bool vertLimit, bool buildMesh)
		{
			BranchNode node = CreateNode(point, normal);
			if (point.magnitude < 0.2f) {
				Debug.Log($"WHAT THA FACK: {point}");
				return;
			}
			node.growDirection = branch.mainGrowDirection;
			branch.hasChanged = true;
			branch.AddNode(node);
			BMeshBuilder.AddMeshNode(branch, node, buildMesh, vertLimit);
		}

		public static bool TryAddClimbNode(RandomState state, BranchNode node, GrowParameters ivyParams)
		{
			if (!ivyParams.climbable || Vector3.Angle(-node.surfaceNormal, Vector3.up) < 5f) {
				return false;
			}

			if (state.Value < ivyParams.climbNodeProbability) {
				return false;
			}

			var climbPointDistance = state.Range(ivyParams.minClimbPointDistance, ivyParams.maxClimbPointDistance);

			var nodePositions = climbNodes.Select(t => t.position).ToArray();
			int hits = 0;
			if (KnnHelper.FillContainer(KnnIds.climbNodes, nodePositions)) {
				hits = KnnHelper.KnnRangeQueryNonAlloc(KnnIds.climbNodes, node.position, ref results, climbPointDistance, results.Length);
			}

			if (hits > ivyParams.maxClimbNodesInRadius) {
				KnnHelper.DisposeContainer(KnnIds.climbNodes);
				return false;
			}

			var newClimbPoint = GameObject.Instantiate(ivyParams.climbNodePrefab);
			newClimbPoint.transform.position = node.position;
			newClimbPoint.transform.forward = node.surfaceNormal;
			// newClimbPoint.transform.parent = GlobalVineSettings.current.GetClimbPointParent().transform;
			// newClimbPoint.transform.localScale = Vector3.one * (ivyParams.branchThickness * 1.7f);
			node.climbNode = newClimbPoint.transform;
			climbNodes.Add(newClimbPoint.transform);
			KnnHelper.DisposeContainer(KnnIds.climbNodes);
			return true;
		}

		public static BranchNode CreateNode(Vector3 position, Vector3 normal)
		{
			var newVineNode = new BranchNode();
			newVineNode.position = position;
			newVineNode.surfaceNormal = -normal;
			return newVineNode;
		}

		public static Branch CopyBranch(RandomState state, Branch branch, BranchNode origin)
		{
			var newBranch = new Branch() {
				ogSeed = branch.ogSeed,
				mainGrowDirection = branch.mainGrowDirection,
				currentHeight = branch.currentHeight,
				branchSense = ChooseBranchSense(state),
				branchNumber = lastNumberAssigned++,
				alive = true,
			};

			CreateNewBranchGameObject(newBranch);

			AddNode(newBranch, origin.position, -origin.surfaceNormal, false, true);

			return newBranch;
		}

		public static Branch CreateChildBranch(RandomState state, Branch branch, BranchNode node)
		{
			var br = CopyBranch(state, branch, node);
			br.hasParent = true;
			br.branchDepth = branch.branchDepth + 1;
			br.initialized = true;
			branch.children.Add(br);
			return br;
		}

		public static Branch GetBranch(RandomState state, Vector3 origin, Vector3 normal, VineSeed seed, bool createGo)
		{
			var newBranch = new Branch();
			newBranch.ogSeed = seed;
			newBranch.alive = true;
			newBranch.branchSense = ChooseBranchSense(state);
			newBranch.branchNumber = lastNumberAssigned++;

			if (createGo) {
				CreateNewBranchGameObject(newBranch);
			}

			AddNode(newBranch, origin, normal, false, true);

			return newBranch;
		}

		public static void CreateNewBranchGameObject(Branch branch)
		{
			var meshCol = CreateMeshCollection($"Branch_{branch.branchNumber}");
			meshCol.objRef.transform.position = Vector3.zero;
			// rend.transform.parent = GlobalVineSettings.current.GetBranchParent().transform;
			meshCol.meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
			branch.branchMesh.Add(meshCol);
		}

		public static GameObject CreateNewGameObject(string name)
		{
			var pool = ObjectPoolCollection.current.GetPool<MeshRenderer>("MeshRenderer");
			var rend = pool.GetObject();
			rend.name = $"{name}_{lastNumberAssigned++}";
			rend.transform.position = Vector3.zero;
			rend.shadowCastingMode = ShadowCastingMode.Off;
			return rend.gameObject;
		}

		public static MeshCollection CreateMeshCollection(string name)
		{
			var col = new MeshCollection();
			var meshPool = ObjectPoolCollection.current.GetBasePool<MeshObjectPool>("Mesh");
			var gameObject = CreateNewGameObject(name);
			col.objRef = gameObject;
			col.meshRenderer = gameObject.GetComponent<MeshRenderer>();
			col.meshFilter = gameObject.GetComponent<MeshFilter>();
			col.meshFilter.sharedMesh = meshPool.GetObject();
			return col;
		}

		public static void ReleaseMeshCollection(MeshCollection collection)
		{
			var rend = collection.meshRenderer;
			var filter = collection.meshFilter;

			collection.meshRenderer = null;
			collection.meshFilter = null;
			collection.objRef = null;

			ReleaseMesh(filter);
			ReleaseMeshRenderer(rend);
		}

		public static void ReleaseMeshRenderer(MeshRenderer render)
		{
			if (!render) {
				return;
			}
			var pool = ObjectPoolCollection.current.GetPool<MeshRenderer>("MeshRenderer");
			pool.ReleaseObject(render);
		}

		public static void ReleaseMesh(MeshFilter meshFilter)
		{
			if (!meshFilter || meshFilter.sharedMesh == null) {
				return;
			}
			var meshPool = ObjectPoolCollection.current.GetBasePool<MeshObjectPool>("Mesh");
			var mesh = meshFilter.sharedMesh;
			meshFilter.sharedMesh = null;
			meshPool.Release(mesh);
		}

		public static Vector3 RotateTowards(this Vector3 start, Vector3 dir, float strength)
		{
			var angle = Vector3.Angle(start, dir);
			var rotated = Vector3.RotateTowards(start, dir, strength * angle * Mathf.Deg2Rad, 360 * Mathf.Deg2Rad);
			return rotated;
		}

		private static int ChooseBranchSense(RandomState state)
		{
			return state.Value < 0.5f ? -1 : 1;
		}
	}
}
