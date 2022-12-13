using Game2D.Entitys;

namespace Game2D.PlayerState
{
    public abstract class PlayerState
    {
        public string AnimationName { get; set; } = "idle";
        public PlayerStateManager Manager { get; protected set; }
        public PlayerState(PlayerStateManager manager)
        {
            Manager = manager;
        }

        public abstract void Update(PlayerEntity player, float dt);
    }
}
