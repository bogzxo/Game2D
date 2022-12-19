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
    public struct InputManagerConfiguration
    {
        public Dictionary<XButtons, Keys> ControllerBindings { get; set; }
        public float DeadZone { get; set; }

        public static InputManagerConfiguration Default = new InputManagerConfiguration()
        {
            ControllerBindings = new Dictionary<XButtons, Keys>
            {
                { XButtons.A, Keys.Space },
                { XButtons.B, Keys.X },
                { XButtons.X, Keys.A },
                { XButtons.Y, Keys.S },
                { XButtons.LB, Keys.Q },
                { XButtons.RB, Keys.W },
                { XButtons.Start, Keys.Escape },
                { XButtons.Back, Keys.Tab }
            },
            DeadZone = 0.25f
        };
    }
    public class InputManager
    {
        private readonly XGamepad _controller;
        private List<Keys> _activeKeys;

        public InputManagerConfiguration Configuration { get; private set; }

        public bool IsAnyActiveInput { get; private set; }
        public Vector2 LastDirection { get; private set; }
        public Vector2 Direction { get; private set; }

        public InputManager(in InputManagerConfiguration config)
        {
            _controller = new XGamepad();
            _activeKeys = new List<Keys>();

            Configuration = config;
        }
        public bool IsKeyDown(Keys key) => _activeKeys.Contains(key);
        public void Update()
        {
            // Update the controller state
            _controller.Update();

            // reset keys
            _activeKeys.Clear();

            // Check if any of the bound buttons are pressed
            foreach (var binding in Configuration.ControllerBindings)
            {
                if (_controller.Buttons[binding.Key].IsPressed || GameManager.Instance.IsKeyDown(binding.Value))
                    _activeKeys.Add(binding.Value);
            }

            UpdateMovementDirection();
        }

        // TODO: fix this spaghetti shit 

        private void UpdateMovementDirection()
        {
            float y = 0;
            if (GameManager.Instance.IsKeyDown(Keys.W))
                y = 1;
            else if (GameManager.Instance.IsKeyDown(Keys.S))
                y = -1;


            float x = 0;
            if (GameManager.Instance.IsKeyDown(Keys.A))
                x = -1;
            else if (GameManager.Instance.IsKeyDown(Keys.D))
                x = 1;


            if (x == 0 && y == 0)
            {
                _controller.Update();

                x = _controller.LeftJoystick.X;
                y = _controller.LeftJoystick.Y;

                ProcessJoystickInput(ref x, ref y);
            }

            IsAnyActiveInput = Direction.LengthSquared > 0;

            if (Direction.X + Direction.Y != 0)
                LastDirection = Direction;

            Direction = new Vector2(x, y);
        }

        // Scaled Radial Dead Zone 
        private void ProcessJoystickInput(ref float x, ref float y)
        {
            // too lazy to refactor

            Vector2 stickInput = new Vector2(x, y);
            if (stickInput.Length < Configuration.DeadZone)
                stickInput = Vector2.Zero;
            else
                stickInput = stickInput.Normalized() * ((stickInput.Length - Configuration.DeadZone) / (1 - Configuration.DeadZone));

            x = stickInput.X;
            y = stickInput.Y;
        }
    }
}
