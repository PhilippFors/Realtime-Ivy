using System.Collections.Generic;
using UnityEngine;
using VineGeneration.Core;

namespace VineGeneration.Generators.ModuleGenerator
{
    public abstract class BaseBranchComputeAction : ScriptableObject
    {
        public abstract void Execute(List<Branch> branches);
    }
}