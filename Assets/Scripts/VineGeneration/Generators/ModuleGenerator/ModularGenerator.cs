using System;
using System.Collections.Generic;
using General.MeshBuilding;
using Sirenix.OdinInspector;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Parameters;

namespace VineGeneration.Generators.ModuleGenerator
{
	public sealed class ModularGenerator : BaseGenerator
	{
		public override int Count => Branches.Count;
		
		public List<MeshCollection> combinedBranchMesh;
		public List<MeshCollection> combinedLeafMesh;
		public List<MeshCollection> combinedClimbNodeMesh;
		[HideInInspector] public VineSeed seed;

		[InlineEditor] public GrowParameters ivyParams;
		[SerializeField] private DirectionCalculator directionCalculator;
		[SerializeField, Tooltip("Can be left empty.")]
		private BaseGrowAction initBranchAction;
		[SerializeField] private BaseGrowAction nodeSpawn;
		[SerializeField] private BaseAsyncAction[] asyncActions;
		[SerializeField] private BaseGrowConstraint[] growConstraints;
		[SerializeField] private BaseGrowHeuristic[] growHeuristics;
		[SerializeField] private BaseGrowAction[] onNodeSpawnAction;

		private void OnDisable()
		{
			foreach (var asyncAction in asyncActions) {
				asyncAction.Cancel();
				asyncAction.ResetModule();
			}
		}

		public override void StartGen()
		{
			foreach (var asyncAction in asyncActions) {
				asyncAction.Initialize(this, Container);
				asyncAction.Execute().Forget();
			}
		}

		private void OnDestroy()
		{
			foreach (var asyncAction in asyncActions) {
				Destroy(asyncAction);
			}
			
			foreach (var growHeuristic in growHeuristics) {
				Destroy(growHeuristic);
			}
			
			foreach (var growConstraint in growConstraints) {
				Destroy(growConstraint);
			}
			
			foreach (var onNodeSpawn in onNodeSpawnAction) {
				Destroy(onNodeSpawn);
			}
			
			if (initBranchAction != null) {
				Destroy(initBranchAction);
			}
			
			if (directionCalculator != null) {
				Destroy(directionCalculator);
			}
			
			if (nodeSpawn != null) {
				Destroy(nodeSpawn);
			}
		}

		public override void StopGen()
		{
			foreach (var asyncAction in asyncActions) {
				if (asyncAction.runUntilDisable) {
					continue;
				}
				asyncAction.Cancel();
				asyncAction.ResetModule();
			}

			foreach (var growHeuristic in growHeuristics) {
				growHeuristic.ResetModule();
			}

			foreach (var growConstraint in growConstraints) {
				growConstraint.ResetModule();
			}

			foreach (var onNodeSpawn in onNodeSpawnAction) {
				onNodeSpawn.ResetModule();
			}

			if (initBranchAction != null) {
				initBranchAction.ResetModule();
			}
			if (directionCalculator != null) {
				directionCalculator.ResetModule();
			}
			if (nodeSpawn != null) {
				nodeSpawn.ResetModule();
			}
		}
		
#if UNITY_EDITOR
		[Button]
		private void GetCount() => Debug.Log($"Branch Count: {Branches.Count}");
#endif
		
		public override void SetBranchParams(Branch b)
		{
			b.thickness = ivyParams.branchThickness;
			b.currentHeight = ivyParams.maxDistanceToSurface;
			var rend = b.branchMesh[^1].objRef.GetComponent<Renderer>();
			if (rend) {
				rend.sharedMaterial = ivyParams.meshParameters.branchMaterial;
			}
		}

		public override void AddBranch(Branch b)
		{
			SetBranchParams(b);
			Branches.Add(b);
		}

		public override void Initialize(GeneratorContainer gen)
		{
			base.Initialize(gen);
			seed = GetComponent<VineSeed>();
			for (int i = 0; i < asyncActions.Length; i++) {
				var action = asyncActions[i];
				var instance = ScriptableObject.Instantiate(action);
				asyncActions[i] = instance;
			}

			for (int i = 0; i < growHeuristics.Length; i++) {
				var action = growHeuristics[i];
				var instance = ScriptableObject.Instantiate(action);
				instance.Initialize(this, Container);
				growHeuristics[i] = instance;
			}

			for (int i = 0; i < growConstraints.Length; i++) {
				var action = growConstraints[i];
				var instance = ScriptableObject.Instantiate(action);
				instance.Initialize(this, Container);
				growConstraints[i] = instance;
			}

			for (int i = 0; i < onNodeSpawnAction.Length; i++) {
				var action = onNodeSpawnAction[i];
				var instance = ScriptableObject.Instantiate(action);
				instance.Initialize(this, Container);
				onNodeSpawnAction[i] = instance;
			}

			if (initBranchAction) {
				var instance = ScriptableObject.Instantiate(initBranchAction);
				instance.Initialize(this, Container);
				initBranchAction = instance;
			}

			var dirCalc = ScriptableObject.Instantiate(this.directionCalculator);
			dirCalc.Initialize(this, Container);
			directionCalculator = dirCalc;

			var nodeSpawnAction = ScriptableObject.Instantiate(this.nodeSpawn);
			nodeSpawnAction.Initialize(this, Container);
			nodeSpawn = nodeSpawnAction;
		}

		public override bool Step(int i)
		{
			var branch = Branches[i];

			if (!branch.alive || branch.Count == 0) {
				return false;
			}

			var stepLength = ivyParams.stepSize;
			var startPos = branch.LastNode.position;
			var startNormal = -branch.LastNode.surfaceNormal;

			var info = new BranchQueryInfo() {
				branch = branch,
				point = startPos,
				normal = startNormal,
				direction = branch.mainGrowDirection,
				ivyParams = ivyParams,
				stepLength = stepLength,
			};

			directionCalculator.Execute(ref info);

			if (!branch.initialized && initBranchAction != null) {
				if (initBranchAction.Execute(ref info)) {
					branch.initialized = true;
				}
				else {
					return false;
				}
			}

			foreach (var growConstraint in growConstraints) {
				if (!growConstraint.Execute(ref info)) {
					if (growConstraint.killBranchOnFail) {
						branch.alive = false;
					}

					return false;
				}
			}

			foreach (var growHeuristic in growHeuristics) {
				if (!growHeuristic.Execute(ref info) && growHeuristic.dealBreaker) {
					if (growHeuristic.killBranchOnFail) {
						branch.alive = false;
					}

					return false;
				}
			}

			nodeSpawn.Execute(ref info);

			branch.mainGrowDirection = info.direction;

			foreach (var nodeAction in onNodeSpawnAction) {
				nodeAction.Execute(ref info);
			}

			branch.hasChanged = true;
			return true;
		}
	}
}
