using System.Collections.Generic;
using UnityEngine;
using Util.Locators;

namespace Lights
{
    /// <summary>
    /// Collection of lights in a scene.
    /// </summary>
    public class LightCollection : MonoBehaviourService
    {
        public static LightCollection current;
        
        public List<Light> Lights => lights;
        
        [SerializeField] private List<Light> lights = new();

        private void Awake()
        {
            current = this;
        }

        public void Add(Light l)
        {
            lights.Add(l);
        }
    }
}