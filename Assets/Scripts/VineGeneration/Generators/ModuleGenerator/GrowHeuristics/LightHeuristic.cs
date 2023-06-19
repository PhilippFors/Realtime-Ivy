using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.GrowHeuristics
{
    [CreateAssetMenu(fileName = "LightHeuristic", menuName = "VineGeneration/Grow Heuristics/LightHeuristic")]
    public class LightHeuristic : BaseGrowHeuristic
    {
        public int directionCheckers;
        public float maxTurnRate;
        public float checkAheadDistance = 0.3f;
        
        private Vector3[] directionSamples;

        public override void Initialize(ModularGenerator gen, GeneratorContainer c)
        {
            base.Initialize(gen, c);
            directionSamples = new Vector3[directionCheckers];
        }

        public override bool Execute(ref BranchQueryInfo info)
        {
            var point = info.point;
            
            if (LightIntensityHelper.IsInLight(info.ivyParams, point + info.direction * info.stepLength)) {
                return true;
            }
            
            RayDirectionGenerator.GetForwardDirectionsNonAlloc(ref directionSamples, info.direction, -info.normal, maxTurnRate);
            foreach (var dir in directionSamples)
            {
                var checkPoint = point + dir * checkAheadDistance;
                if (LightIntensityHelper.IsInLight(info.ivyParams, checkPoint))
                {
                    info.direction = dir;
                    return true;
                }
            }
            return false;
        }
    }
}