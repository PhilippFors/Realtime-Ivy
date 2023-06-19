using System.Collections.Generic;
using UnityEngine;

namespace VineGeneration.Core
{
	public abstract class BaseGenerator : MonoBehaviour
	{
		public Branch this[int i] => Branches[i];
		public abstract int Count { get; }

		public GeneratorType type;
		public GeneratorContainer Container { get; private set; }
		public List<Branch> Branches { get; private set; }

		public virtual void Initialize(GeneratorContainer gen)
		{
			Container = gen;
			Branches = new List<Branch>();
		}
		public virtual void StartGen() { }

		public virtual void StopGen() { }

		public virtual void PreStep() { }

		public virtual void PostStep() { }

		public abstract bool Step(int id);
		
		public virtual void SetBranchParams(Branch branch) { }

		public abstract void AddBranch(Branch b);
	}
}
