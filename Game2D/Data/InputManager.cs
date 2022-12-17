using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XInputium;
using XInputium.XInput;

namespace Game2D.Data
{
    public class InputManager
    {
        private readonly Dictionary<XButtons, Keys> _bindings;
        private readonly XGamepad _controller;
        private List<Keys> _activeKeys;

        public bool IsAnyActiveInput { get; private set; }

        public InputManager()
        {
            _controller = new XGamepad(); ;
            _activeKeys = new List<Keys>();

            _bindings = new Dictionary<XButtons, Keys>
            {
                { XButtons.A, Keys.Space },
                { XButtons.B, Keys.X },
                { XButtons.X, Keys.A },
                { XButtons.Y, Keys.S },
                { XButtons.LB, Keys.Q },
                { XButtons.RB, Keys.W },
                { XButtons.Start, Keys.Escape },
                { XButtons.Back, Keys.Tab }
            };
        }
        public bool IsKeyDown(Keys key) => _activeKeys.Contains(key);
        public void Update()
        {
            // Update the controller state
            _controller.Update();

            // reset keys
            _activeKeys.Clear();

            // Check if any of the bound buttons are pressed
            foreach (var binding in _bindings)
            {
                if (_controller.Buttons[binding.Key].IsPressed || GameManager.Instance.IsKeyDown(binding.Value))
                    _activeKeys.Add(binding.Value);
            }
        }
        public Vector2 GetMovementDirection()
        {
            // Check if the W or S keys are being pressed
            float y = 0;
            if (GameManager.Instance.IsKeyDown(Keys.W))
            {
                y = 1;
            }
            else if (GameManager.Instance.IsKeyDown(Keys.S))
            {
                y = -1;
            }

            // Check if the A or D keys are being pressed
            float x = 0;
            if (GameManager.Instance.IsKeyDown(Keys.A))
            {
                x = -1;
            }
            else if (GameManager.Instance.IsKeyDown(Keys.D))
            {
                x = 1;
            }

            // If no keyboard input was detected, check the gamepad joystick
            if (x == 0 && y == 0)
            {
                // Update the controller state
                _controller.Update();

                // Get the left joystick axis values
                x = _controller.LeftJoystick.X;
                y = _controller.LeftJoystick.Y;
            }

            IsAnyActiveInput = x + y > 0;

            // Return the movement direction as a Vector2
            return new Vector2(x, y);
        }
    }
}
