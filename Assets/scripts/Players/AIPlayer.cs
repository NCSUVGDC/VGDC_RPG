namespace VGDC_RPG.Players
{
    public class AIPlayer : Player
    {
        public override string GUIName
        {
            get
            {
                return "Enemy";
            }
        }

        public override int MaxHitPoints
        {
            get
            {
                return 50;
            }
        }

        /*private Player target;

        public override void Turn(int turn)
        {
            base.Turn(turn);

            if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.Main)
            {
                UpdateTarget();

                if (target != null && (Math.Abs(X - target.X) + Math.Abs(Y - target.Y) <= 1))
                    Attack(target);
                else
                {
                    List<Int2> path = null;
                    if (target != null)
                        path = Map.Pathfinding.AStarSearch.FindPathBeside(GameLogic.Instance.Map, new Int2(X, Y), new Int2(target.X, target.Y));
                    if (path != null)
                    {
                        ComputePossibleMovementTiles();
                        for (int i = path.Count - 1; i >= 0; i--)
                            if (possibleTiles.Contains(path[i]))
                            {
                                var nr = path.GetRange(0, i + 1);
                                Move(nr);
                                Debug.Log("Moving AI.");
                                return;
                            }
                    }
                }
            }
            else if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.SelectingStones)
            {
                //TODO: AI would select its stone here.  Random for now.
                SelectedStone = UnityEngine.Random.Range(1, Stones.COUNT + 1);
                StoneSelected = true;
            }

            TakingTurn = false;
            GameLogic.Instance.NextTurn();
        }

        private void UpdateTarget()
        {
            float sd = float.MaxValue;
            Player nt = null;
            for (int i = 0; i < GameLogic.Instance.TeamCount; i++)
            {
                if (i == TeamID)
                    continue;
                foreach (var p in GameLogic.Instance.Players[i])
                {
                    var dx = X - p.X;
                    var dy = Y - p.Y;
                    var td = dx * dx + dy * dy;
                    if (td < sd)
                    {
                        sd = td;
                        nt = p;
                    }
                }
            }

            target = nt;
        }*/
    }
}
