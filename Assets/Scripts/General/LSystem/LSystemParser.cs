using System.Collections.Generic;
using VineGeneration.Generators.LSystemGenerator;

namespace General.LSystem
{
    public class LSystemParser
    {
        //Recursive
        public void GenerateRecursive(LSystemGenerator generator, LSystemData data)
        {
            data.result = IterateRecursive(generator, data, data.root, 0);
        }
        
        private List<BaseRule> IterateRecursive(LSystemGenerator generator, LSystemData data, List<BaseRule> rules, int iterationDepth)
        {
            if (iterationDepth >= data.maxIterationDepth)
            {
                return rules;
            }

            var newRules = new List<BaseRule>();
            foreach (var symbol in rules)
            {
                ParseRecursive(generator, data, newRules, symbol, iterationDepth);
            }
            return newRules;
        }

        private void ParseRecursive(LSystemGenerator generator, LSystemData data, List<BaseRule> newRules, BaseRule current, int iterationDepth)
        {
            foreach (var (rule, results) in data.ruleConfig.rules)
            {
                if (results.Length > 0 && rule.id.Equals(current.id))
                {
                    List<BaseRule> parsed = new();
                    foreach (var res in results)
                    {
                        parsed.AddRange(res.Parse(generator));
                    }
                    newRules.AddRange(IterateRecursive(generator, data, parsed, iterationDepth + 1));
                }
                else
                {
                    newRules.Add(rule);
                }
            }
        }
        
        //Iterative
        public void GenerateIterative(LSystemData data)
        {
            foreach (var r in data.root)
            {
                
            }
        }
        
        private List<BaseRule> ParseIterative(LSystemData data, List<BaseRule> rules)
        {
            var newRules = new List<BaseRule>();
            foreach (var symbol in rules)
            {
                foreach (var (rule, results) in data.ruleConfig.rules)
                {
                    if (results.Length == 0)
                    {
                        continue;
                    }
                    
                    if (rule.id.Equals(symbol.id))
                    {
                        newRules.AddRange(results);
                    }
                }
            }
            return newRules;
        }
        

        private LSymbol[] GetSymbols(List<BaseRule> rules)
        {
            var symbols = new LSymbol[rules.Count];
            for (int i = 0; i < rules.Count; i++)
            {
                symbols[i] = rules[i].id;
            }

            return symbols;
        }

        private string ParseToString(LSymbol[] symbols)
        {
            var str = "";
            foreach (var s in symbols)
            {
                str += s.identifier + " ";
            }

            return str;
        }
    }
}