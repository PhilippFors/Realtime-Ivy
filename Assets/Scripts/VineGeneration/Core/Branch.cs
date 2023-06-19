using System.Collections.Generic;
using General.MeshBuilding;
using UnityEngine;

namespace VineGeneration.Core
{
    public class Branch
    {
        public BranchNode this[int i] => nodes[i];
        public int Count => nodes.Count;
        public BranchNode LastNode => nodes.Count > 0 ? nodes[^1] : null;
        
        public readonly List<Branch> children = new();
        public readonly List<BranchNode> nodes = new();
        
        public List<MeshCollection> branchMesh;
        public MeshCollection climbNodeMesh;
        public List<MeshCollection> leafMesh;
        
        public Vector3 mainGrowDirection;
        public VineSeed ogSeed;
        public float thickness;
        public float currentHeight;
        public float totalLength;
        public float lastBranchedOffLength;
        public int branchSense;
        public int branchDepth;
        public int branchNumber;
        public bool initialized;
        public bool hasChanged;
        public bool alive;
        public bool hasParent;

        public Branch()
        {
            alive = true;
            branchMesh = new List<MeshCollection>();
            leafMesh = new List<MeshCollection>();
            climbNodeMesh = new MeshCollection();
        }

        public void AddNode(BranchNode node)
        {
            node.index = Count;
            if (Count > 0)
            {
                totalLength += Vector3.Distance(node.position, LastNode.position);
            }

            nodes.Add(node);
        }
        
        public void RemoveNode(BranchNode node) => nodes.Remove(node);
    }
}