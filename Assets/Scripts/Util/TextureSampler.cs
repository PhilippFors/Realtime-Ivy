using System.Collections.Generic;
using System.Linq;
using KNN;
using KNN.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Util
{
    public static class TextureSampler
    {
        private static readonly HashSet<Vector2> closestMeshUV = new HashSet<Vector2>();
        private static readonly HashSet<Vector3> closestMeshVertex = new HashSet<Vector3>();

        public static Color SampleTexture(Mesh mesh, Texture2D tex, Vector3 pos)
        {
            var verts = mesh.vertices;
            var uvs = new List<Vector2>();
            mesh.GetUVs(0, uvs);
            var closest = closestMeshUV.ToArray();
            var closestV = closestMeshVertex.ToArray();

            foreach (var v in closestV) {
                Debug.Log($"VertexPos: {v}");
            }

            foreach (var u in closest) {
                Debug.Log($"UV: {u}");
            }

            var avg = Vector2.zero;
            foreach (var px in closest) {
                avg += px;
            }

            avg /= closest.Length;

            var getPixelCoord = GetPixelCoords(avg, tex);
            var pixels = tex.GetPixel(getPixelCoord.x, getPixelCoord.y);
            return pixels;
        }

        public static Vector2Int GetPixelCoords(Vector2 uv, Texture2D tex)
            => new(Mathf.FloorToInt(uv.x * tex.width),
                Mathf.FloorToInt(uv.y * tex.height));
        
        
        // Just gets the nearest vertex which is useless to me.
        private static void CalculateClosestUV(Vector3[] verts, Vector2[] uvs, Vector3[] checkPoints)
        {
            closestMeshUV.Clear();
            closestMeshVertex.Clear();
            var points = new NativeArray<float3>(verts.Length, Allocator.Persistent);

            for (var i = 0; i < points.Length; ++i) {
                points[i] = verts[i];
            }

            var knnContainer = new KnnContainer(points, true, Allocator.TempJob);
            var results = new NativeArray<int>(3, Allocator.TempJob);

            var queryPositions = new NativeArray<float3>(checkPoints.Length, Allocator.TempJob);
            for (var i = 0; i < queryPositions.Length; ++i) {
                queryPositions[i] = checkPoints[i];
            }

            // Batch query
            var batchQueryJob = new QueryKNearestBatchJob(knnContainer, queryPositions, results);
            batchQueryJob.Execute(0);

            for (var i = 0; i < results.Length; i++) {
                var index = results[i];
                var uv = uvs[index];
                var v = verts[index];
                closestMeshUV.Add(uv);
                closestMeshVertex.Add(v);
            }

            queryPositions.Dispose();
            results.Dispose();
            points.Dispose();
            knnContainer.Dispose();
        }
    }
}