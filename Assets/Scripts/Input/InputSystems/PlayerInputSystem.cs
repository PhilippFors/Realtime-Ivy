using Player.Input.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player.Input.InputSystems
{
    [CreateAssetMenu(menuName = "Input/Systems/Player Input System")]
    public class PlayerInputSystem : BaseInputSystem
    {
        [field: SerializeField] public InputConfig InputConfig { get; private set; }
        
        private InputState inputCache;
        private int frameCount;
        private bool updatedThisFrame;
        
        public override InputState GetState()
        {
            if (inputCache == null)
            {
                inputCache = new InputState();
            }
            
            if (Time.frameCount == frameCount)
            {
                updatedThisFrame = false;
                return inputCache;
            }
            
            if (InputSystem.settings.updateMode == InputSettings.UpdateMode.ProcessEventsManually && !updatedThisFrame)
            {
                updatedThisFrame = true;
                InputSystem.Update();
            }

            ProcessInput(inputCache);
            frameCount = Time.frameCount;
            return inputCache;
        }

        public InputActionState Get(Inputs pattern) => InputConfig.Get(pattern);

        protected override void ProcessInput(InputState state)
        {
            var movement = InputConfig.Get(Inputs.Movement);
            state.movementTriggered = movement.Triggered;
            state.movementPressed = movement.IsPressed;
            state.movementDirection = movement.ReadValue<Vector2>();

            state.mouseDelta = InputConfig.ReadValue<Vector2>(Inputs.MouseDelta);

            state.mousePosition = InputConfig.ReadValue<Vector2>(Inputs.MousePosition);

            var mouseScroll = InputConfig.Get(Inputs.MouseScroll);
            state.mouseScrollTriggered = mouseScroll.Triggered;
            state.mouseScroll = mouseScroll.ReadValue<Vector2>().y;

            var jump = InputConfig.Get(Inputs.Jump);
            state.jumpTriggered = jump.Triggered;
            state.jumpPressed = jump.IsPressed;
            state.jumpReleased = jump.Released;
            state.jumpHoldTime = jump.HoldTime;

            var fire1 = InputConfig.Get(Inputs.Fire1);
            state.fire1Triggered = fire1.Triggered;
            state.fire1Pressed = fire1.IsPressed;
            state.fire1Released = fire1.Released;
            state.fire1HoldTime = fire1.HoldTime;

            var fire2 = InputConfig.Get(Inputs.Fire2);
            state.fire2Triggered = fire2.Triggered;
            state.fire2Pressed = fire2.IsPressed;
            state.fire2Released = fire2.Released;
            state.fire2HoldTime = fire2.HoldTime;

            var interact = InputConfig.Get(Inputs.Interact);
            state.interactTriggered = interact.Triggered;
            state.interactPressed = interact.IsPressed;
            state.interactReleased = interact.Released;
            state.interactHoldTime = interact.HoldTime;

            var absorber = InputConfig.Get(Inputs.Absorber);
            state.absorberTriggered = absorber.Triggered;
            state.absorberPressed = absorber.IsPressed;
            state.absorberReleased = absorber.Released;
            state.absorberHoldTime = absorber.HoldTime;

            var melee = InputConfig.Get(Inputs.Melee);
            state.meleeTriggered = melee.Triggered;
            state.meleePressed = melee.IsPressed;
            state.meleeReleased = melee.Released;
            state.meleeHoldTime = melee.HoldTime;

            var slide = InputConfig.Get(Inputs.Slide);
            state.slideTriggered = slide.Triggered;
            state.slidePressed = slide.IsPressed;
            state.slideReleased = slide.Released;
            state.slideHoldTime = slide.HoldTime;

            var dodge = InputConfig.Get(Inputs.Dodge);
            state.dodgeTriggered = dodge.Triggered;
            state.dodgePressed = dodge.IsPressed;
            state.dodgeReleased = dodge.Released;
            state.dodgeHoldTime = dodge.HoldTime;

            var slot1 = InputConfig.Get(Inputs.Slot1);
            state.slot1Triggered = slot1.Triggered;
            state.slot1Pressed = slot1.IsPressed;
            state.slot1Released = slot1.Released;

            var slot2 = InputConfig.Get(Inputs.Slot2);
            state.slot2Triggered = slot2.Triggered;
            state.slot2Pressed = slot2.IsPressed;
            state.slot2Released = slot2.Released;

            var slot3 = InputConfig.Get(Inputs.Slot3);
            state.slot3Triggered = slot3.Triggered;
            state.slot3Pressed = slot3.IsPressed;
            state.slot3Released = slot3.Released;

            var slot4 = InputConfig.Get(Inputs.Slot4);
            state.slot4Triggered = slot4.Triggered;
            state.slot4Pressed = slot4.IsPressed;
            state.slot4Released = slot4.Released;

            var slot5 = InputConfig.Get(Inputs.Slot5);
            state.slot5Triggered = slot5.Triggered;
            state.slot5Pressed = slot5.IsPressed;
            state.slot5Released = slot5.Released;
        }
    }
}