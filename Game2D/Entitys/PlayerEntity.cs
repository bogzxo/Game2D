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
        public PlayerState.PlayerState State { get => _manager.CurrentState; }
        #endregion

        #region Private members
        private AnimationManager _animationManager;
        private readonly PlayerStateManager _manager;
        #endregion

        public PlayerEntity()
        {
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

            Position = new Vector2((GameManager.Instance.GameWorld.Width * Chunk.Width) / 2.0f, Chunk.Height);
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
            DrawableComponent.Sprite!.TextureRectangle = _animationManager.GetCurrentAnimation().boundingBox;
            DrawableComponent.Draw(dt);
            Particle.Draw(dt);
        }

        private float _jumpCooldown = 0;

        private void MoveUpdate(float dt)
        {
            Information.IsMoving = false;

            _jumpCooldown += dt;

            if (PhysicsComponent.OnGround && _jumpCooldown > 0.1f && (GameManager.Instance.KeyboardState.IsKeyDown(Keys.Space)/* || GameManager.Instance.InputManager.State.IsButtonDown(1)*/))
            {
                _jumpCooldown = 0;
                PhysicsComponent.ApplyForce(new Vector2(0, 2.5f));
            }

            Vector2 inputState = GameManager.Instance.InputManager.Direction;

            PhysicsComponent.ApplyForce(inputState * Information.Speed * dt);

            DrawableComponent.Sprite!.IsFlipped = GameManager.Instance.InputManager.LastDirection.X < 0;


            if (!GameManager.Instance.InputManager.IsAnyActiveInput)
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
