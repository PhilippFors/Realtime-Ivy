using UnityEngine;

namespace General.MeshBuilding
{
    public class MeshDebugUtil
    {
        public static bool logger = true;

        public static void Sphere(Vector3 position)
        {
            if (logger)
                Sphere(position, Color.blue);
        }

        public static void Sphere(Vector3 position, Color x)
        {
            if (logger) {
                var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = position;
                sphere.transform.localScale = new Vector3(.05f, .05f, .05f);
                sphere.GetComponent<Renderer>().material.color = x;
            }
        }

        public static void DrawRay(Vector3 start, Vector3 dir, Color x)
        {
            if (logger) {
                Debug.DrawRay(start, dir, x, float.PositiveInfinity);
            }
        }

        public static void DrawRay(Vector3 start, Vector3 dir)
        {
            if (logger) {
                DrawRay(start, dir, Color.blue);
            }
        }
    }
}