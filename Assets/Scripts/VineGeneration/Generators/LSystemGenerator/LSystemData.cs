using System.Collections.Generic;
using General.LSystem;
using UnityEngine;
using VineGeneration.Core;

namespace VineGeneration.Generators.LSystemGenerator
{
    [CreateAssetMenu(fileName = "LSystemData", menuName = "LSystem/LSystemData")]
    public class LSystemData : ScriptableObject
    {
        public List<BaseRule> root = new();
        public List<BaseRule> result = new();
        public ProductionRules ruleConfig;
        public Branch branch;
        public int maxIterationDepth = 5;
        public int lastInterpreted;
        public bool finished;
    }
}