using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Util;
using VineGeneration.Util;
using VineGeneration.Util.Extensions;
using Random = UnityEngine.Random;

namespace VineGeneration.Core
{
	public enum GeneratorType
	{
		Main,
		Sub
	}

	/// <summary>
	/// Container for main and sub generators, combined meshes and reference gameobjects.
	/// </summary>
	[DefaultExecutionOrder(-5)]
	public class GeneratorContainer : MonoBehaviour
	{
		public static int genCount;
		
		public RandomState Rng { get; private set; }

		[SerializeField] private bool runIndependent;
		[SerializeField, ShowIf("runIndependent")]
		private bool generate;
		[SerializeField, ShowIf("runIndependent")]
		private float growDelay;
		[SerializeField, ShowIf("runIndependent")]
		private int maxIterationsPerFrame = 60;
		[SerializeField] private bool useRandomSeed = true;
		[SerializeField] private int currentSeed = 1234;

		private readonly List<BaseGenerator> generatorList = new();
		private BaseGenerator mainGenerator;
		private BaseGenerator subGenerator;

		private void OnEnable()
		{
			if (runIndependent) {
				return;
			}
			GeneratorSimulatorSystem.current.RegisterGenerator(this);
		}

		private void OnDisable()
		{
			if (runIndependent) {
				return;
			}
			GeneratorSimulatorSystem.current.UnregisterGenerator(this);
		}

		public void StartGeneration()
		{
			foreach (var gen in generatorList) {
				gen.StartGen();
			}
		}

		public void StopGeneration()
		{
			foreach (var gen in generatorList) {
				gen.StopGen();
			}
		}

		private void Awake()
		{
			if (useRandomSeed) {
				currentSeed = Random.Range(0, int.MaxValue);
			}

			Rng = new RandomState(currentSeed);

			var generators = GetComponents<BaseGenerator>();
			foreach (var gen in generators) {
				if (gen.type == GeneratorType.Main) {
					mainGenerator = gen;
				}
				else {
					subGenerator = gen;
				}
			}

			if (!mainGenerator) {
				Debug.LogError("No main generator assigned!");
				return;
			}

			generatorList.Add(mainGenerator);

			if (subGenerator) {
				generatorList.Add(subGenerator);
			}

			foreach (var gens in generatorList) {
				gens.Initialize(this);
			}
		}

		private void Start()
		{
			if (generatorList.Count == 0) {
				Debug.LogError("No generators assigned!");
				return;
			}
			if (runIndependent) {
				Generate().Forget();
			}
		}

		private void OnDestroy()
		{
			generate = false;
			StopGeneration();
		}

		public T GetGenerator<T>(GeneratorType type) where T : BaseGenerator
		{
			if (type == GeneratorType.Main) {
				return mainGenerator as T;
			}

			return subGenerator as T;
		}

		public void AddBranch(GeneratorType type, Branch b)
		{
			if (type == GeneratorType.Main) {
				mainGenerator.AddBranch(b);
			}
			else {
				subGenerator.AddBranch(b);
			}
		}

		private async UniTask Generate()
		{
			var d = growDelay * 1000;
			int milliseconds = (int) d;
			while (generate) {
				Grow(maxIterationsPerFrame).Forget();
				await UniTask.Delay(milliseconds);
			}
		}

		public async UniTask Grow(int maxIterations)
		{
			KnnHelper.FillContainer(KnnIds.breadCrumbs, BreadCrumbGenerator.BreadCrumbs.GetBreadCrumbArray());
			foreach (var generator in generatorList) {
				generator.PreStep();
				for (int i = 0; i < generator.Count; i++) {
					if (generator.Step(i)) {
						genCount++;
						if (genCount >= maxIterations) {
							generator.PostStep();
							await UniTask.Yield();
							generator.PreStep();
							genCount = 0;
						}
					}
				}
				generator.PostStep();
			}
			KnnHelper.DisposeContainer(KnnIds.breadCrumbs);
		}

		private void OnDrawGizmos()
		{
			int count = 0;
			if (generatorList == null) {
				return;
			}
			Gizmos.color = Color.cyan;
			foreach (var gen in generatorList) {
				foreach (var branch in gen.Branches) {
					foreach (var node in branch.nodes) {
						count++;
						Gizmos.DrawWireSphere(node.position, 0.02f);
						if (count > 15) {
							return;
						}
					}
				}
			}
		}
	}
}
