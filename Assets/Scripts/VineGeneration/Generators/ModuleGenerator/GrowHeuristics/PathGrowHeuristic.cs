using System.Collections.Generic;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.GrowHeuristics
{
    [CreateAssetMenu(menuName = "VineGeneration/Grow Heuristics/Path Heuristic")]
    public class PathGrowHeuristic : BaseGrowHeuristic
    {
        public int directionSampleAmount = 3;
        public int turnCheckerSampleAmount = 5;
        public float maxTurnRate = 40;

        private Vector3[] directionSamples;
        private Vector3[] turnCheckerSamples;
        private int[] resultArr = new int[10];
        private int[] resultArr2 = new int[10];
        private List<BreadCrumb> breadCrumbs;

        public override bool Execute(ref BranchQueryInfo info)
        {
            breadCrumbs = BreadCrumbGenerator.BreadCrumbs;
            if (breadCrumbs.Count == 0)
            {
                return false;
            }

            if (directionSamples == null || directionSamples.Length != directionSampleAmount)
            {
                directionSamples = new Vector3[directionSampleAmount];
            }

            if (turnCheckerSamples == null || turnCheckerSamples.Length != turnCheckerSampleAmount)
            {
                turnCheckerSamples = new Vector3[turnCheckerSampleAmount];
            }

            var startNormal = info.normal;
            var startPos = info.point;
            var stepLength = info.stepLength;
            var startDirection = info.direction;
            RayDirectionGenerator.GetForwardDirectionsNonAlloc(ref directionSamples, startDirection, startNormal, maxTurnRate);

#if UNITY_EDITOR
            // DebugUtils.DrawDebugLines(startPos, stepLength, directionSamples, Color.green, Color.red, 20);
#endif
            
            foreach (var checkDir in directionSamples)
            {
                var checkPoint = startPos + checkDir * stepLength;
                if (checkDir.magnitude > 0.01f && EnvironmentHelper.CheckPathForward(info, ref turnCheckerSamples, ref resultArr2, checkPoint, checkDir, stepLength * 3, true, maxTurnRate * 0.5f))
                {
                    var hits = KnnHelper.KnnQueryNonAlloc(KnnIds.breadCrumbs, checkPoint, ref resultArr, 1);
                    if (hits == 0)
                    {
                        continue;
                    }
                    breadCrumbs.InBreadCrumbDistance(resultArr[0], startPos, out var dist);

                    var resultCrumb = breadCrumbs[resultArr[0]];
                    var dot = Vector3.Dot(resultCrumb.direction, checkDir);
                    if (dot < 0.6f && dist > GlobalVineSettings.current.breadCrumbRadius * 0.7f)
                    {
                        continue;
                    }

                    info.direction = checkDir;
                    return true;
                }
            }

            return false;
        }
    }
}