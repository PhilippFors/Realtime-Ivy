using General.LSystem;
using UnityEngine;

namespace VineGeneration.Generators.LSystemGenerator.Rules
{
    
[CreateAssetMenu(fileName = "TurnLeftRule", menuName = "LSystem/Rules/TurnLeftRule")]
    public class TurnLeftRule : BaseRule
    {
        public float minAngle;
        public float maxAngle;

        public BaseRule rightRule;
        
        public override bool ApplyRule(LSystemData data, LSystemInterpreter interpreter, LSystemGenerator generator)
        {
            var branch = data.branch;
            var oldDir = branch.mainGrowDirection;
            var newDir = Quaternion.AngleAxis(-generator.Container.Rng.Range(minAngle, maxAngle), -branch.LastNode.surfaceNormal) * oldDir;
            branch.mainGrowDirection = newDir;
            return true;
        }
        
        public override BaseRule[] Parse(LSystemGenerator generator)
        {
            var random = generator.Container.Rng.Value;
            if (random > 0.55f)
            {
                return new[] { rightRule };
            }

            return new BaseRule[] { this };
        }
    }
}