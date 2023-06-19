using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Player.Input.Data;
using UnityEngine;
using VineGeneration.Util;

namespace VineGeneration.Core
{
	/// <summary>
	/// Entry point for the climbing plant growth. Holds a list of all generators and updates them.
	/// </summary>
	[DefaultExecutionOrder(-15)]
	public class GeneratorSimulatorSystem : MonoBehaviour
	{
		public static GeneratorSimulatorSystem current;

		[SerializeField] private InputConfig controls;
		[SerializeField] private int maxIterationsPerFrame = 20;
		[SerializeField] private bool generate = true;
		[SerializeField] private float delay = 0.1f;
		[SerializeField] private bool debug;

		private List<GeneratorContainer> containers = new();
		private int currentBatch;
		private bool checkEndedBranches;
		private bool meshBuilderRunning;

		private void Awake()
		{
			current = this;
			GrowUtil.climbNodes.Clear();
			KnnHelper.Init();
		}

		private void OnDestroy()
		{
			generate = false;
			containers.Clear();
		}

		private void Start()
		{
			Generate().Forget();
		}

		public void RegisterGenerator(GeneratorContainer generator)
		{
			containers.Add(generator);
		}

		public void UnregisterGenerator(GeneratorContainer generator)
		{
			containers.Remove(generator);
		}

		private void Update()
		{
			if (!debug) {
				return;
			}

			if (controls.Get(Inputs.Fire2).IsPressed) {
				generate = false;
			}
		}

		private async UniTask Generate()
		{
			var d = delay * 1000;
			int milliseconds = (int)d;
			while (generate) {
				GeneratorContainer.genCount = 0;
				var time = Time.realtimeSinceStartup;
				for (int i = 0; i < containers.Count; i++) {
					var gen = containers[i];
					await gen.Grow(maxIterationsPerFrame);
				}
				var t = Time.realtimeSinceStartup - time;
				var rest = milliseconds - (int)(t * 1000);
				if (rest < 0) {
					rest = 0;
				}
				await UniTask.Delay(rest);
			}
		}
	}
}
