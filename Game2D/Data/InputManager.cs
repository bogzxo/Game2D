using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.Data
{
    public enum JoystickButton
    {
        A,
        B,
        X,
        Y,
        Start,
        Select,
        Up,
        Down,
        Left,
        Right,
        Home,
        L1,
        L2,
        R1,
        R2
    }


    public class InputManager
    {
        public bool IsAnyActiveInput { get; private set; }

        private Vector2 oldAxis;

        public JoystickState State { get; private set; }

        public Dictionary<Keys, JoystickButton> GamepadMapping { get; private set; }

        public InputManager()
        {
            GamepadMapping = new Dictionary<Keys, JoystickButton> {
                { Keys.Space, JoystickButton.X }
            };
        }

        public void Update(float dt)
        {

        }

        public Vector2 GetMovementAxis()
        {
            Vector2 axis = Vector2.Zero;

            if (GameManager.Instance.JoystickStates.FirstOrDefault() != null)
            {
                var state = GameManager.Instance.JoystickStates.FirstOrDefault();

                State = state;

                axis.X = state.GetAxis(0);
                axis.Y = -state.GetAxis(1);
            }
            if (GameManager.Instance.KeyboardState.IsKeyDown(Keys.W)) axis.Y = 1;
            if (GameManager.Instance.KeyboardState.IsKeyDown(Keys.S)) axis.Y = -1;
            if (GameManager.Instance.KeyboardState.IsKeyDown(Keys.A)) axis.X = -1;
            if (GameManager.Instance.KeyboardState.IsKeyDown(Keys.D)) axis.X = 1;

            IsAnyActiveInput = MathF.Round(axis.Length, 3) > 0;

            oldAxis = axis;
            return axis;
        }
    }
}
