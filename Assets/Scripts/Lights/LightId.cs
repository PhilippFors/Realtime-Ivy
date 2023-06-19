using UnityEngine;
using Util.Locators;

namespace Lights
{
    /// <summary>
    /// Assign this to a light so it can be found and assign itself to LightCollection.cs
    /// </summary>
    public class LightId : MonoBehaviour
    {
        private void Start()
        {
            var lightCollection = ServiceLocator.Get<LightCollection>();
            lightCollection.Add(GetComponent<Light>());
        }
    }
}