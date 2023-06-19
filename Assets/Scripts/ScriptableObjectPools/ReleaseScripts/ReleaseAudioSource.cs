using System.Collections;
using UnityEngine;
namespace ScriptableObjectPools.ReleaseScripts
{
	public class ReleaseAudioSource : MonoBehaviour
	{
		public AudioSourcePool pool;
		public ReleaseEvent releaseEvent;
		public float releaseTime;

		private AudioSource audioSource;
		private float timer;

		private void Awake()
		{
			audioSource = GetComponent<AudioSource>();
		}

		private void OnEnable()
		{
			timer = 0;
			if (releaseEvent == ReleaseEvent.OnFinishPlaying) {
				StartCoroutine(WaitUntilEnd());
			}
		}

		public enum ReleaseEvent
		{
			OnFinishPlaying,
			OnTime
		}

		private void Update()
		{
			if (releaseEvent == ReleaseEvent.OnTime) {
				timer += Time.deltaTime;
				if (timer >= releaseTime) {
					pool.ReleaseObject(audioSource);
				}
			}
		}

		private IEnumerator WaitUntilEnd()
		{
			yield return new WaitUntil(() => audioSource.isPlaying);

			yield return new WaitUntil(() => !audioSource.isPlaying);

			pool.ReleaseObject(audioSource);
		}
	}
}
