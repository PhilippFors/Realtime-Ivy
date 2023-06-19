using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VineGeneration.Core;

namespace VineGeneration.Generators.ModuleGenerator
{
	public abstract class BaseAsyncAction : ScriptableObject
	{
		public bool runUntilDisable;
		[SerializeField] protected float loopPause;

		protected CancellationTokenSource token;
		protected GeneratorContainer container;
		protected ModularGenerator generator;
		protected int loopPauseMilli;

		protected List<Branch> branches;

		public virtual void Initialize(ModularGenerator gen, GeneratorContainer cont)
		{
			var l = loopPause * 1000;
			loopPauseMilli = (int)l;
			container = cont;
			generator = gen;
			branches = gen.Branches;
		}

		public virtual async UniTaskVoid Execute()
		{
			token = new CancellationTokenSource();
			while (!token.IsCancellationRequested) {
				await ExecuteInternal();
				await UniTask.Delay(loopPauseMilli, cancellationToken: token.Token);
			}
			token.Dispose();
		}

		public void Cancel()
		{
			if (token == null) {
				return;
			}

			token.Cancel();
			token.Dispose();
			token = null;
		}

		public virtual void ResetModule()
		{
			
		}

		protected virtual async UniTask ExecuteInternal()
		{ }
	}
}
