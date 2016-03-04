using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VGDC_RPG.Players
{
    public class UserPlayer : Player
    {
        public override void Turn()
        {
            base.Turn();
        }

        void OnGUI()
        {
            if (TakingTurn && GameLogic.Instance.CurrentGameState == GameLogic.GameState.SelectingStones)
            {
                var buttonHeight = 30;
                var buttonWidth = 120;
                GUI.Label(new Rect(0, Screen.height - Stones.COUNT * buttonHeight - 50, 120, 20), GUIName);
                GUI.Label(new Rect(0, Screen.height - Stones.COUNT * buttonHeight - 20, 120, 20), "Select a stone:");
                for (int i = 0; i < Stones.COUNT; i++)
                    if (GUI.Button(new Rect(0, Screen.height - (Stones.COUNT - i) * buttonHeight, buttonWidth, buttonHeight), Stones.UIText[i + 1]))
                    {
                        SelectedStone = i + 1;
                        StoneSelected = true;
                    }
            }
        }

        public override void Update()
        {
            if (this == GameLogic.Instance.CurrentPlayer && GameLogic.Instance.CurrentGameState == GameLogic.GameState.SelectingStones && StoneSelected)
            {
                TakingTurn = false;
                GameLogic.Instance.NextPlayer();
            }
            else
                base.Update();
        }
    }
}
