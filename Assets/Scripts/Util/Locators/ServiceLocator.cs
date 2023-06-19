using System.Collections.Generic;
using UnityEngine;

namespace Util.Locators
{
    /// <summary>
    /// Contains a static collection of Monobehaviour or other Unity Object classes.
    /// Only unique services should be used here, as it doesn't support multiple entities of a service.
    /// </summary>
    public static class ServiceLocator
    {
        private static Dictionary<int, Object> services = new();

        public static T Get<T>() where T : Object
        {
            services.TryGetValue(typeof(T).GetHashCode(), out var value);
            if (!value)
            {
                Debug.LogWarning($"Service type '{typeof(T).Name}' is not in collection.");
            }
            return (T) value;
        }

        public static void Clear()
        {
            services.Clear();
        }
        
        public static bool Register(Object obj)
        {
            var hash = obj.GetType().GetHashCode();
            if (services.ContainsKey(hash)) {
                return false;
            }

            services.Add(hash, obj);
            return true;
        }

        public static bool RegisterRange(IService[] obj)
        {
            foreach (var o in obj) {
                if (!Register((Component) o)) {
                    return false;
                }
            }

            return true;
        }

        public static void UnRegister(Object obj)
        {
            var hash = obj.GetType().GetHashCode();
            if (services.ContainsKey(hash))
            {
                services.Remove(hash);
            }
        }

        public static void UnregisterRange(IService[] obj)
        {
            foreach (var o in obj) {
                UnRegister((Component) o);
            }
        }
    }
}