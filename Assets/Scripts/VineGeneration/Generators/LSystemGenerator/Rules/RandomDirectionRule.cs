using General.LSystem;
using UnityEngine;

namespace VineGeneration.Generators.LSystemGenerator.Rules
{
    [CreateAssetMenu(fileName = "RandomDirectionRule", menuName = "LSystem/Rules/RandomDirectionRule")]
    public class RandomDirectionRule : BaseRule
    {
        public BaseRule left;
        public BaseRule right;
        
        public override bool ApplyRule(LSystemData data, LSystemInterpreter interpreter, LSystemGenerator generator)
        {
            return true;
        }

        public override BaseRule[] Parse(LSystemGenerator generator)
        {
            var random = generator.Container.Rng.Value;
            if (random > 0.5f)
            {
                return new[] { left };
            }

            return new[] { right };
        }
    }
}