using Sirenix.OdinInspector;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Generators.LSystemGenerator;
using VineGeneration.Util;

namespace General.LSystem.Debugging
{
    public class LSystemDebugger : MonoBehaviour
    {
        public LSystemGenerator generator;
        
        // private void Start()
        // {
        //     generator.Initialize();
        // }

        [Button]
        private void AddBranch()
        {
            // generator.AddBranch(CreateBranch());
        }

        [Button]
        private void Step()
        {
            generator.PreStep();
            for (int i = 0; i < generator.Branches.Count; i++)
            {
                generator.Step(i);
            }
        }

        // private Branch CreateBranch()
        // {
        //     var newBr = GrowUtil.CreateBranch(generator.Container.Rng, transform.position, Vector3.up, null);
        //     newBr.mainGrowDirection = transform.forward;
        //     newBr.meshData.meshStatic = true;
        //     return newBr;
        // }
    }
}