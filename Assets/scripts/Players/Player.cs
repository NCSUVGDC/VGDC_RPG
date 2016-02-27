using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        private List<Int2> movementPath = null;

        private Material material;

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
                }
                else
                {
                    int index = Mathf.FloorToInt(movementLerp);
                    transform.position = Vector3.Lerp(new Vector3(movementPath[index].X + 0.5f, transform.position.y, movementPath[index].Y + 0.5f), new Vector3(movementPath[index + 1].X + 0.5f, transform.position.y, movementPath[index + 1].Y + 0.5f), movementLerp - index);
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
    }
}