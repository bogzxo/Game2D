using Game2D.Animations;
using Game2D.Data;
using Game2D.Entities.Components;
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

namespace Game2D.Entities
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
        private AnimationManager _animationManager;
        private readonly PlayerStateManager _manager;
        #endregion

        public PlayerEntity()
        {
            GameManager.Instance.Player = this;

            PhysicsComponent = new EntityPhysicsComponent(new Vector2(0, 0), Size, 1.0f);
            DrawableComponent = new EntityDrawableComponent(PhysicsComponent);

            _manager = new PlayerStateManager();

            Components = new Dictionary<Type, IEntityComponent>()
            {
                {typeof(EntityPhysicsComponent), PhysicsComponent },
                {typeof(EntityDrawableComponent), DrawableComponent }
            };
            Particle = new ParticleSystemEntity(100) { MaximumAge = 5.0f, SpawnRate = 0.1f };
            InitializeSprite();

            Position = new Vector2(32, 38);
        }

        private void InitializeSprite()
        {
            var image = new Texture(new System.Drawing.Bitmap("Assets/Player/spritesheet.png"), false, false);
            _animationManager = new AnimationManager(new Vector2(6, 2));

            DrawableComponent.Sprite = new Sprite(image, Size);

            _animationManager.AddAnimation("run", new AnimationInformation(new Vector2i(0, 0), 5));
            _animationManager.AddAnimation("idle", new AnimationInformation(new Vector2i(0, 1), 3));

            _animationManager.SetCurrentAnimation("idle");
        }

        public void Draw(float dt)
        {
            var inputState = GameManager.Instance.InputManager.GetMovementAxis();

            DrawableComponent.Sprite!.TextureRectangle = _animationManager.GetCurrentAnimation().boundingBox;
            DrawableComponent.Draw(dt);
            Particle.Draw(dt);

            ImGui.Begin("Player Info");
            {
                ImGui.Text($"Player Position: {Position}");
                ImGui.Text($"Local Position: Chunk({(int)(Position.X / Chunk.Width)}) [{(int)(Position.X % Chunk.Width)}, {(int)(Position.Y)}] = {GameManager.Instance.GameWorld[(int)Position.X, (int)Position.Y].Id}");
                ImGui.Text($"Current Player State: {_manager.CurrentState.ToString()?.Split('.').LastOrDefault()}");
                ImGui.Text($"IsMoving: {Information.IsMoving}");
                ImGui.Text($"IsOnGround: {PhysicsComponent.OnGround}");
                ImGui.Text($"Acceleration: {PhysicsComponent.Acceleration}");


                ImGui.End();
            }
        }

        private float _jumpCooldown = 0;

        private void MoveUpdate(float dt)
        {
            bool onGround = GameManager.Instance.GameWorld[(int)Position.X, (int)(Position.Y - Size.Y / 2.0f)].Id != World.Tiles.TileId.None;
            Information.IsMoving = false;

            _jumpCooldown += dt;

            if (onGround && _jumpCooldown > 0.1f && (GameManager.Instance.KeyboardState.IsKeyDown(Keys.Space)))
            {
                _jumpCooldown = 0;
                PhysicsComponent.ApplyForce(new Vector2(0, 2.5f));
            }

            Vector2 inputState = GameManager.Instance.InputManager.GetMovementAxis();

            PhysicsComponent.ApplyForce(inputState * Information.Speed * dt);

            DrawableComponent.Sprite!.IsFlipped = inputState.X < 0.0f;


            if (!GameManager.Instance.InputManager.IsAnyActiveInput && PhysicsComponent.OnGround)
                _manager.ChangeState(PlayerStates.Idle);
        }

        public void Update(float dt)
        {
            MoveUpdate(dt);

            _manager.Update(this, dt);
            Particle.Physics.Position = Position - new Vector2(0, Size.Y);
            Particle.Update(dt);

            _animationManager.SetCurrentAnimation(_manager.CurrentState.AnimationName);
            Particle.IsSpawning = Information.IsMoving && PhysicsComponent.OnGround;
            _animationManager.Update(dt);

            GameManager.Instance.Camera.Position = new Vector3(Position.X, Position.Y, GameManager.Instance.Camera.Position.Z);

            foreach (IEntityComponent item in Components.Values)
                item.Update(dt);
        }
    }
}
