using UnityEngine;
using UnityEngine.Pool;
namespace ScriptableObjectPools
{
	[CreateAssetMenu(fileName = "MeshObjectPool", menuName = "ObjectPools/Mesh Object Pool")]
	public class MeshObjectPool : BaseScriptableObjectPool
	{
		[SerializeField] private int defaultSize = 50;
		[SerializeField] private UnityEngine.Rendering.IndexFormat indexFormat;

		private IObjectPool<Mesh> meshPool;

		public Mesh GetObject()
		{
			var m = meshPool.Get();
			return m;
		}

		public void Release(Mesh mesh)
		{
			meshPool.Release(mesh);
		}

		private Mesh CreateMesh()
		{
			var mesh = new Mesh();
			mesh.indexFormat = indexFormat;
			return mesh;
		}

		private void ReturnMesh(Mesh mesh)
		{
			mesh.Clear();
		}

		private void DestroyMesh(Mesh mesh)
		{
			mesh.Clear();
		}

		public override void Clear()
		{
			meshPool.Clear();
			initialized = false;
		}

		public override void InitPool()
		{
			if (initialized) {
				return;
			}
			meshPool = new ObjectPool<Mesh>(CreateMesh, actionOnRelease: ReturnMesh, actionOnDestroy: DestroyMesh, defaultCapacity: defaultSize);
			initialized = true;
		}
	}
}
