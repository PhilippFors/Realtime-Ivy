using Sirenix.OdinInspector;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Generators.ModuleGenerator.Tropisms;

namespace VineGeneration.Generators.ModuleGenerator
{
	public abstract class DirectionCalculator : BaseGrowAction
	{
		public bool useTropisms;

		[SerializeField, InlineEditor] private BaseTropism[] tropisms;

		protected BaseTropism[] tropismInstances;

		public override void Initialize(ModularGenerator gen, GeneratorContainer c)
		{
			base.Initialize(gen, c);
			if (tropisms == null) {
				return;
			}
			tropismInstances = new BaseTropism[tropisms.Length];
			for (int i = 0; i < tropisms.Length; i++) {
				tropismInstances[i] = Instantiate(tropisms[i]);
				tropismInstances[i].Initialize(gen, c);
			}
		}
	}
}
