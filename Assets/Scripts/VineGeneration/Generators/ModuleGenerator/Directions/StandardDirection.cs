using UnityEngine;
using VineGeneration.Core;

namespace VineGeneration.Generators.ModuleGenerator.Directions
{
    [CreateAssetMenu(menuName = "VineGeneration/Direction Generators/Standard")]
    public class StandardDirection : DirectionCalculator
    {
        public override bool Execute(ref BranchQueryInfo info)
        {
            var branch = info.branch;
            var ivyParams = info.ivyParams;
            var stepLength = info.stepLength;
            var startNormal = -branch.LastNode.surfaceNormal;
            
            var param1 = Mathf.Sin(branch.branchSense *
                                   branch.totalLength *
                                   ivyParams.directionFrequency *
                                   (1 + container.Rng.Range(-ivyParams.dirRandomWeight, ivyParams.dirRandomWeight)));


            var param2 = Quaternion.AngleAxis(param1 *
                                              ivyParams.directionAmplitude *
                                              stepLength *
                                              10f *
                                              Mathf.Max(ivyParams.dirRandomWeight, 1f), startNormal);
            var main = Vector3.ProjectOnPlane(param2 * branch.mainGrowDirection, startNormal).normalized;
            info.direction = main;
            
            if (useTropisms)
            {
                foreach (var t in tropismInstances)
                {
                    t.Execute(ref info);
                }
            }
            
            return true;
        }
    }
}