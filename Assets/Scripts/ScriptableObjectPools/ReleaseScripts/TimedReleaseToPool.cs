using DG.Tweening;
using UnityEngine;
namespace ScriptableObjectPools.ReleaseScripts
{
	public class TimedReleaseToPool : MonoBehaviour
	{
		public TransformObjectPool pool;
		public float timeToRelease;
		
		[SerializeField] private MeshRenderer meshRenderer;

		private float currentTime;
		private bool run; 
		
		private void OnEnable()
		{
			meshRenderer.material.DOFloat(0.5f, "_Mult", 0.5f);
			currentTime = 0;
			run = true;
		}

		private void Update()
		{
			if (!run) {
				return;
			}
			
			if (currentTime <= timeToRelease) {
				currentTime += Time.deltaTime;
				return;
			}

			run = false;
			meshRenderer.material.DOFloat(0, "_Mult", 0.5f).onComplete += () => pool.ReleaseObject(transform);
		}
	}
}
