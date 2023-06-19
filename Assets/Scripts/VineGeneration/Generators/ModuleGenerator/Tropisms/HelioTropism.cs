using System.Collections.Generic;
using Lights;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;

namespace VineGeneration.Generators.ModuleGenerator.Tropisms
{
    /// <summary>
    /// Rotates direction towards nearest light source.
    /// </summary>
    [CreateAssetMenu(fileName = "HelioTropism", menuName = "VineGeneration/Tropisms/HelioTropism")]
    public class HelioTropism : BaseTropism
    {
        private List<LightInfo> outputLights = new();

        public override bool Execute(ref BranchQueryInfo info)
        {
            outputLights.Clear();
            var lightCollection = LightCollection.current.Lights;
            if (lightCollection.Count == 0)
            {
                return false;
            }

            if (lightCollection.IsPointLit(outputLights, info.point, info.ivyParams.minLightIntensity, GlobalVineSettings.current.generalMask))
            {
                float maxIntensity = float.MinValue;
                int index = -1;
                for (int i = 0; i < outputLights.Count; i++)
                {
                    var lightInfo = outputLights[i];
                    if (lightInfo.intensity > maxIntensity)
                    {
                        maxIntensity = lightInfo.intensity;
                        index = i;
                    }
                }

                if (index == -1)
                {
                    return false;
                }

                var dir = outputLights[index].light.transform.position - info.point;
                var newDir = info.direction.RotateTowards(dir, weight);
                info.direction = Vector3.ProjectOnPlane(newDir.normalized, info.normal).normalized;
            }
            
            return true;
        }
    }
}