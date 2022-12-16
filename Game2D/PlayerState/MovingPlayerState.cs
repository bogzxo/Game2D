using Game2D.Data;
using Game2D.Entities;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

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
