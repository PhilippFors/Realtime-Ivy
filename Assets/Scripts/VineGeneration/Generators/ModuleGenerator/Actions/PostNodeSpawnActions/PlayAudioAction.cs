using ScriptableObjectPools;
using UnityEngine;
using VineGeneration.Core;
namespace VineGeneration.Generators.ModuleGenerator.Actions.PostNodeSpawnActions
{
	[CreateAssetMenu(fileName = "PlayAudioAction", menuName = "VineGeneration/Actions/PlayAudioAction")]
	public class PlayAudioAction : BaseGrowAction
	{
		[SerializeField] private float volume = 0.5f;
		[SerializeField] private float maxDistance = 10f;
		[SerializeField] private AudioClip clip;
		[SerializeField] private AudioSourcePool audioSourcePool;
		private AudioSource audioSource;
		
		public override void Initialize(ModularGenerator gen, GeneratorContainer c)
		{
			base.Initialize(gen, c);
			audioSource = audioSourcePool.GetObject();
			audioSource.loop = false;
			audioSource.clip = clip;
			audioSource.maxDistance = maxDistance;
			audioSource.volume = volume;
		}

		private void Reset()
		{
			audioSourcePool.ReleaseObject(audioSource);
		}

		public override bool Execute(ref BranchQueryInfo info)
		{
			if (info.branch.hasChanged && !audioSource.isPlaying) {
				audioSource.Play();
			}
			
			audioSource.transform.position = info.point;

			return true;
		}
	}
}
