using Game2D.Entities;

namespace Game2D.PlayerState
{
    public class MovingPlayerState : PlayerState
    {
        public MovingPlayerState(PlayerStateManager manager)
            : base(manager)
        {
            AnimationName = "run";
        }

        public override void Update(PlayerEntity player, float dt)
        {
        }
    }
}