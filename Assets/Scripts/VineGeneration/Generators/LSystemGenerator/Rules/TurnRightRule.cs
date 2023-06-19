using General.LSystem;
using UnityEngine;

namespace VineGeneration.Generators.LSystemGenerator.Rules
{
    [CreateAssetMenu(fileName = "TurnRightRule", menuName = "LSystem/Rules/TurnRightRule")]
    public class TurnRightRule : BaseRule
    {
        public float minAngle;
        public float maxAngle;

        public BaseRule leftRule;

        public override bool ApplyRule(LSystemData data, LSystemInterpreter interpreter, LSystemGenerator generator)
        {
            var branch = data.branch;
            var oldDir = branch.mainGrowDirection;
            var newDir = Quaternion.AngleAxis(generator.Container.Rng.Range(minAngle, maxAngle), -branch.LastNode.surfaceNormal) * oldDir;
            branch.mainGrowDirection = newDir;
            return true;
        }

        public override BaseRule[] Parse(LSystemGenerator generator)
        {
            var random = generator.Container.Rng.Value;
            if (random > 0.55f)
            {
                return new[] { leftRule };
            }

            return new BaseRule[] { this };
        }
    }
}