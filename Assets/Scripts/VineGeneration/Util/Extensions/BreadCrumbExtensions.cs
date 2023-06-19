using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace VineGeneration.Util.Extensions
{
    public static class BreadCrumbExtensions
    {
        public static NativeArray<float3> FillBreadCrumbs(this List<BreadCrumb> breadCrumbs)
        {
            var points = new NativeArray<float3>(breadCrumbs.Count, Allocator.TempJob);
            for (int i = 0; i < breadCrumbs.Count; i++)
            {
                points[i] = breadCrumbs[i].point;
            }

            return points;
        }
        
        public static Vector3[] GetBreadCrumbArray(this List<BreadCrumb> breadCrumbs)
        {
            var points = new Vector3[breadCrumbs.Count];
            for (int i = 0; i < breadCrumbs.Count; i++)
            {
                points[i] = breadCrumbs[i].point;
            }

            return points;
        }
    }
}