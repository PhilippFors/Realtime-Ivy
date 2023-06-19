using Player.Input.Data;
using UnityEngine;

namespace Player.Input
{
    [DefaultExecutionOrder(-2)]
    public class InputInitializer : MonoBehaviour
    {
        [SerializeField] private InputConfig[] inputConfigs;

        private void Awake()
        {
            foreach (var config in inputConfigs) {
                config.Initialize();
            }
        }
    }
}