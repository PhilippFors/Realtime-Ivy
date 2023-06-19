using System.Collections.Generic;
using UnityEngine;
using VineGeneration.Core;
namespace VineGeneration.Generators.ModuleGenerator.Actions.PostNodeSpawnActions
{
	[CreateAssetMenu(fileName = "TipVisualizeAction", menuName = "VineGeneration/Actions/NodeSpawnActions/TipVisualizeAction")]
	public class TipVisualizeAction : BaseGrowAction
	{
		[SerializeField] private GameObject prefab;
		private Dictionary<int, GameObject> tips;

		public override void Initialize(ModularGenerator gen, GeneratorContainer c)
		{
			base.Initialize(gen, c);
			tips = new Dictionary<int, GameObject>();
		}

		public override bool Execute(ref BranchQueryInfo info)
		{
			var hash = info.branch.GetHashCode();
			if (!tips.ContainsKey(hash)) {
				tips.Add(hash, Instantiate(prefab));
			}

			var tip = tips[info.branch.GetHashCode()];
			tip.transform.position = info.branch.LastNode.position;

			return true;
		}

		public override void ResetModule()
		{
			foreach (var t in tips) {
				Destroy(t.Value);
			}

			tips.Clear();
		}
	}
}
