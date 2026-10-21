using System;
using UnityEngine.InputSystem;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Utils.Scripts
{
    // Как InputController в Combo-Dungeon, только под телефон: вместо клавиши R — тап по экрану (мышь в редакторе).
    public class InputController : ITickable
    {
        private bool _wasPressed;

        public event Action OnTap;

        // Тап = переход «не нажато → нажато» между кадрами (ловит и нажатие, пришедшее между апдейтами).
        public void Tick()
        {
            bool isPressed = Pointer.current?.press.isPressed == true;

            if (isPressed && !_wasPressed)
                OnTap?.Invoke();

            _wasPressed = isPressed;
        }
    }
}
