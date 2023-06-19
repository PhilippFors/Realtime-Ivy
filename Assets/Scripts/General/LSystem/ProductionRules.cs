using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace General.LSystem
{
    [CreateAssetMenu(fileName = "rule config", menuName = "LSystem/Rule Config")]
    public class ProductionRules : SerializedScriptableObject
    {
        public Dictionary<BaseRule, BaseRule[]> rules = new();
    }
}