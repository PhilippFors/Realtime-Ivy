using UnityEngine;

namespace Util.Locators
{
    /// <summary>
    /// Attach this to a gameobject with a service and it will register the object.
    /// </summary>
    [DefaultExecutionOrder(-1)]
    public class ServiceID : MonoBehaviour
    {
        private IService[] registeredObjects;
        
        private void OnEnable()
        {
            var service = GetComponents<IService>();
            // var temp = new List<IService>();
            // for (int i = 0; i < service.Length; i++) {
            //     if (ServiceLocator.Register((Component) service[i])) {
            //         temp.Add(service[i]);
            //     }
            // }

            ServiceLocator.RegisterRange(service);
            registeredObjects = service;
        }

        private void OnDisable()
        {
            ServiceLocator.UnregisterRange(registeredObjects);
            registeredObjects = null;
        }
    }
}
