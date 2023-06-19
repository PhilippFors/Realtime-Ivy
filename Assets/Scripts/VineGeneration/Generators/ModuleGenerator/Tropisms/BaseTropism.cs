using UnityEngine;

namespace VineGeneration.Generators.ModuleGenerator.Tropisms
{
    public abstract class BaseTropism : BaseGrowAction
    {
        [SerializeField] protected float weight;
    }
}