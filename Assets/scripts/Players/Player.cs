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
        private List<Int2> movementPath = null;

        private Material material;

        public bool TakingTurn = false;

        // Use this for initialization
        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
        }

        // Update is called once per frame
        void Update()
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
                    GameLogic.Instance.NextPlayer();
                }
                else
                {
                    int index = Mathf.FloorToInt(movementLerp);
                    transform.position = Vector3.Lerp(new Vector3(movementPath[index].X + 0.5f, transform.position.y, movementPath[index].Y + 0.5f), new Vector3(movementPath[index + 1].X + 0.5f, transform.position.y, movementPath[index + 1].Y + 0.5f), movementLerp - index);
                }
            }
            else if (TakingTurn)
            {
                if (Input.GetMouseButtonDown(0))
                {

                    float x = Input.mousePosition.x;
                    float y = Input.mousePosition.y;



                    x -= GameLogic.Instance.Camera.GetComponent<Camera>().pixelWidth / 2.0f;
                    y -= GameLogic.Instance.Camera.GetComponent<Camera>().pixelHeight / 2.0f;
                    x /= 64.0f;
                    y /= 64.0f;
                    x += GameLogic.Instance.Camera.transform.position.x;
                    y += GameLogic.Instance.Camera.transform.position.z;


                    Debug.Log("MP: " + x + ", " + y);
                    Debug.Log("SS: " + Screen.width + ", " + Screen.height);

                    Int2 t = new Int2(Mathf.FloorToInt(x), Mathf.FloorToInt(y));
                    Debug.Log(t.X + ", " + t.Y);

                    if (possibleTiles.Contains(t))
                        Move(PathFinder.FindPath(GameLogic.Instance.Map, new Int2(X, Y), t));
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

            GameLogic.Instance.Camera.transform.position = new Vector3(X + 0.5f, 10, Y + 0.5f);
            possibleTiles = PathFinder.FindHighlight(GameLogic.Instance.Map, new Int2(X, Y), 8);
            foreach (var t in possibleTiles)
                GameLogic.Instance.Map.SelectedTile(t.X, t.Y);
            GameLogic.Instance.Map.ApplySelection();
        }
    }
}