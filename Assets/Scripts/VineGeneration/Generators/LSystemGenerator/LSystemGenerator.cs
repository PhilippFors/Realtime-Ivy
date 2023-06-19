using System.Collections.Generic;
using General.LSystem;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Parameters;

namespace VineGeneration.Generators.LSystemGenerator
{
    public class LSystemGenerator : BaseGenerator
    {
        public GrowParameters ivyParams;
        public LSystemData template;

        private Queue<LSystemData> toParse = new();
        public List<LSystemData> toInterpret = new();
        private LSystemParser parser;
        private LSystemInterpreter interpreter;

        public override void Initialize(GeneratorContainer gen)
        {
            base.Initialize(gen);
            parser = new LSystemParser();
            interpreter = new LSystemInterpreter();
        }

        public override int Count => toInterpret.Count;
        
        public override void PreStep()
        {
            while (toParse.Count > 0)
            {
                var data = toParse.Dequeue();
                parser.GenerateRecursive(this, data);
                toInterpret.Add(data);
            }
        }

        public override bool Step(int i)
        {
            interpreter.Interpret(toInterpret[i], this);
            return true;
        }

        public override void SetBranchParams(Branch b)
        {
            b.thickness = ivyParams.branchThickness;
            b.currentHeight = ivyParams.maxDistanceToSurface;
            var rend = b.branchMesh[^1].objRef.GetComponent<Renderer>();
            if (rend)
            {
               rend.sharedMaterial = ivyParams.meshParameters.branchMaterial;
            }
        }

        public override void AddBranch(Branch b)
        {
            SetBranchParams(b);
            var data = Instantiate(template);
            data.branch = b;
            Branches.Add(b);
            toParse.Enqueue(data);
        }

        // private void OnDrawGizmos()
        // {
        //     if (!Application.isPlaying)
        //     {
        //         return;
        //     }
        //     if (Branches.Count > 0)
        //     {
        //         Gizmos.color = Color.magenta;
        //         foreach (var b in Branches)
        //         {
        //             foreach (var n in b.nodes)
        //             {
        //                 Gizmos.DrawWireSphere(n.position, 0.05f);   
        //             }
        //         }
        //     }
        // }
    }
}