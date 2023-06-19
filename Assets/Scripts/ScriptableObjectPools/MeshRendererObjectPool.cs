using UnityEngine;
namespace ScriptableObjectPools
{
	[CreateAssetMenu(fileName = "MeshRendererObjectPool", menuName = "ObjectPools/MeshRendererObjectPool")]
	public class MeshRendererObjectPool : BaseScriptableObjectPool<MeshRenderer>
	{
		protected override MeshRenderer InstantiateObject(bool setActive)
		{
			var obj =  base.InstantiateObject(setActive);
			obj.transform.position = Vector3.zero;
			// obj.GetComponent<MeshFilter>().sharedMesh = new Mesh();
			obj.name = "MeshRendererObject";
			return obj;
		}
		public override void ReleaseObject(MeshRenderer obj)
		{
			base.ReleaseObject(obj);
			obj.sharedMaterial = null;
			// obj.GetComponent<MeshFilter>().sharedMesh.Clear();
			obj.name = "MeshRendererObject";
		}
	}
}
