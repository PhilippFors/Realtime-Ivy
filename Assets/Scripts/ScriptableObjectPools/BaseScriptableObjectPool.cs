using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
namespace ScriptableObjectPools
{
	public abstract class BaseScriptableObjectPool : ScriptableObject
	{
		public string id;
		public bool initialized;
		public abstract void InitPool();
		public virtual void Reset() { }

		public virtual void Clear() { }
	}

	public abstract class BaseScriptableObjectPool<T> : BaseScriptableObjectPool where T : Component
	{
		[SerializeField, Range(1, 1000)] private int initialSize = 50;
		[SerializeField] protected T prefab;

		protected static int InstanceIdCounter;

		public readonly Queue<T> pool = new Queue<T>();
		protected readonly List<T> used = new List<T>();
		protected Transform poolTransform;

		public override void Reset()
		{
			foreach (var obj in used) {
				if (!obj) {
					continue;
				}
				obj.gameObject.SetActive(false);
				pool.Enqueue(obj);
				obj.transform.parent = poolTransform;
			}
			used.Clear();
		}

		public override void Clear()
		{
			foreach (var obj in used) {
				if (!obj) {
					continue;
				}
				Destroy(obj.gameObject);
			}
			used.Clear();
			foreach (var obj in pool) {
				if (!obj) {
					continue;
				}
				Destroy(obj.gameObject);
			}
			initialized = false;
		}

		public override void InitPool()
		{
			poolTransform = new GameObject($"{name} [{GetInstanceID()}]").transform;

			if (initialized) {
				foreach (var obj in pool) {
					obj.transform.parent = poolTransform;
				}
				return;
			}

			pool.Clear();
			used.Clear();
			for (int i = 0; i < initialSize; i++) {
				var newObj = InstantiateObject(false);
				newObj.transform.parent = poolTransform;
				pool.Enqueue(newObj);
			}
			initialized = true;
		}

		public T[] GetAmount(int amount, bool setActive = true)
		{
			var list = new T[amount];

			for (int i = 0; i < amount; i++) {
				list[i] = GetObject(setActive);
			}

			return list;
		}

		public T GetObject(bool setActive = true)
		{
			T result = null;
			if (pool.Count == 0) {
				result = InstantiateObject(setActive);
			}
			else {
				var obj = pool.Dequeue();
				GameObject go = obj.gameObject;
				go.SetActive(setActive);
				result = obj;
			}

			used.Add(result);
			result.transform.parent = null;
			return result;
		}

		public virtual void ReleaseObject(T obj)
		{
			obj.gameObject.SetActive(false);

			Assert.IsFalse(pool.Contains(obj), "Trying to release object multiple times.");

			pool.Enqueue(obj);
			used.Remove(obj);
			obj.transform.parent = poolTransform;
		}

		protected virtual T InstantiateObject(bool setActive)
		{
			var newObj = Instantiate(prefab, poolTransform);
			newObj.gameObject.SetActive(setActive);
			newObj.name = $"{newObj.name} [{InstanceIdCounter++}]";
			return newObj;
		}
	}
}
