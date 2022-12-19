using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.Entities.Components
{
    public class EntityPhysicsComponent : IEntityComponent
    {
        public bool Enabled { get; set; } = true;

        // The position of the object
        public Vector2 Position { get; set; }

        // The Size of the object
        public Vector2 Size { get; set; }

        // The velocity of the object
        public Vector2 Velocity { get; set; }

        // The acceleration of the object
        public Vector2 Acceleration { get; set; }

        // The mass of the object
        public float Mass { get; set; }

        // The friction of the object
        public float Friction { get; set; } = 0.9f; // default value for friction

        // The gravity of the game world
        public Vector2 Gravity { get; set; } = new Vector2(0, -1.5f); // default value for gravity

        // If the player is on the ground
        public bool OnGround { get; private set; }

        private Vector2 _gravitySpeed;
        public EntityPhysicsComponent(Vector2 position, Vector2 size, float mass)
        {
            Size = size;
            Position = position;
            Mass = mass;
        }

        public void Update(float deltaTime)
        {
            // Check ground collision
            OnGround = RectangularCollision(Position - new Vector2(0, Size.Y / 2), GameManager.Instance.Player.Size);

            // Apply gravity
            _gravitySpeed = !OnGround ? Vector2.Lerp(_gravitySpeed, Gravity, deltaTime * 0.1f) : Vector2.Zero;
            Acceleration += _gravitySpeed;

            // Apply friction
            Velocity *= Friction;

            // Update velocity based on acceleration
            Velocity += Acceleration * deltaTime;

            // Check collisions using the target new position as reference
            CheckCollisions(Position + Velocity);

            // Reset acceleration for next update
            Acceleration = Vector2.Lerp(Acceleration, Vector2.Zero, deltaTime * 10.0f);
        }
        private static bool Collides(Vector2 position) => GameManager.Instance.GameWorld[(int)(position.X), (int)(position.Y)].Id != World.Tiles.TileId.None;


        public bool RectangularCollision(Vector2 pos, Vector2 size)
        {
            // Calculate the top left and bottom right corners of the rectangular bounding box
            Vector2 topLeft = pos - size / 2;
            Vector2 bottomRight = pos + size / 2;

            // Check top left corner
            if (Collides(topLeft))
            {
                return true;
            }

            // Check top right corner
            if (Collides(new Vector2(bottomRight.X, topLeft.Y)))
            {
                return true;
            }

            // Check bottom left corner
            if (Collides(new Vector2(topLeft.X, bottomRight.Y)))
            {
                return true;
            }

            // Check bottom right corner
            if (Collides(bottomRight))
            {
                return true;
            }

            // No collisions at any of the corners, so return false
            return false;
        }

        private void CheckCollisions(Vector2 target)
        {
            if (RectangularCollision(new Vector2(target.X, Position.Y), GameManager.Instance.Player.Size))
            {
                Acceleration *= new Vector2(0, 1);
                target.X = Position.X;
            }

            if (RectangularCollision(new Vector2(Position.X, target.Y), GameManager.Instance.Player.Size))
            {
                var newAcceleration = new Vector2(xCollision ? 0 : 1, yCollision ? 0 : 1);
                Acceleration *= newAcceleration;
                target = new Vector2(xCollision ? Position.X : target.X, yCollision ? Position.Y : target.Y);
            }
            Position = target;
        }

        // Apply a force described by a Vector2 to the physics component
        public void ApplyForce(Vector2 force)
        {
            Acceleration += force / Mass;
        }


        public void Draw(float dt)
        {

        }

    }
}
