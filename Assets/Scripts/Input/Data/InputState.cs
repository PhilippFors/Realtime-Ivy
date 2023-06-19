using UnityEngine;

namespace Player.Input.Data
{
    public class InputState
    {
        public bool movementPressed;
        public bool movementTriggered;
        public Vector2 movementDirection;

        public Vector2 mouseDelta;

        public Vector2 mousePosition;

        public bool mouseScrollTriggered;
        public float mouseScroll;

        public bool jumpTriggered;
        public bool jumpPressed;
        public bool jumpReleased;
        public float jumpHoldTime;

        public bool fire1Triggered;
        public bool fire1Pressed;
        public bool fire1Released;
        public float fire1HoldTime;

        public bool fire2Triggered;
        public bool fire2Pressed;
        public bool fire2Released;
        public float fire2HoldTime;
        
        public bool interactTriggered;
        public bool interactPressed;
        public bool interactReleased;
        public float interactHoldTime;
        
        public bool meleeTriggered;
        public bool meleePressed;
        public bool meleeReleased;
        public float meleeHoldTime;

        public bool absorberTriggered;
        public bool absorberPressed;
        public bool absorberReleased;
        public float absorberHoldTime;

        public bool slideTriggered;
        public bool slidePressed;
        public bool slideReleased;
        public float slideHoldTime;

        public bool dodgeTriggered;
        public bool dodgePressed;
        public bool dodgeReleased;
        public float dodgeHoldTime;

        public bool slot1Triggered;
        public bool slot1Pressed;
        public bool slot1Released;

        public bool slot2Triggered;
        public bool slot2Pressed;
        public bool slot2Released;

        public bool slot3Triggered;
        public bool slot3Pressed;
        public bool slot3Released;

        public bool slot4Triggered;
        public bool slot4Pressed;
        public bool slot4Released;

        public bool slot5Triggered;
        public bool slot5Pressed;
        public bool slot5Released;
    }
}