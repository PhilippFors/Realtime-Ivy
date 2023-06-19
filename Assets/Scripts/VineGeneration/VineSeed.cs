using UnityEngine;
using VineGeneration.Core;
using VineGeneration.Generators.ModuleGenerator;
using VineGeneration.Util;

namespace VineGeneration
{
	public class VineSeed : MonoBehaviour
	{
		public bool seedActive;

		[SerializeField] private LayerMask hitMask;
		[SerializeField] private Vector3 currentSurfaceNormal;
		[SerializeField] private bool canPlaceSeed;

		private GeneratorContainer container;
		private Rigidbody rb;

		private void Awake()
		{
			container = GetComponent<GeneratorContainer>();
			rb = GetComponent<Rigidbody>();
		}
		
		public void OnGrabbed()
		{
			if (seedActive) {
				container.StopGeneration();
			}
			seedActive = false;
			rb.freezeRotation = false;
			rb.isKinematic = false;
			rb.useGravity = true;
			canPlaceSeed = false;
		}

		public void OnRelease()
		{
			canPlaceSeed = true;
		}

		private void OnCollisionEnter(Collision other)
		{
			if (!canPlaceSeed) {
				return;
			}

			var contact = GetClosestContact(other);
			currentSurfaceNormal = contact.normal;
			
			if (!other.collider.GetComponent<VineSeed>()) {
				rb.useGravity = false;
				rb.isKinematic = true;
				rb.freezeRotation = true;
			}
			
			if (Physics.Raycast(transform.position, -currentSurfaceNormal, out var hit, 1f, hitMask)) {
				canPlaceSeed = false;
				seedActive = true;
				
				var generator = container.GetGenerator<ModularGenerator>(GeneratorType.Main);
				var br = GrowUtil.GetBranch(container.Rng, hit.point + hit.normal * generator.ivyParams.maxDistanceToSurface, hit.normal, this, true);
				generator.AddBranch(br);
				container.StartGeneration();
			}
		}

		private ContactPoint GetClosestContact(Collision other)
		{
			var distance = Mathf.Infinity;
			var closestContact = other.contacts[0];
			foreach (var contact in other.contacts) {
				var dist = Vector3.Distance(transform.position, contact.point);
				if (dist < distance) {
					distance = dist;
					closestContact = contact;
				}
			}
			return closestContact;
		}
	}
}
