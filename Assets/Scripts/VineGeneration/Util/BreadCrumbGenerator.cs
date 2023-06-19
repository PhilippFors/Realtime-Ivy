using System.Collections.Generic;
using ScriptableObjectPools;
using ScriptableObjectPools.ReleaseScripts;
using UnityEngine;

namespace VineGeneration.Util
{
	[DefaultExecutionOrder(-20)]
	public class BreadCrumbGenerator : MonoBehaviour
	{
		public static readonly List<BreadCrumb> BreadCrumbs = new List<BreadCrumb>();
		
		public TransformObjectPool spherePool;
		private List<BreadCrumb> internalBreadCrumbs;
		public float breadCrumbRadius = 1f;
		public float breadCrumbAliveTime = 3;

		private float timer;
		private bool canPlace;

		private void Awake()
		{
			internalBreadCrumbs = new List<BreadCrumb>();
		}

		private void Start()
		{
			breadCrumbRadius = GlobalVineSettings.current.breadCrumbRadius;
			breadCrumbAliveTime = GlobalVineSettings.current.breadCrumbAliveTime;
		}

		private void Update()
		{
			TickBreadCrumbAliveTime();
		}

		public void GenerateBreadCrumbs(Ray ray)
		{
			if (!Physics.Raycast(ray, out var mainHit, GlobalVineSettings.current.maximumPaintDistance, GlobalVineSettings.current.environmentMask)) {
				return;
			}
			
			var lastPoint = Vector3.zero;
			if (internalBreadCrumbs.Count >= 1) {
				lastPoint = internalBreadCrumbs[^1].center;
			}

			var distToLast = Vector3.Distance(lastPoint, mainHit.point);
			if (distToLast >= breadCrumbRadius * 0.85f || internalBreadCrumbs.Count == 0) {
				var c = mainHit.point;

				var p = c + mainHit.normal * 0.015f;
				Vector3 direction = Vector3.zero;
				if (internalBreadCrumbs.Count > 0) {
					var lastCrumb = internalBreadCrumbs[^1];
					direction = (p - lastCrumb.point).normalized;
					lastCrumb.direction = direction;
				}

				BreadCrumb newC = new BreadCrumb {
					center = p,
					point = p,
					direction = direction,
					normal = mainHit.normal,
					aliveTime = breadCrumbAliveTime
				};

				BreadCrumbs.Add(newC);
				internalBreadCrumbs.Add(newC);
				
				var sphere = spherePool.GetObject(false);
				sphere.GetComponent<TimedReleaseToPool>().timeToRelease = breadCrumbAliveTime;
				sphere.transform.position = p;
				sphere.transform.localScale = Vector3.one * (breadCrumbRadius * 2);
				sphere.gameObject.SetActive(true);
			}
		}

		private void TickBreadCrumbAliveTime()
		{
			for (int i = 0; i < internalBreadCrumbs.Count; i++) {
				var b = internalBreadCrumbs[i];
				b.aliveTime -= Time.deltaTime;
				if (b.aliveTime <= 0) {
					BreadCrumbs.Remove(b);
					internalBreadCrumbs.Remove(b);
					i--;
				}
			}
		}

		private void OnDrawGizmos()
		{
			if (BreadCrumbs == null) {
				return;
			}

			foreach (var b in BreadCrumbs) {
				Gizmos.color = Color.yellow;
				Gizmos.DrawSphere(b.point, 0.05f);
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(b.point, breadCrumbRadius);
			}
		}
	}
}
