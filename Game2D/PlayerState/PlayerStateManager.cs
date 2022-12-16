using Game2D.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2D.PlayerState
{

    public class PlayerStateManager
    {
        private Dictionary<PlayerStates, PlayerState> playerStates;

        public PlayerState CurrentState { get; private set; }

        public PlayerStateManager()
        {
            playerStates = new Dictionary<PlayerStates, PlayerState>
            {
                { PlayerStates.Idle, new IdlePlayerState(this) },
                { PlayerStates.Moving, new MovingPlayerState(this) }
            };

            CurrentState = playerStates[PlayerStates.Idle];
        }

        public void ChangeState(PlayerStates state)
        {
            CurrentState = playerStates[state];
        }

        public void Update(PlayerEntity player, float dt)
        {
            CurrentState.Update(player, dt);
        }
    }
}
