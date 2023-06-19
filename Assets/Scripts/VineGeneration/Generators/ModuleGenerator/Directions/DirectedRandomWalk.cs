using UnityEngine;
using VineGeneration.Core;

namespace VineGeneration.Generators.ModuleGenerator.Directions
{
    [CreateAssetMenu(menuName = "VineGeneration/Direction Generators/Directed random walk")]
    public class DirectedRandomWalk : DirectionCalculator
    {
        public float maximumAngle;
        public int sampleCount;
        
        public override bool Execute(ref BranchQueryInfo info)
        {
            return false;
        }
    }
}