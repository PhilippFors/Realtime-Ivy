using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.Actions
{
    [CreateAssetMenu(menuName = "VineGeneration/Actions/Init/Main Branch")]
    public class MainBranchInitialization : BaseGrowAction
    {
        public override bool Execute(ref BranchQueryInfo info)
        {
            var breadCrumbs = BreadCrumbGenerator.BreadCrumbs;

            if (breadCrumbs.Count == 0)
            {
                return false;
            }

            var querySize = 1;
            var resultArr = new int[5];
            var startPos = info.point;
            KnnHelper.KnnQueryNonAlloc(KnnIds.breadCrumbs, startPos, ref resultArr, querySize);
            if (resultArr.Length == 0)
            {
                return false;
            }

            if (breadCrumbs.InBreadCrumbDistance(resultArr[0], startPos))
            {
                var crumb = breadCrumbs[resultArr[0]];
                var dirToCrumb = (crumb.point - startPos).normalized;
                var dirArr = new Vector3[26];
                var turnCheckers = new Vector3[3];
                RayDirectionGenerator.GetForwardDirectionsNonAlloc(ref dirArr, dirToCrumb, crumb.normal, 360);
                DebugUtils.DrawDebugLines(startPos, 0.8f, dirArr, Color.red, Color.blue, 15);
                var radius = GlobalVineSettings.current.breadCrumbRadius;
                foreach (var checkDir in dirArr)
                {
                    if (!EnvironmentHelper.CheckPathForward(info, ref turnCheckers, ref resultArr, startPos, checkDir, radius * 0.8f, true, 40))
                    {
                        continue;
                    }

                    // var randomDir = Quaternion.AngleAxis(Random.Range(-10, 10), crumb.normal) * checkDir;
                    var newPoint = startPos + checkDir * info.ivyParams.stepSize;
                    info.direction = checkDir;
                    info.branch.mainGrowDirection = checkDir;
                    info.point = newPoint;
                    GrowUtil.AddNode(info.branch, newPoint, crumb.normal, false, true);
                    return true;
                }
            }

            return false;
        }
    }
}