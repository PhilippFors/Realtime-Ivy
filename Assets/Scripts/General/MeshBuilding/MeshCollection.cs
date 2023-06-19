using UnityEngine;

namespace General.MeshBuilding
{
    [System.Serializable]
    public class MeshCollection
    {
        public BMesh bMesh;
        public GameObject objRef;
        public MeshRenderer meshRenderer;
        public MeshFilter meshFilter;
        public bool isStatic;
        public bool isCombined;
    }
}