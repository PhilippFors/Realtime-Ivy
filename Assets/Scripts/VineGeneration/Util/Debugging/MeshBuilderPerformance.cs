using General.MeshBuilding;
using UnityEngine;
using VineGeneration.Core;

namespace VineGeneration.Util.Debugging
{
    public class MeshBuilderPerformance : MonoBehaviour
    {
        public Branch branch;
        public MeshFilter filter;

        private void Start()
        {
            // filter.GetComponent<MeshFilter>();
            // branch = new Branch();
            // {
            //     branch.thickness = 0.05f;
            // }
            // branch.meshData.meshFilter = filter;
            // for (int i = 0; i < 200; i++)
            // {
            //     branch.AddNode(new VineNode()
            //     {
            //         growDirection = Vector3.forward,
            //         position = Vector3.forward * i * 0.06f,
            //         surfaceNormal = -Vector3.up
            //     });
            // }
            //
            // BMeshBuilder.BuildMesh(branch);
        }

        private void Update()
        {
            // Normal MeshBuild
            var newNode = new BranchNode()
            {
                growDirection = Vector3.forward,
                position = branch[^1].position + Vector3.forward * 0.06f,
                surfaceNormal = -Vector3.up
            };

            branch.AddNode(newNode);

            // Profiler.BeginSample("MeshBuilderPerformance - Bmesh MeshBuild");
            // BMeshBuilder.AddMeshNode(branch, newNode);
            // Profiler.EndSample();
            //
            // Profiler.BeginSample("MeshBuilderPerformance - Normal MeshBuild");
            // MeshBuilder.BuildMesh(branch);
            // Profiler.EndSample();
        }
    }
}