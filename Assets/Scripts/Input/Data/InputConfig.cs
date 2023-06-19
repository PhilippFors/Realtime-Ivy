using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input.Data
{
    [CreateAssetMenu(fileName = "New Input", menuName = "Input/Input Config")]
    public class InputConfig : ScriptableObject
    {
        [SerializeField] private InputActionAsset inputActionMap;
        [SerializeField] private List<InputCombination> inputActions = new List<InputCombination>();
        
        private Dictionary<Inputs, InputActionState> inputDictionary;
        
        public void Initialize()
        {
            inputDictionary = new Dictionary<Inputs, InputActionState>();
            
            foreach (var input in inputActions) {
                inputDictionary.Add(input.inputs, new InputActionState(input.Action, input.inputs));
            }
            
            inputActionMap.Enable();
            EnableAllInputs();
        }
        
        public InputActionState Get(Inputs pattern)
        {
            inputDictionary.TryGetValue(pattern, out var val);
            return val;
        }

        public InputAction GetAction(Inputs pattern) => Get(pattern).GetAction();
        
        public T ReadValue<T>(Inputs pattern) where T : struct => Get(pattern).ReadValue<T>();

        /// <summary>
        /// Checks if the input was triggered this frame.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool Triggered(Inputs pattern) => Get(pattern).Triggered;

        /// <summary>
        /// Checks if the input is currently pressed.
        /// For continuous checks.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool IsPressed(Inputs pattern) => Get(pattern).IsPressed;

        /// <summary>
        /// Checks if the input was released this frame.
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public bool Released(Inputs pattern) => Get(pattern).Released;

        public void EnableInput(Inputs pattern) => Get(pattern).Enable();

        public void DisableInput(Inputs pattern) => Get(pattern).Disable();
        
        public void EnableAllInputs()
        {
            foreach (var input in inputDictionary) {
                input.Value.GetAction().Enable();
            }
        }

        public void DisableAllInputs()
        {
            foreach (var input in inputDictionary) {
                input.Value.GetAction().Disable();
            }
        }
    }
}