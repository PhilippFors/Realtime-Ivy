using UnityEngine;

namespace Util.Locators
{
    /// <summary>
    /// Base class for services. Inherit from this to define a service class.
    /// </summary>
    [RequireComponent(typeof(ServiceID))]
    public class MonoBehaviourService : MonoBehaviour, IService
    {
    }
}