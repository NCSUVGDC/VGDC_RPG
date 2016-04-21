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
            Attacking,
            Item,
            EndTurn
        }

        public Player Player { get; set; }
        private UserChoice choice;

        public void ActionStart()
        {
            choice = UserChoice.Choosing;
        }

        public void OnGUI()
        {
            var buttonHeight = 60;
            var buttonWidth = 100;
            if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.SelectingStones && GameLogic.Instance.DoPlayerUpdates)
            {
                buttonHeight = 30;
                buttonWidth = 120;
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
               
                if (Player.canMove && GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, Screen.height / 2 - buttonHeight * 2, buttonWidth, buttonHeight), "Move"))
                {
                    choice = UserChoice.Move;
                    foreach (var t in Player.possibleTiles)
                        GameLogic.Instance.Map.SelectTile(t.X, t.Y, 1);
                    GameLogic.Instance.Map.ApplySelection();
                }
                else if (Player.canAttack && GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, Screen.height / 2 - buttonHeight * 1, buttonWidth, buttonHeight), "Attack"))
                {
                    choice = UserChoice.Attack;
                    GameLogic.Instance.Map.ClearSelection();
                    foreach (var t in Player.attackTiles)
                        GameLogic.Instance.Map.SelectTile(t.X, t.Y, 2);
                    GameLogic.Instance.Map.ApplySelection();
                }
                else if (!Player.Defending && GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, Screen.height / 2 - buttonHeight * 0, buttonWidth, buttonHeight), "Defend"))
                    choice = UserChoice.Defend;
                else if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, Screen.height / 2 + buttonHeight * 1, buttonWidth, buttonHeight), "Item"))
                    choice = UserChoice.Item;
                else if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, Screen.height / 2 + buttonHeight * 2, buttonWidth, buttonHeight), "End Turn"))
                    choice = UserChoice.EndTurn;
            }
            if ((choice == UserChoice.Attack || choice == UserChoice.Move || choice == UserChoice.Item) && !Player.IsMoving && GameLogic.Instance.DoPlayerUpdates)
            {
                if (GUI.Button(new Rect(Screen.width - 100, Screen.height - 30, 100, 30), "Cancel"))
                {
                    choice = UserChoice.Choosing;
                    GameLogic.Instance.Map.ClearSelection();
                }
            }
            if ((choice == UserChoice.Item) && GameLogic.Instance.DoPlayerUpdates)
                for (int i = 0; i < Player.Inventory.Count; i++)
                    if (GUI.Button(new Rect((Screen.width - buttonWidth) / 2f, buttonHeight * i, buttonWidth, buttonHeight), Player.Inventory[i].GUIName))
                    {
                        if (Player.Inventory[i].RequiresAction)
                            choice = UserChoice.EndTurn;
                        else
                            choice = UserChoice.Choosing;
                        Player.Inventory.Use(Player.Inventory[i], Player);
                        return;
                    }

                            
                        
        }

        Int2 lht;
        public void Update()
        {
            if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.SelectingStones && Player.StoneSelected)
            {
                Player.TakingTurn = false;
                GameLogic.Instance.NextPlayer();
                return;
            }

            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;

            var t = GameLogic.Instance.GetScreenTile(x, y);
            switch (choice)
            {
                case UserChoice.Move:
                    if (Input.GetMouseButtonDown(0))
                    {
                        Debug.Log(t.X + ", " + t.Y);

                        if (Player.possibleTiles.Contains(t))
                            Player.Move(Map.Pathfinding.AStarSearch.FindPath/*Map.Pathfinding.JumpPointSearch.FindPath*/(GameLogic.Instance.Map, new Int2(Player.X, Player.Y), t/*, false*/));
                    }
                    break;
                case UserChoice.Attack:
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (Player.attackTiles.Contains(t))
                        {
                            /*if (Player.Ranged)
                            {
                                GameLogic.Instance.Map.ClearSelection();
                                var a = GameObject.Instantiate<GameObject>(Player.Arrow).GetComponent<Projectiles.Arrow>();
                                Player.awaiting++;
                                var target = GameLogic.GetPlayerOnTile(t.X, t.Y);
                                if (target != null)
                                    a.Damage = Player.GetAttackDamage(target);
                                a.StartPosition = new Vector3(Player.X + 0.5f, 3, Player.Y + 0.5f);
                                a.TargetPosition = new Vector3(t.X + 0.5f, 3, t.Y + 0.5f);
                                a.Owner = Player;
                                choice = UserChoice.Attacking;
                                Player.RemainingActionPoints = 0;
                                return;
                            }

                            for (int i = 0; i < GameLogic.Instance.TeamCount; i++)
                            {
                                if (i == Player.TeamID)
                                    continue;
                                foreach (var p in GameLogic.Instance.Players[i])
                                    if (p.X == t.X && p.Y == t.Y)
                                    {
                                        Player.RemainingActionPoints = 0;
                                        if (!Player.Ranged)
                                        {
                                            Player.Attack(p);
                                            Player.TakingTurn = false;
                                            GameLogic.Instance.NextAction();
                                        }
                                        return;
                                    }
                            }*/

                            choice = UserChoice.Attacking;
                            if (!Player.ActiveWeapon.Attack(Player, t))
                                choice = UserChoice.Attack;
                        }
                    }
                    else
                    {
                        if (lht == t)
                            break;
                        GameLogic.Instance.Map.ClearSelection();
                        foreach (var tile in Player.attackTiles)
                            GameLogic.Instance.Map.SelectTile(tile.X, tile.Y, 3);

                        if (Player.attackTiles.Contains(t))
                            for (int ty = Mathf.Max(0, t.Y - /*Player.SplashRange -- TODO*/3); ty <= Mathf.Min(GameLogic.Instance.Map.Height - 1, t.Y + 3); ty++)
                                for (int tx = Mathf.Max(0, t.X - /*Player.SplashRange -- TODO*/3); tx <= Mathf.Min(GameLogic.Instance.Map.Width - 1, t.X + 3); tx++)
                                    if (Constants.GetPDamage(new Int2(tx, ty), new Vector2(t.X + 0.5f, t.Y + 0.5f), 3) > 0)
                                        GameLogic.Instance.Map.SelectTile(tx, ty, 2);
                        GameLogic.Instance.Map.ApplySelection();
                    }
                    break;
                case UserChoice.Defend:
                    Player.Defend();
                    Player.TakingTurn = false;
                    GameLogic.Instance.NextAction();
                    break;
                case UserChoice.EndTurn:
                    Player.TakingTurn = false;
                    Player.RemainingActionPoints = 0;
                    GameLogic.Instance.NextAction();
                    break;
            }

            if (Input.GetMouseButtonDown(2))
            {
                Debug.Log("RC: " + GameLogic.Instance.Map.ProjectileRayCast(new Vector2(Player.X + 0.5f, Player.Y + 0.5f), new Vector2(t.X + 0.5f, t.Y + 0.5f)));
            }

            lht = t;
        }
    }
}
