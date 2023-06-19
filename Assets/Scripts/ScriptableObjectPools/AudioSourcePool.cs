using UnityEngine;
namespace ScriptableObjectPools
{
	[CreateAssetMenu(fileName = "AudioSourcePool", menuName = "ObjectPools/Audio Source Pool")]
	public class AudioSourcePool : BaseScriptableObjectPool<AudioSource>
	{
	}
}
