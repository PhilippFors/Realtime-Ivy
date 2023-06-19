using Player.Input.Data;
using UnityEngine;

namespace Player.Input.InputSystems
{
    public abstract class BaseInputSystem : ScriptableObject
    {
        public virtual InputState GetState()
        {
            return default;
        }
        
        protected abstract void ProcessInput(InputState state);
    }
}