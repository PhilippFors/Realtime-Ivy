using System.Collections.Generic;
using ScriptableObjectPools;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace General.MeshBuilding
{
	public static class BMeshBuilder
	{
		private static void BuildMesh(Branch branch, int startNode)
		{
			var branchMeshData = branch.branchMesh[^1];

			var bMesh = new BMesh();
			bMesh.AddVertexAttribute("uv", BMesh.AttributeBaseType.Float, 2);

			var points = branch.nodes;
			for (int i = startNode; i < points.Count; i++) {
				Vector3 forward = Vector3.zero;
				if (i < points.Count - 1) {
					forward += points[i + 1].position - points[i].position;
				}

				if (i > 0) {
					forward += points[i].position - points[i - 1].position;
				}

				forward.Normalize();
				var norm = points[i].surfaceNormal;
				var left = Quaternion.AngleAxis(-90, -norm) * forward;
				var pos = points[i].position;
				Vector3 pointA = pos + left * branch.thickness;
				Vector3 pointB = pos - left * branch.thickness;
				var vertA = bMesh.AddVertex(pointA);
				var vertB = bMesh.AddVertex(pointB);

				BMesh.Vertex lastA;
				BMesh.Vertex lastB;

				if (i > 0 && bMesh.vertices.Count > 3) {
					lastB = bMesh.vertices[^3];
					lastA = bMesh.vertices[^4];
					bMesh.AddFace(lastB, vertB, vertA, lastA);
				}
				else if (branch.branchMesh.Count > 1) {
					lastB = branch.branchMesh[^2].bMesh.vertices[^3];
					lastA = branch.branchMesh[^2].bMesh.vertices[^4];
					bMesh.EnsureVertexAttributes(lastB);
					bMesh.EnsureVertexAttributes(lastA);
					bMesh.vertices.Insert(0, lastA);
					bMesh.vertices.Insert(0, lastB);
					bMesh.AddFace(lastB, vertB, vertA, lastA);
				}
			}

			var filter = branchMeshData.objRef.GetComponent<MeshFilter>();
			BMeshUnity.SetInMeshFilter(bMesh, filter, filter.sharedMesh);
			branchMeshData.bMesh = bMesh;
		}

		public static void AddMeshNode(Branch branch, BranchNode node, bool buildMesh, bool useVertLimit)
		{
			if (branch.Count < 2) {
				return;
			}

			var branchMesh = branch.branchMesh[^1];
			if (branchMesh.bMesh == null) {
				var nodeIndex = node.index;
				BuildMesh(branch, branch.branchMesh.Count > 1 ? branch.nodes.FindIndex(n => nodeIndex == n.index) : 0);
			}

			if (useVertLimit && branchMesh.bMesh.vertices.Count > 500) {
				var prevRender = branchMesh.objRef.GetComponent<MeshRenderer>();
				GrowUtil.CreateNewBranchGameObject(branch);
				branchMesh = branch.branchMesh[^1];
				branchMesh.objRef.GetComponent<MeshRenderer>().sharedMaterial = prevRender.sharedMaterial;
				var nodeIndex = node.index;
				BuildMesh(branch, branch.nodes.FindIndex(n => nodeIndex == n.index));
			}

			var bMesh = branchMesh.bMesh;
			var forward = branch[^1].position - branch[^2].position;
			forward.Normalize();
			var norm = node.surfaceNormal;
			var left = Quaternion.AngleAxis(-90, -norm) * forward;
			var pos = node.position;
			Vector3 pointA = pos + left * branch.thickness;
			Vector3 pointB = pos - left * branch.thickness;

			var vertA = bMesh.AddVertex(pointA);
			var vertB = bMesh.AddVertex(pointB);
			BMesh.Vertex lastB;
			BMesh.Vertex lastA;

			if (bMesh.vertices.Count > 2) {
				lastB = bMesh.vertices[^3];
				lastA = bMesh.vertices[^4];
			}
			else {
				lastB = branch.branchMesh[^2].bMesh.vertices[^3];
				lastA = branch.branchMesh[^2].bMesh.vertices[^4];
				bMesh.EnsureVertexAttributes(lastB);
				bMesh.EnsureVertexAttributes(lastA);
				bMesh.vertices.Insert(0, lastA);
				bMesh.vertices.Insert(0, lastB);
			}

			bMesh.AddFace(lastB, vertB, vertA, lastA);

			if (buildMesh) {
				SetInMeshFilter(branch);
			}
		}

		public static void BuildUVs(Branch branch)
		{
			foreach (var mesh in branch.branchMesh) {
				var bMesh = mesh.bMesh;
				if (bMesh == null || bMesh.vertices == null) {
					continue;
				}
				for (int i = 0; i < bMesh.vertices.Count; i++) {
					var step = 1f / bMesh.vertices.Count;
					var uvLeft = new Vector2(0, i * step);
					var uvRight = new Vector2(1, i * step);
					bMesh.vertices[i].attributes[BMeshUnity.uvId] = new BMesh.FloatAttributeValue(uvLeft.x, uvLeft.y);
					if (i + 1 < bMesh.vertices.Count) {
						bMesh.vertices[i + 1].attributes[BMeshUnity.uvId] = new BMesh.FloatAttributeValue(uvRight.x, uvRight.y);
					}
				}
			}
		}

		public static void SetInMeshFilter(Branch branch)
		{
			if (branch.branchMesh.Count == 0) {
				return;
			}

			var mesh = branch.branchMesh[^1];
			if (!mesh.meshFilter.sharedMesh || !mesh.objRef) {
				return;
			}
			BMeshUnity.SetInMeshFilter(mesh.bMesh, mesh.meshFilter, mesh.meshFilter.sharedMesh);
		}

		private static bool change;
		private static List<CombineInstance> combineInstances = new();

		private static void TraverseChildBranches(Branch mainParent, List<CombineInstance> instances, Branch next)
		{
			foreach (var child in next.children) {
				Mesh mesh = child.branchMesh[^1].meshFilter.sharedMesh;
				// if (child.hasChanged)
				// {
				//     change = true;
				//     child.hasChanged = false;
				// }

				if (mesh != null) {
					var combineInstance = new CombineInstance {
						mesh = mesh,
						transform = mainParent.branchMesh[^1].objRef.transform.localToWorldMatrix
					};
					instances.Add(combineInstance);
				}

				if (child.children.Count > 0) {
					TraverseChildBranches(mainParent, instances, child);
				}
			}
		}

		public static bool AddToCombinedMesh(string name, ref List<MeshCollection> combinedList, Mesh toCombine, Transform toCombineTransform, bool useVertLimit, int vertLimit = 2000)
		{
			if (combinedList.Count == 0) {
				return false;
			}
			
			var lastMeshFilter = combinedList[^1].meshFilter;
			var lastTransform = lastMeshFilter.transform;
			var meshPool = ObjectPoolCollection.current.GetBasePool<MeshObjectPool>("Mesh");
			var newCombineMesh = meshPool.GetObject();
			
			// if (lastMeshFilter.sharedMesh.vertexCount == 0) {
			// 	newCombineMesh.SetVertices(toCombine.vertices);
			// 	newCombineMesh.SetTriangles(toCombine.triangles, 0);
			// 	newCombineMesh.SetIndices(toCombine.GetIndices(0), MeshTopology.Triangles, 0);
			// 	newCombineMesh.RecalculateNormals();
			// 	newCombineMesh.RecalculateBounds();
			// 	GrowUtil.ReleaseMesh(lastMeshFilter);
			// 	lastMeshFilter.sharedMesh = newCombineMesh;
			// 	return true;
			// }

			if (toCombine == null) {
				return false;
			}
			
			if (useVertLimit && lastMeshFilter.sharedMesh.vertexCount > vertLimit) {
				var newGo = GrowUtil.CreateMeshCollection(name);
				combinedList.Add(newGo);
				var renderer = newGo.meshRenderer;
				renderer.sharedMaterial = lastMeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
				return false;
			}

			var instances = new CombineInstance[] {
				new CombineInstance {
					mesh = toCombine,
					transform = toCombineTransform.localToWorldMatrix
				},
				new CombineInstance {
					mesh = lastMeshFilter.sharedMesh,
					transform = lastTransform.localToWorldMatrix
				},
			};

			newCombineMesh.CombineMeshes(instances);
			newCombineMesh.RecalculateNormals();
			newCombineMesh.RecalculateBounds();
			GrowUtil.ReleaseMesh(lastMeshFilter);
			lastMeshFilter.sharedMesh = newCombineMesh;
			return true;
		}
	}
}
