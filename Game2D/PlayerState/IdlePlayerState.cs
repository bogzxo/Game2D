using Game2D.Entities;

namespace Game2D.PlayerState
{
    public class IdlePlayerState : PlayerState
    {
        public IdlePlayerState(PlayerStateManager manager)
            : base(manager)
        {

        }

        public override void Update(PlayerEntity player, float dt)
        {
            if (GameManager.Instance.KeyboardState.IsAnyKeyDown || GameManager.Instance.InputManager.IsAnyActiveInput)
                Manager.ChangeState(PlayerStates.Moving);
        }
    }
}
