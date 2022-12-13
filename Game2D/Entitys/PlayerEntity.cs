using Game2D.Animations;
using Game2D.Data;
using Game2D.Entitys.Components;
using Game2D.GameScreens.Graphics;
using Game2D.PlayerState;
using Game2D.Rendering;
using Game2D.World;
using ImGuiNET;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.Entitys
{
    public struct PlayerInformation
    {
        public float Speed { get; private set; }
        public bool IsMoving { get; set; }

        public PlayerInformation()
        {
            Speed = 5.0f;
            IsMoving = false;
        }
    }
    public class PlayerEntity : IEntity
    {
        #region Public Members
        public EntityPhysicsComponent PhysicsComponent { get; private set; }
        public EntityDrawableComponent DrawableComponent { get; private set; }
        public Vector2 Position { get => PhysicsComponent.Position; set => PhysicsComponent.Position = value; }
        public Vector2 Size { get; private set; } = new Vector2(0.7f);
        public Dictionary<Type, IEntityComponent> Components { get; set; }
        public ParticleSystemEntity Particle { get; private set; }
        public PlayerInformation Information = new PlayerInformation();
        #endregion

        #region Private members
        AnimationManager animationManager;
        PlayerStateManager manager;
        #endregion

        public PlayerEntity()
        {
            GameManager.Instance.Player = this;

            PhysicsComponent = new EntityPhysicsComponent(new Vector2(0, 0), Size, 1.0f);
            DrawableComponent = new EntityDrawableComponent(PhysicsComponent);

            manager = new PlayerStateManager();

            Components = new Dictionary<Type, IEntityComponent>()
            {
                {typeof(EntityPhysicsComponent), PhysicsComponent },
                {typeof(EntityDrawableComponent), DrawableComponent }
            };
            Particle = new ParticleSystemEntity(100) { MaximumAge = 5.0f, SpawnRate = 0.1f };
            initializeSprite();

            Position = new Vector2(16, 16);
        }

        private void initializeSprite()
        {
            var image = new Texture(new System.Drawing.Bitmap("Assets/Player/spritesheet.png"), false, false);
            animationManager = new AnimationManager(new Vector2(6, 2));

            DrawableComponent.Sprite = new Sprite(image, Size);

            animationManager.AddAnimation("run", new AnimationInformation(new Vector2i(0, 0), 5));
            animationManager.AddAnimation("idle", new AnimationInformation(new Vector2i(0, 1), 3));

            animationManager.SetCurrentAnimation("idle");
        }

        public void Draw(float dt)
        {
            var inputState = GameManager.Instance.InputManager.GetMovementAxis();

            DrawableComponent.Sprite.TextureRectangle = animationManager.GetCurrentAnimation().boundingBox;
            DrawableComponent.Draw(dt);
            Particle.Draw(dt);

            ImGui.Begin("Player Info");
            {
                ImGui.Text($"Player Position: {Position}");
                ImGui.Text($"Local Position: Chunk({(int)(Position.X / Chunk.Width)}) [{(int)(Position.X % Chunk.Width)}, {(int)(Position.Y)}] = {GameManager.Instance.GameWorld[(int)Position.X, (int)Position.Y].Id}");
                ImGui.Text($"Current Player State: {manager.CurrentState.ToString().Split('.').LastOrDefault()}");
                ImGui.Text($"IsMoving: {Information.IsMoving}");
                ImGui.Text($"IsOnGround: {PhysicsComponent.OnGround}");
                ImGui.Text($"Acceleration: {PhysicsComponent.Acceleration}");


                ImGui.End();
            }
        }

        float jumpCooldown = 0;

        public void moveUpdate(float dt)
        {
            bool onGround = GameManager.Instance.GameWorld[(int)Position.X, (int)(Position.Y - Size.Y / 2.0f)].Id != World.Tiles.TileId.None;
            Information.IsMoving = false;

            jumpCooldown += dt;

            if (onGround && jumpCooldown > 0.1f && (GameManager.Instance.KeyboardState.IsKeyDown(Keys.Space)))
            {
                jumpCooldown = 0;
                PhysicsComponent.ApplyForce(new Vector2(0, 2.5f));
            }

            bool collides(Vector2 position) => GameManager.Instance.GameWorld[(int)(position.X), (int)(position.Y)].Id != World.Tiles.TileId.None;

            var inputState = GameManager.Instance.InputManager.GetMovementAxis();

            PhysicsComponent.ApplyForce(inputState * Information.Speed * dt);

            // fuck you sprite isnt null
            DrawableComponent.Sprite.IsFlipped = inputState.X < 0.0f;


            if (!GameManager.Instance.InputManager.IsAnyActiveInput && PhysicsComponent.OnGround)
                manager.ChangeState(PlayerStates.Idle);
        }

        public void Update(float dt)
        {
            moveUpdate(dt);

            manager.Update(this, dt);
            Particle.Physics.Position = Position - new Vector2(0, Size.Y);
            Particle.Update(dt);

            animationManager.SetCurrentAnimation(manager.CurrentState.AnimationName);
            Particle.IsSpawning = Information.IsMoving && PhysicsComponent.OnGround;
            animationManager.Update(dt);

            GameManager.Instance.Camera.Position = new Vector3(Position.X, Position.Y, GameManager.Instance.Camera.Position.Z);

            foreach (var item in Components.Values)
                item.Update(dt);
        }
    }
}
