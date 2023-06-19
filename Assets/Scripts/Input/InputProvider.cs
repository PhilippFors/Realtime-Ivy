using Player.Input.Data;
using Player.Input.InputSystems;
using UnityEngine;

namespace Player.Input
{
    [CreateAssetMenu(menuName = "Input/InputProvider")]
    public class InputProvider : ScriptableObject
    {
        [SerializeField] private BaseInputSystem[] systems;

        private InputState cache;
        private int frameCount;

        public InputState GetState()
        {
            if (Time.frameCount == frameCount) {
                return cache;
            }

            var inputState = new InputState();
            foreach (var system in systems) {
                // system.ProcessInput(ref inputState);
            }

            frameCount = Time.frameCount;
            cache = inputState;
            return inputState;
        }
    }
}