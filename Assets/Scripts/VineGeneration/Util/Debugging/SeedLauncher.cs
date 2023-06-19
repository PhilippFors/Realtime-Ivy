using Player.Input;
using Player.Input.Data;
using UnityEngine;

namespace VineGeneration.Util.Debugging
{
    public class SeedLauncher : MonoBehaviour
    {
        public InputConfig debugControls;
        public VineSeed seedPrefab;
        public float launchStrength;
        private Transform mainCam;

        private void Start()
        {
            mainCam = Camera.main.transform;
        }

        private void Update()
        {
            if (debugControls.Triggered(Inputs.Fire1))
            {
                var seed = Instantiate(seedPrefab);
                seed.transform.position = mainCam.position;
                var rb = seed.GetComponent<Rigidbody>();
                rb.AddForce(mainCam.forward * launchStrength, ForceMode.Impulse);
            }
        }
    }
}