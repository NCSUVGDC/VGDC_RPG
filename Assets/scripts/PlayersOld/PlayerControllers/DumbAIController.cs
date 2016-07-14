//using System.Collections.Generic;
//using UnityEngine;
//using VGDC_RPG.Items;

//namespace VGDC_RPG.Players.PlayerControllers
//{
//    public class DumbAIController : IPlayerController
//    {
//        public Player Player { get; set; }

//        private Player target;

//        public void OnGUI()
//        {
            
//        }

//        public void ActionStart()
//        {
//            if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.Main)
//            {
//                Debug.Log(Player.GUIName + ": " + Player.ActiveWeapon.GUIName);

//                UpdateTarget();

//                if (Player.HitPoints <= 10)
//                {
//                    Item ci = null;
//                    foreach (var i in Player.Inventory)
//                        if (i.UseEffect.HPAdd > 0)
//                            ci = i;
//                    if (ci == null)
//                        foreach (var i in Player.Inventory)
//                            if (i.UseEffect.HPRegen > 0)
//                                ci = i;
//                    if (ci == null)
//                        foreach (var i in Player.Inventory)
//                            if (i.UseEffect.TempHPAdd > 0)
//                                ci = i;
//                    if (ci != null)
//                    {
//                        Player.Inventory.Use(ci, Player);

//                        Player.TakingTurn = false;
//                        GameLogic.Instance.NextAction();
//                        return;
//                    }
//                }

//                if (Player.canAttack && target != null && Player.attackTiles.Contains(new Int2(target.X, target.Y)))//(Mathf.Abs(Player.X - target.X) + Mathf.Abs(Player.Y - target.Y) <= 1))
//                {
//                    Player.RemainingActionPoints = -1;
//                    /*if (!Player.Ranged)
//                    {
//                        Debug.Assert(target.HitPoints > 0);
//                        Debug.Assert(Player.HitPoints > 0);
//                        Player.Attack(target);
//                        Player.TakingTurn = false;
//                        GameLogic.Instance.NextAction();
//                    }
//                    else
//                    {
//                        var a = GameObject.Instantiate<GameObject>(Player.Arrow).GetComponent<Projectiles.Arrow>();
//                        Player.awaiting++;
//                        a.Damage = Player.GetAttackDamage(target);
//                        a.StartPosition = new Vector3(Player.X + 0.5f, 3, Player.Y + 0.5f);
//                        a.TargetPosition = new Vector3(target.X + 0.5f, 3, target.Y + 0.5f);
//                        a.Owner = Player;
//                    }*/
//                    Player.ActiveWeapon.Attack(Player, new Int2(target.X, target.Y));
//                    //Player.RemainingActionPoints = -1;
//                }
//                else if (Player.canMove)
//                {
//                    List<Int2> path = null;
//                    if (target != null)
//                        path = Map.Pathfinding.AStarSearch.FindPathBeside(GameLogic.Instance.Map, new Int2(Player.X, Player.Y), new Int2(target.X, target.Y));
//                    if (path != null)
//                    {
//                        for (int i = path.Count - 1; i >= 0; i--)
//                            if (Player.possibleTiles.Contains(path[i]))
//                            {
//                                var nr = path.GetRange(0, i + 1);
//                                Player.Move(nr);
//                                Debug.Log("Moving AI.");
//                                return;
//                            }
//                    }
//                    Player.TakingTurn = false;
//                    GameLogic.Instance.NextAction();
//                }
//                else
//                {
//                    Player.Defend();
//                    Player.TakingTurn = false;
//                    GameLogic.Instance.NextAction();
//                }
//            }
//            else if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.SelectingStones)
//            {
//                //TODO: AI would select its stone here.  Random for now.
//                Player.SelectedStone = Random.Range(1, Stones.COUNT + 1);
//                Player.StoneSelected = true;

//                Player.TakingTurn = false;
//                GameLogic.Instance.NextAction();
//            }
//        }

//        public void Update()
//        {
            
//        }

//        private void UpdateTarget()
//        {
//            float sd = float.MaxValue;
//            Player nt = null;
//            for (int i = 0; i < GameLogic.Instance.TeamCount; i++)
//            {
//                if (i == Player.TeamID)
//                    continue;
//                foreach (var p in GameLogic.Instance.Players[i])
//                {
//                    if (p.HitPoints <= 0)
//                        continue;
//                    var dx = Player.X - p.X;
//                    var dy = Player.Y - p.Y;
//                    var td = dx * dx + dy * dy;
//                    if (td < sd)
//                    {
//                        sd = td;
//                        nt = p;
//                    }
//                }
//            }

//            target = nt;
//        }
//    }
//}
