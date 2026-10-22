using System;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Utils.Scripts
{
    public class InputController : ITickable
    {
        private bool _wasPressed;

        public event Action OnTap;

        public void Tick()
        {
            bool isPressed = Pointer.current?.press.isPressed == true;

            if (isPressed && !_wasPressed)
                OnTap?.Invoke();

            _wasPressed = isPressed;
        }
    }
}
