/*using System;
using UnityEngine;

namespace VGDC_RPG.Players.PlayerControllers
{
    class NetRemoteHostController : IPlayerController
    {
        public Player Player { get; set; }

        public enum ControlState
        {
            Waiting,
            Move,
        }

        public ControlState State;

        private int mx, my;

        public void OnGUI()
        {
            throw new NotImplementedException();
        }

        public void TurnStart()
        {
            Debug.Log("Remote player turn start.");
        }

        public void Update()
        {
            switch (State)
            {
                case ControlState.Move:
                    State = ControlState.Waiting;
                    Player.Move()
                    break;
            }
        }

        public void NetMove(int x, int y)
        {
            mx = x;
            my = y;
        }
    }
}
*/