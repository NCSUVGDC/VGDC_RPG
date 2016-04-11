using UnityEngine;
using System.Collections.Generic;
using System;
using VGDC_RPG.Items;

namespace VGDC_RPG.Players
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Player : MonoBehaviour
    {
        /// <summary>
        /// An array of idle frames.
        /// </summary>
        private Texture2D[] IdleFramesFront;
        private Texture2D[] IdleFramesBack;
        private Texture2D[] IdleFramesLeft;
        private Texture2D[] IdleFramesRight;

        private Texture2D[] MovingFramesFront;
        private Texture2D[] MovingFramesBack;
        private Texture2D[] MovingFramesLeft;
        private Texture2D[] MovingFramesRight;


        private int direction;

        /// <summary>
        /// An array of moving frames.
        /// </summary>
        //public Texture2D[] MovingFrames;
        /// <summary>
        /// The numbers of frames to cycle through per second.
        /// </summary>
        public float FramesPerSecond = 2;

        public void Heal(int hp)
        {
            int h = Mathf.Clamp(hp, 0, MaxHitPoints - HitPoints);
            HitPoints += h;
            GameLogic.SpawnText(h.ToString(), X, Y, Color.green);
        }

        public void AddEffect(PlayerEffect useEffect)
        {
            ActiveEffects.Add(useEffect);
            useEffect.ApplyEffect(this);
        }

        /// <summary>
        /// The current x-coordinate of the player.
        /// </summary>
        public int X;
        /// <summary>
        /// The current y-coordinate of the player.
        /// </summary>
        public int Y;

        /// <summary>
        /// Whether or not the player currently has a path on which it is moving.
        /// </summary>
        public bool IsMoving
        {
            get
            {
                return movementPath != null;
            }
        }

        private int frame;
        private float timer;

        private float movementLerp = 0;
        internal List<Int2> possibleTiles = null;
        internal List<Int2> movementPath = null;
        internal List<Int2> attackTiles = null;

        private Material material;
        private TextMesh texmex;

        /// <summary>
        /// True if the player is currently taking a turn.
        /// </summary>
        public bool TakingTurn = false;
        /// <summary>
        /// True if the player is currently defending.
        /// </summary>
        public bool Defending = false;

        /// <summary>
        /// This players team ID number.
        /// </summary>
        public int TeamID = -1;

        internal int awaiting = 0;

        public GameObject Arrow;


        public bool canAttack, canMove;
        public PlayerControllers.IPlayerController PlayerController;

        //=== Player Attributes ===
        public virtual bool Ranged { get { return false; } }
        public virtual int Range { get { return 0; } }

        public int ActionPoints = 2;
        public int RemainingActionPoints;
        public virtual int MovementPerAction { get { return 5; } }

        public int SelectedStone = 0;
        public bool StoneSelected = false;

        public int HitPoints;
        public int EffectHitPoints;
        public int MaxHitPoints { get { return BaseMaxHitPoints + EffectHitPoints; } }

        public virtual int BaseDamage { get { return 5; } }
        public virtual float DefenseReduction { get { return 0.15f; } }
        public virtual float AttackChance { get { return 0.75f; } }

        public virtual int BaseMaxHitPoints { get { return 25; } }
        public virtual string GUIName { get { return "Player"; } }
        public virtual string AssetName { get { throw new Exception("No asset name for player."); } }


        public List<PlayerEffect> ActiveEffects = new List<PlayerEffect>();
        //=========================

        public Inventory Inventory = new Inventory();

        // Use this for initialization
        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            texmex = GetComponentInChildren<TextMesh>();
            HitPoints = MaxHitPoints;
            UpdateText();//texmex.text = GUIName;
            if (IdleFramesFront == null)
                LoadTextures();
            if (IdleFramesFront.Length != 0)
                material.mainTexture = IdleFramesFront[0];
            ComputeAttackTiles();//attackTiles = GameLogic.Instance.Map.GetNeighbors(new Int2(X, Y));

            Inventory.Add(new InstantHealthPotionItem()); //TODO: temporary.
            Inventory.Add(new HealingPotion());
        }

        private void LoadTextures()
        {
            int i = 0;
            var s = Resources.Load<TextAsset>("Idle_" + AssetName).text.Split('\n');
            IdleFramesFront = LoadTextures(s, ref i);
            IdleFramesBack = LoadTextures(s, ref i);
            IdleFramesLeft = LoadTextures(s, ref i);
            IdleFramesRight = LoadTextures(s, ref i);

            i = 0;
            s = Resources.Load<TextAsset>("Walking_" + AssetName).text.Split('\n');
            MovingFramesFront = LoadTextures(s, ref i);
            MovingFramesBack = LoadTextures(s, ref i);
            MovingFramesLeft = LoadTextures(s, ref i);
            MovingFramesRight = LoadTextures(s, ref i);
        }

        private Texture2D[] LoadTextures(string[] c, ref int i)
        {
            int n = int.Parse(c[i++]);
            var r = new Texture2D[n];
            for (int j = 0; j < n; j++)
            {
                //Debug.Log(c[i]);
                r[j] = Resources.Load<Texture2D>(c[i++].Trim());
            }
            return r;
        }

        /// <summary>
        /// Handles update of the player texture and movement.
        /// </summary>
        public virtual void Update()
        {
            if (!GameLogic.Instance.DoPlayerUpdates)
                return;

            if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.Main)
            {
                var dt = Time.deltaTime;
                timer += dt;
                if (timer >= 1 / FramesPerSecond)
                    frame++;
                while (timer >= 1 / FramesPerSecond)
                    timer -= 1 / FramesPerSecond;
                if (IsMoving)
                {
                    switch (direction)
                    {
                        case 0:
                            frame %= MovingFramesFront.Length;
                            material.mainTexture = MovingFramesFront[frame];
                            break;
                        case 1:
                            frame %= MovingFramesBack.Length;
                            material.mainTexture = MovingFramesBack[frame];
                            break;
                        case 2:
                            frame %= MovingFramesLeft.Length;
                            material.mainTexture = MovingFramesLeft[frame];
                            break;
                        case 3:
                            frame %= MovingFramesRight.Length;
                            material.mainTexture = MovingFramesRight[frame];
                            break;
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case 0:
                            frame %= IdleFramesFront.Length;
                            material.mainTexture = IdleFramesFront[frame];
                            break;
                        case 1:
                            frame %= IdleFramesBack.Length;
                            material.mainTexture = IdleFramesBack[frame];
                            break;
                        case 2:
                            frame %= IdleFramesLeft.Length;
                            material.mainTexture = IdleFramesLeft[frame];
                            break;
                        case 3:
                            frame %= IdleFramesRight.Length;
                            material.mainTexture = IdleFramesRight[frame];
                            break;
                    }
                }

                if (movementPath != null)
                {
                    movementLerp += Time.deltaTime * 4.0f;
                    if (movementLerp >= movementPath.Count - 1)
                    {
                        GameLogic.Instance.Map.UnblockTile(X, Y);
                        movementLerp = 0;
                        var l = movementPath[movementPath.Count - 1];
                        movementPath = null;
                        X = l.X;
                        Y = l.Y;
                        transform.position = new Vector3(X + 0.5f, transform.position.y, Y + 0.5f);
                        GameLogic.Instance.Map.BlockTile(X, Y);
                        TakingTurn = false;
                        //ComputeAttackTiles();//attackTiles = GameLogic.Instance.Map.GetNeighbors(new Int2(X, Y));
                        GameLogic.Instance.NextAction();
                    }
                    else
                    {
                        int index = Mathf.FloorToInt(movementLerp);
                        if (movementPath[index].X < movementPath[index + 1].X)
                            LookRight();
                        else if (movementPath[index].X > movementPath[index + 1].X)
                            LookLeft();
                        else if (movementPath[index].Y < movementPath[index + 1].Y)
                            LookBack();
                        else
                            LookForward();
                        transform.position = Vector3.Lerp(new Vector3(movementPath[index].X + 0.5f, transform.position.y, movementPath[index].Y + 0.5f), new Vector3(movementPath[index + 1].X + 0.5f, transform.position.y, movementPath[index + 1].Y + 0.5f), movementLerp - index);
                    }
                }
            }

            if (TakingTurn)
                PlayerController.Update();
        }

        public void LookForward()
        {
            direction = 0;
        }

        public void LookBack()
        {
            direction = 1;
        }

        public void LookLeft()
        {
            direction = 2;
        }

        public void LookRight()
        {
            direction = 3;
        }

        protected void ComputeAttackTiles()
        {
            if (!Ranged)
                attackTiles = GameLogic.Instance.Map.GetNeighbors(new Int2(X, Y));
            else
            {
                if (attackTiles == null)
                    attackTiles = new List<Int2>();
                else
                    attackTiles.Clear();
                for (int y = Math.Max(Y - Range, 0); y <= Math.Min(Y + Range, GameLogic.Instance.Map.Height - 1); y++)
                    for (int x = Math.Max(X - Range, 0); x <= Math.Min(X + Range, GameLogic.Instance.Map.Width - 1); x++)
                        if ((!GameLogic.Instance.Map.IsProjectileResistant(x, y) || GameLogic.Instance.Map.IsObjectOnTile(x, y)) &&
                            Map.Pathfinding.AStarSearch.Heuristic(new Int2(X, Y), new Int2(x, y)) <= Range &&
                                GameLogic.Instance.Map.ProjectileRayCast(new Vector2(X + 0.5f, Y + 0.5f), new Vector2(x + 0.5f, y + 0.5f)))
                            attackTiles.Add(new Int2(x, y));
            }
        }

        /// <summary>
        /// Moves the player along the given path of tiles.
        /// </summary>
        /// <param name="tiles">The path of tiles to move along.</param>
        public void Move(List<Int2> tiles)
        {
            if (tiles != null && tiles.Count != 0)
                movementPath = tiles;
        }

        public virtual void StartTurn()
        {
            Debug.Assert(HitPoints > 0);

            RemainingActionPoints = ActionPoints;
            Defending = false;

            List<PlayerEffect> tr = new List<PlayerEffect>();
            foreach (var e in ActiveEffects)
            {
                e.Turn(this);
                if (e.Duration <= 0)
                {
                    tr.Add(e);
                    e.RemoveEffect(this);
                }
            }

            foreach (var e in tr)
                ActiveEffects.Remove(e);
        }

        /// <summary>
        /// Called at the start of the players turn.
        /// </summary>
        public virtual void Action()
        {
            Debug.Assert(HitPoints > 0);

            TakingTurn = true;
            RemainingActionPoints--;

            ComputeAttackTiles();
            ComputePossibleMovementTiles();

            canMove = false;
            if (RemainingActionPoints > 0)
                foreach (var t in GameLogic.Instance.Map.GetNeighbors(new Int2(X, Y)))
                    if (GameLogic.Instance.Map.IsWalkable(t.X, t.Y))
                        canMove = true;

            canAttack = Ranged;//false;
            for (int i = 0; i < GameLogic.Instance.TeamCount; i++)
            {
                if (i == TeamID)
                    continue;
                foreach (var p in GameLogic.Instance.Players[i])
                    if (attackTiles != null && attackTiles.Contains(new Int2(p.X, p.Y)))
                    {
                        canAttack = true;
                        i = GameLogic.Instance.TeamCount;
                        break;
                    }
            }

            Debug.Assert(HitPoints > 0);

            PlayerController.ActionStart();

            //if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.Main)
            //{
            //    ComputePossibleMovementTiles();
            //}
        }

        internal void ComputePossibleMovementTiles()
        {
            possibleTiles = Map.Pathfinding.AStarSearch.FindHighlight(GameLogic.Instance.Map, new Int2(X, Y), MovementPerAction);//PathFinder.FindHighlight(GameLogic.Instance.Map, new Int2(X, Y), MovementPerAction);
            
        }

        /// <summary>
        /// Attacks the given player.
        /// </summary>
        /// <param name="other">The player to recieve the attack.</param>
        public void Attack(Player other)
        {
            Attack(other, GetAttackDamage(other));
        }

        public void Attack(Player other, int amount)
        {
            Debug.Assert(HitPoints > 0);

            //ActionPoints = 0;
            if (UnityEngine.Random.value <= AttackChance)
            {
                other.Damage(amount);
                GameLogic.PlayHit();
            }
            else
            {
                Debug.Log("Missed");
                GameLogic.SpawnText("Missed!", X, Y, Color.white);
            }
        }

        /// <summary>
        /// Recieve damage.
        /// </summary>
        /// <param name="amount">The amount of damage taken before damage reduction.</param>
        public void Damage(int amount)
        {
            Debug.Assert(HitPoints > 0);

            amount = Defending ? Mathf.FloorToInt(amount * (1 - DefenseReduction)) : amount;
            HitPoints -= amount;
            Debug.Log("Damaged for " + amount);
            GameLogic.SpawnText("-" + amount, X, Y, Color.red);
            if (HitPoints <= 0)
                Kill();
            else
                UpdateText();
        }

        /// <summary>
        /// Removes the player from the game.
        /// </summary>
        public void Kill()
        {
            GameLogic.Instance.Map.UnblockTile(X, Y);
            GameLogic.Instance.RemovePlayer(this);
            Destroy(gameObject);
        }

        private void UpdateText()
        {
            texmex.text = GUIName + "\n" + HitPoints + "\n" + TeamID;
        }

        void OnGUI()
        {
            if (TakingTurn)
                PlayerController.OnGUI();
        }

        public int GetAttackDamage(Player target)
        {
            return Mathf.FloorToInt(BaseDamage * Stones.Effectiveness[SelectedStone - 1, target.SelectedStone - 1]);
        }

        public void Defend()
        {
            Debug.Assert(HitPoints > 0);

            Defending = true;
            RemainingActionPoints = 0;
        }
    }
}