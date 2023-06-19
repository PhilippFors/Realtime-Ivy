using UnityEngine;
using VineGeneration.Generators.LSystemGenerator;

namespace General.LSystem
{
    public abstract class BaseRule : ScriptableObject
    {
        public LSymbol id;
        public string description;
        
        /// <summary>
        /// Return false if the interpreter should move to the next iteration
        /// </summary>
        /// <returns></returns>
        public abstract bool ApplyRule(LSystemData data, LSystemInterpreter interpreter, LSystemGenerator generator);
        
        /// <summary>
        /// During parse section, can be used for randomization and replace the rule with multiple rules
        /// </summary>
        /// <returns></returns>
        public virtual BaseRule[] Parse(LSystemGenerator generator) => new[] {this};
    }
}