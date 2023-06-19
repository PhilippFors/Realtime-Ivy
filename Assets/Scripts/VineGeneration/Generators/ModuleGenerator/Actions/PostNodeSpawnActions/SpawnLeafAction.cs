using ScriptableObjectPools;
using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Util;
namespace VineGeneration.Generators.ModuleGenerator.Actions.PostNodeSpawnActions
{
    [CreateAssetMenu(menuName = "VineGeneration/Actions/Spawn leave action")]
    public class SpawnLeafAction : BaseGrowAction
    {
        [SerializeField] private bool useVertLimit;
        [SerializeField] private int minLeaves = 2;
        [SerializeField] private int maxLeaves = 4;
        [SerializeField] private float leaveSize = 0.08f;
        [SerializeField] private Vector2 leaveAngle = new Vector2(30, 45);
        [SerializeField] private TransformObjectPool leafPool;
        private GameObject[] leaves;

        public override void Initialize(ModularGenerator gen, GeneratorContainer c)
        {
            base.Initialize(gen, c);
            leaves = new GameObject[maxLeaves * 2];
        }

        public override bool Execute(ref BranchQueryInfo info)
        {
            if (!info.branch.alive || !info.branch.hasChanged)
            {
                return true;
            }
            
            var leaveAmount = container.Rng.Range(minLeaves, maxLeaves + 1);
            for(int i = 0 ; i < leaves.Length; i++) {
                leaves[i] = leafPool.GetObject(false).gameObject;
            }
            if (info.branch.Count > 1)
            {
                LeafSpawnHelper.SpawnLeaves(ref leaves, leaveAmount, leaveSize, leaveAngle, info, container.Rng, useVertLimit);
            }
            for(int i = 0 ; i < leaves.Length; i++) {
                leafPool.ReleaseObject(leaves[i].transform);
            }

            return true;
        }
    }
}