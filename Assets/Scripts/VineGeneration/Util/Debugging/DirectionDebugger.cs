using UnityEngine;

namespace VineGeneration.Util.Debugging
{
    public class DirectionDebugger : MonoBehaviour
    {
        public LayerMask hitMask;
        public int iterations = 1000;

        private void Update()
        {
            // var startPosition = transform.position;
            // Profiler.BeginSample("convert");
            // float3 startPosition3 = startPosition;
            // float3 direction = transform.forward;
            // Profiler.EndSample();
            // Profiler.BeginSample("math");
            // for (int i = 0; i < iterations; i++)
            // {
            //     var result = math.distance(startPosition, startPosition + transform.forward * 10);
            // }
            // Profiler.EndSample();
            //
            // Profiler.BeginSample("vector");
            // for (int i = 0; i < iterations; i++)
            // {
            //     var result = Vector3.Distance(startPosition, startPosition + transform.forward * 10);
            // }
            // Profiler.EndSample();
        }

        private void OnDrawGizmos()
        {
            // if (Physics.Raycast(transform.position, -transform.up, out var hit, 1, hitMask))
            // {
            //     var right = Quaternion.AngleAxis(90, transform.right) * transform.up;
            //     var angledDir = Quaternion.AngleAxis(1.5f, right) * transform.up;
            //     var newMainGrowDir = Vector3.ProjectOnPlane(angledDir, hit.normal).normalized;
            //     Gizmos.color = Color.yellow;
            //     Gizmos.DrawLine(transform.position, transform.up + transform.position);
            //     Gizmos.color = Color.red;
            //     Gizmos.DrawLine(hit.point, newMainGrowDir + hit.point);
            // }
            if (Physics.Raycast(transform.position, transform.forward, out var hit, 1, hitMask))
            {
                var direction = transform.forward;
                var startPos = transform.position;
                Ray ray = new Ray(startPos, direction);
                if (Physics.Raycast(ray, out var hit2, 1, hitMask))
                {
                    Gizmos.DrawLine(startPos, startPos + transform.forward);
                    var project = Vector3.ProjectOnPlane(transform.forward, hit2.normal);
                    Gizmos.DrawLine(hit2.point, hit2.point + project.normalized);
                } 
                // var right = Quaternion.AngleAxis(90, oldNormal) * direction;
                // var angledDir = Quaternion.AngleAxis(-1.5f, right) * direction;
                // // newMainGrowDir = Vector3.ProjectOnPlane(angledDir, oldNormal);
                // // var potentialPoint = Vector3.ProjectOnPlane(angledDir, oldNormal).normalized *
                // //     0.5f + transform.position + oldNormal * 0.2f;
                // var potentialPoint = Vector3.ProjectOnPlane(direction, oldNormal) *
                //     0.5f + transform.position + oldNormal * 0.2f;
                // // var potentialPoint = transform.position + direction.normalized * 0.2f + oldNormal * 0.05f;
                // Gizmos.color = Color.yellow;
                // Gizmos.DrawWireSphere(potentialPoint, 0.1f);
                // var pot2 = potentialPoint + direction * 0.5f + -oldNormal * 0.2f * 1.15f;
                // Gizmos.DrawWireSphere(pot2, 0.1f);
                // var ray = new Ray(pot2, -direction);
                // if (Physics.Raycast(ray, out var hit2, 0.2f * 1.15f, hitMask))
                // {
                //     Gizmos.color = Color.red;
                //     Gizmos.DrawWireSphere(hit2.point, 0.1f);
                // }
            }
        }
    }
}