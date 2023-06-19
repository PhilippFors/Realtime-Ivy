using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjectPools
{
	[DefaultExecutionOrder(-25)]
	public class ObjectPoolCollection : MonoBehaviour
	{
		public static ObjectPoolCollection current;

		[SerializeField] private List<BaseScriptableObjectPool> pools = new List<BaseScriptableObjectPool>();

		private void Awake()
		{
			current = this;
			foreach (var pool in pools) {
				pool.InitPool();
			}
		}

		private void OnDestroy()
		{
			foreach (var pool in pools) {
				pool.Clear();
			}
		}

		public BaseScriptableObjectPool<T> GetPool<T>(string id) where T : Component
		{
			var pool = pools.Find(p => p.id == id) as BaseScriptableObjectPool<T>;
			return pool;
		}
		public T GetBasePool<T>(string id) where T : BaseScriptableObjectPool
		{
			var pool = pools.Find(p => p.id == id) as T;
			return pool;
		}
	}
}
