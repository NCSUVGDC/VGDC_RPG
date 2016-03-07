using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using VGDC_RPG.Map;

namespace VGDC_RPG.Players
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Player : MonoBehaviour
    {
        /// <summary>
        /// An array of idle frames.
        /// </summary>
        public Texture2D[] IdleFrames;
        /// <summary>
        /// An array of moving frames.
        /// </summary>
        public Texture2D[] MovingFrames;
        /// <summary>
        /// The numbers of frames to cycle through per second.
        /// </summary>
        public float FramesPerSecond = 2;

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
        protected List<Int2> possibleTiles = null;
        protected List<Int2> movementPath = null;
        protected List<Int2> attackTiles = null;

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

        public GameObject Arrow;

        //=== Player Attributes ===
        public bool Ranged = false;
        public int Range = 0;

        public int ActionPoints = 2;
        public int MovementPerAction = 5;

        public int SelectedStone = 0;
        public bool StoneSelected = false;

        public int HitPoints = 25;
        public int BaseDamage = 5;
        public float DefenseReduction = 0.15f;
        public float AttackChance = 0.75f;

        public virtual int AttackDamage { get { return BaseDamage; } }
        public virtual string GUIName { get { return "Player"; } }
        //=========================

        // Use this for initialization
        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            texmex = GetComponentInChildren<TextMesh>();
            UpdateText();//texmex.text = GUIName;
            if (IdleFrames.Length != 0)
                material.mainTexture = IdleFrames[0];
            ComputeAttackTiles();//attackTiles = GameLogic.Instance.Map.GetNeighbors(new Int2(X, Y));
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
                    frame %= MovingFrames.Length;
                    material.mainTexture = MovingFrames[frame];
                }
                else
                {
                    frame %= IdleFrames.Length;
                    material.mainTexture = IdleFrames[frame];
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
                        ComputeAttackTiles();//attackTiles = GameLogic.Instance.Map.GetNeighbors(new Int2(X, Y));
                        GameLogic.Instance.NextTurn();
                    }
                    else
                    {
                        int index = Mathf.FloorToInt(movementLerp);
                        transform.position = Vector3.Lerp(new Vector3(movementPath[index].X + 0.5f, transform.position.y, movementPath[index].Y + 0.5f), new Vector3(movementPath[index + 1].X + 0.5f, transform.position.y, movementPath[index + 1].Y + 0.5f), movementLerp - index);
                    }
                }
            }
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
                        if (Map.Pathfinding.AStarSearch.Heuristic(new Int2(X, Y), new Int2(x, y)) <= Range &&
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

        /// <summary>
        /// Called at the start of the players turn.
        /// </summary>
        /// <param name="turn">The number of the turn being taken.</param>
        public virtual void Turn(int turn)
        {
            TakingTurn = true;
            if (turn == 1)
                Defending = false;
            
            //if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.Main)
            //{
            //    ComputePossibleMovementTiles();
            //}
        }

        protected void ComputePossibleMovementTiles()
        {
            possibleTiles = Map.Pathfinding.AStarSearch.FindHighlight(GameLogic.Instance.Map, new Int2(X, Y), MovementPerAction);//PathFinder.FindHighlight(GameLogic.Instance.Map, new Int2(X, Y), MovementPerAction);
            foreach (var t in possibleTiles)
                GameLogic.Instance.Map.SelectedTile(t.X, t.Y);
            GameLogic.Instance.Map.ApplySelection();
        }

        /// <summary>
        /// Attacks the given player.
        /// </summary>
        /// <param name="other">The player to recieve the attack.</param>
        public void Attack(Player other)
        {
            if (UnityEngine.Random.value <= AttackChance)
            {
                other.Damage(AttackDamage);
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
            texmex.text = GUIName + "\n" + HitPoints;
        }
    }
}