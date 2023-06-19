using ObjectIdentification;
using UnityEngine;
using VineGeneration.Parameters;

namespace VineGeneration.Util
{
    public static class CorruptionHelper
    {
        public static bool RaycastCorruption(Vector3 point, Vector3 direction, float stepLength,
            GrowParameters ivyParams)
        {
            if (!ivyParams.hatesCorruption)
            {
                return false;
            }

            if (Physics.Raycast(point, direction, out var hit, stepLength * 3,
                    GlobalVineSettings.current.corruptionMask))
            {
                var id = hit.transform.GetComponentInChildren<ObjectIdentifier>();
                if (id)
                {
                    return id.identity == ObjectIdentities.Corruption;
                }
            }

            return false;
        }
    }
}