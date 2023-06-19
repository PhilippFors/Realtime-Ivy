using System.Collections.Generic;
using System.Linq;
using KNN;
using KNN.Jobs;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace VineGeneration.Util
{
    public static class KnnIds
    {
        public static int breadCrumbs = nameof(breadCrumbs).GetHashCode();
        public static int climbNodes = nameof(climbNodes).GetHashCode();
    }

    public class KnnData
    {
        public KnnContainer container;
        public NativeArray<float3> points;
        public bool initialized;
    }

    public static class KnnHelper
    {
        private static Dictionary<int, KnnData> knnData = new Dictionary<int, KnnData>();

        public static void Init()
        {
            knnData = new Dictionary<int, KnnData>();
        }
        
        public static KnnContainer GetContainer(int id)
        {
            if (knnData.TryGetValue(id, out var data))
            {
                return data.container;
            }

            Debug.LogError("KnnContainer not found");
            return default;
        }

        public static bool FillContainer(int id, Vector3[] points)
        {
            if (points.Length == 0)
            {
                return false;
            }

            var fill = new NativeArray<float3>(points.Length, Allocator.Persistent);
            for (int i = 0; i < points.Length; i++)
            {
                fill[i] = points[i];
            }

            var container = new KnnContainer(fill, true, Allocator.Persistent);
            if (!knnData.ContainsKey(id))
            {
                var data = new KnnData()
                {
                    container = container,
                    points = fill,
                    initialized = true,
                };
                knnData.Add(id, data);
            }
            else
            {
                var data = knnData[id];
                data.container = container;
                data.points = fill;
                data.initialized = true;
            }

            return true;
        }

        public static void DisposeContainer(int id)
        {
            if (knnData.TryGetValue(id, out var knn) && knn.initialized)
            {
                knn.container.Dispose();
                knn.points.Dispose();
                knn.initialized = false;
            }
        }

        public static void KnnRangeQuery(this KnnContainer knn, Vector3 samplePoint, float range,
            out List<int> resultList)
        {
            var rangeResults = new NativeList<int>(Allocator.TempJob);
            var kJob = new QueryRangeJob(knn, samplePoint, range, rangeResults);
            kJob.Schedule().Complete();

            resultList = new List<int>();
            resultList.AddRange(rangeResults.ToArray());

            rangeResults.Dispose();
        }

        public static int KnnRangeQueryNonAlloc(this KnnContainer knn, Vector3 samplePoint,
            ref int[] resultArr, float range, int querySize)
        {
            var results = new NativeList<int>(Allocator.TempJob);
            var kJob = new QueryRangeJob(knn, samplePoint, range, results);
            kJob.Schedule().Complete();

            int hits = 0;
            for (int i = 0; i < querySize || i < results.Length; i++)
            {
                resultArr[i] = results[i];
                hits++;
            }

            results.Dispose();
            return hits;
        }

        public static int KnnRangeQueryNonAlloc(int id, Vector3 samplePoint, ref int[] resultArr, float range,
            int querySize)
        {
            if (!knnData.ContainsKey(id) || !knnData[id].initialized)
            {
                return 0;
            }

            var knn = knnData[id].container;
            var results = new NativeList<int>(Allocator.TempJob);
            var kJob = new QueryRangeJob(knn, samplePoint, range, results);
            kJob.Schedule().Complete();
            int hits = 0;
            for (int i = 0; i < querySize && i < results.Length && i < resultArr.Length; i++)
            {
                resultArr[i] = results[i];
                hits++;
            }

            results.Dispose();
            return hits;
        }

        public static void KnnQuery(this KnnContainer knn, Vector3 samplePoint, out int[] resultList, int querySize)
        {
            var results = new NativeArray<int>(querySize, Allocator.TempJob);
            var kJob = new QueryKNearestJob(knn, samplePoint, results);
            kJob.Schedule().Complete();

            resultList = new int[querySize];
            for (int i = 0; i < querySize; i++)
            {
                resultList[i] = results[i];
            }

            results.Dispose();
        }

        public static void KnnQuery(int id, Vector3 samplePoint, out int[] resultList, int querySize)
        {
            var knn = knnData[id].container;
            var results = new NativeArray<int>(querySize, Allocator.TempJob);
            var kJob = new QueryKNearestJob(knn, samplePoint, results);
            kJob.Schedule().Complete();

            resultList = new int[querySize];
            for (int i = 0; i < querySize || i < results.Length; i++)
            {
                resultList[i] = results[i];
            }

            results.Dispose();
        }

        public static int KnnQueryNonAlloc(this KnnContainer knn, Vector3 samplePoint, ref int[] resultArr,
            int querySize)
        {
            if (knn.Points.Length == 0)
            {
                return 0;
            }

            var results = new NativeArray<int>(querySize, Allocator.Persistent);
            var kJob = new QueryKNearestJob(knn, samplePoint, results);
            kJob.Schedule().Complete();
            int hits = 0;
            for (int i = 0; i < querySize || i < results.Length; i++)
            {
                resultArr[i] = results[i];
                hits++;
            }

            results.Dispose();
            return hits;
        }

        public static int KnnQueryNonAlloc(int id, Vector3 samplePoint, ref int[] resultArr, int querySize)
        {
            var knn = knnData[id];
            if (!knn.initialized)
            {
                return 0;
            }

            var container = knn.container;
            var results = new NativeArray<int>(querySize, Allocator.Persistent);
            var kJob = new QueryKNearestJob(container, samplePoint, results);
            kJob.Schedule().Complete();
            int hits = 0;
            for (int i = 0; i < querySize || i < results.Length; i++)
            {
                resultArr[i] = results[i];
                hits++;
            }

            results.Dispose();
            return hits;
        }
    }
}