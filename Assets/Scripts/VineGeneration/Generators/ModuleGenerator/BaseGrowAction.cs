using UnityEngine;
using VineGeneration.Core;

namespace VineGeneration.Generators.ModuleGenerator
{
    public abstract class BaseGrowAction : ScriptableObject
    {
        protected ModularGenerator generator;
        protected GeneratorContainer container;

        public virtual void Initialize(ModularGenerator gen, GeneratorContainer c)
        {
            generator = gen;
            container = c;
        }
        
        public abstract bool Execute(ref BranchQueryInfo info);

        public virtual void ResetModule()
        {
            
        }
    }
}