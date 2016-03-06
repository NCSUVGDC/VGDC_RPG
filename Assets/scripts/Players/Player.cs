using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using VGDC_RPG.Map;

namespace VGDC_RPG.Players
{
    public class Player : MonoBehaviour
    {
        public Texture2D[] IdleFrames;
        public Texture2D[] MovingFrames;
        public float FramesPerSecond = 2;

        public int X, Y;

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

        public bool TakingTurn = false;

        public int TeamID = -1;

        //=== Player Attributes ===
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
            if (IdleFrames.Length != 0)
                material.mainTexture = IdleFrames[0];
            attackTiles = GameLogic.Instance.Map.GetNeighbors(new Int2(X, Y));
        }

        // Update is called once per frame
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
                        attackTiles = GameLogic.Instance.Map.GetNeighbors(new Int2(X, Y));
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

        public void Move(List<Int2> tiles)
        {
            if (tiles != null && tiles.Count != 0)
            {
                movementPath = tiles;
                movementPath.Insert(0, new Int2(X, Y));
            }
        }

        public virtual void Turn()
        {
            TakingTurn = true;
            
            if (GameLogic.Instance.CurrentGameState == GameLogic.GameState.Main)
            {
                possibleTiles = Map.Pathfinding.AStarSearch.FindHighlight(GameLogic.Instance.Map, new Int2(X, Y), MovementPerAction);//PathFinder.FindHighlight(GameLogic.Instance.Map, new Int2(X, Y), MovementPerAction);
                foreach (var t in possibleTiles)
                    GameLogic.Instance.Map.SelectedTile(t.X, t.Y);
                GameLogic.Instance.Map.ApplySelection();
            }
        }

        public void Attack(Player other)
        {
            if (UnityEngine.Random.value <= AttackChance)
            {
                other.Damage(AttackDamage);
            }
            else
                Debug.Log("Missed");
        }

        public void Damage(int amount)
        {
            HitPoints -= amount;
            Debug.Log("Damaged for " + amount);
            if (HitPoints <= 0)
                Kill();
        }

        public void Kill()
        {
            GameLogic.Instance.Map.UnblockTile(X, Y);
            GameLogic.Instance.RemovePlayer(this);
            Destroy(gameObject);
        }
    }
}