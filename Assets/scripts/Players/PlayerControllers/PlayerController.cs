using UnityEngine;

namespace VGDC_RPG.Players.PlayerControllers
{
    public class PlayerController : IPlayerController
    {
        private enum UserChoice
        {
            Choosing,
            Move,
            Defend,
            Attack,
            EndTurn
        }

        public Player Player { get; set; }
        private UserChoice choice;

        public void TurnStart()
        {
            choice = UserChoice.Choosing;
        }

        public void OnGUI()
        {
            if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.SelectingStones && GameLogic.Instance.DoPlayerUpdates)
            {
                var buttonHeight = 30;
                var buttonWidth = 120;
                GUI.Label(new Rect(0, Screen.height - Stones.COUNT * buttonHeight - 50, 120, 20), Player.GUIName);
                GUI.Label(new Rect(0, Screen.height - Stones.COUNT * buttonHeight - 20, 120, 20), "Select a stone:");
                for (int i = 0; i < Stones.COUNT; i++)
                    if (GUI.Button(new Rect(0, Screen.height - (Stones.COUNT - i) * buttonHeight, buttonWidth, buttonHeight), Stones.UIText[i + 1]))
                    {
                        Player.SelectedStone = i + 1;
                        Player.StoneSelected = true;
                    }
            }
            if (choice == UserChoice.Choosing && GameLogic.Instance.CurrentGameState == GameLogic.GameState.Main && GameLogic.Instance.DoPlayerUpdates)
            {
                var buttonHeight = 60;
                var buttonWidth = 100;
                if (Player.canMove && GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, Screen.height / 2 - buttonHeight * 2, buttonWidth, buttonHeight), "Move"))
                {
                    choice = UserChoice.Move;
                    foreach (var t in Player.possibleTiles)
                        GameLogic.Instance.Map.SelectedTile(t.X, t.Y);
                    GameLogic.Instance.Map.ApplySelection();
                }
                else if (Player.canAttack && GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, Screen.height / 2 - buttonHeight * 1, buttonWidth, buttonHeight), "Attack"))
                {
                    choice = UserChoice.Attack;
                    GameLogic.Instance.Map.ClearSelection();
                    foreach (var t in Player.attackTiles)
                        GameLogic.Instance.Map.SelectedTile(t.X, t.Y);
                    GameLogic.Instance.Map.ApplySelection();
                }
                else if (!Player.Defending && GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, Screen.height / 2 - buttonHeight * 0, buttonWidth, buttonHeight), "Defend"))
                    choice = UserChoice.Defend;
                else if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, Screen.height / 2 + buttonHeight * 1, buttonWidth, buttonHeight), "End Turn"))
                    choice = UserChoice.EndTurn;
            }
        }

        public void Update()
        {
            if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.SelectingStones && Player.StoneSelected)
            {
                Player.TakingTurn = false;
                GameLogic.Instance.NextPlayer();
                return;
            }

            switch (choice)
            {
                case UserChoice.Move:
                    if (Input.GetMouseButtonDown(0))
                    {

                        float x = Input.mousePosition.x;
                        float y = Input.mousePosition.y;


                        var t = GameLogic.Instance.GetScreenTile(x, y);

                        Debug.Log(t.X + ", " + t.Y);

                        if (Player.possibleTiles.Contains(t))
                            Player.Move(Map.Pathfinding.AStarSearch.FindPath/*Map.Pathfinding.JumpPointSearch.FindPath*/(GameLogic.Instance.Map, new Int2(Player.X, Player.Y), t/*, false*/));
                    }
                    break;
                case UserChoice.Attack:
                    if (Input.GetMouseButtonDown(0))
                    {

                        float x = Input.mousePosition.x;
                        float y = Input.mousePosition.y;

                        var t = GameLogic.Instance.GetScreenTile(x, y);
                        if (Player.attackTiles.Contains(t))
                        {
                            for (int i = 0; i < GameLogic.Instance.TeamCount; i++)
                            {
                                if (i == Player.TeamID)
                                    continue;
                                foreach (var p in GameLogic.Instance.Players[i])
                                    if (p.X == t.X && p.Y == t.Y)
                                    {
                                        if (!Player.Ranged)
                                        {
                                            Player.Attack(p);
                                            Player.TakingTurn = false;
                                            GameLogic.Instance.NextTurn();
                                        }
                                        else
                                        {
                                            var a = GameObject.Instantiate<GameObject>(Player.Arrow).GetComponent<Projectiles.Arrow>();
                                            a.StartPosition = new Vector3(Player.X + 0.5f, 3, Player.Y + 0.5f);
                                            a.TargetPosition = new Vector3(p.X + 0.5f, 3, p.Y + 0.5f);
                                            a.Owner = Player;
                                            a.Target = p;
                                        }
                                        return;
                                    }
                            }
                        }
                    }
                    if (Input.GetMouseButtonDown(2))
                    {
                        float x = Input.mousePosition.x;
                        float y = Input.mousePosition.y;

                        var t = GameLogic.Instance.GetScreenTile(x, y);
                        Debug.Log("RC: " + GameLogic.Instance.Map.ProjectileRayCast(new Vector2(Player.X + 0.5f, Player.Y + 0.5f), new Vector2(t.X + 0.5f, t.Y + 0.5f)));
                    }
                    break;
                case UserChoice.Defend:
                    Player.Defending = true;
                    Player.TakingTurn = false;
                    GameLogic.Instance.NextTurn();
                    break;
                case UserChoice.EndTurn:
                    Player.TakingTurn = false;
                    GameLogic.Instance.NextTurn();
                    break;
            }
        }
    }
}
