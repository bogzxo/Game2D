using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.Entitys.Components
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
        public Vector2 Gravity { get; set; } = new Vector2(0, -1.0f); // default value for gravity

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
            // Chech ground collision
            OnGround = collides(Position - new Vector2(0, Size.Y / 2));

            // Apply gravity
            if (!OnGround)
                _gravitySpeed = Vector2.Lerp(_gravitySpeed, Gravity, deltaTime * 0.01f);
            else _gravitySpeed = Vector2.Zero;
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
        bool collides(Vector2 position) => GameManager.Instance.GameWorld[(int)(position.X), (int)(position.Y)].Id != World.Tiles.TileId.None;

        private void CheckCollisions(Vector2 target)
        {
            if (collides(new Vector2(target.X, Position.Y)))
            {
                Acceleration *= new Vector2(0, 1);
                target.X = Position.X;
            }

            if (collides(new Vector2(Position.X, target.Y)))
            {
                Acceleration *= new Vector2(1, 0);
                target.Y = Position.Y;
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
