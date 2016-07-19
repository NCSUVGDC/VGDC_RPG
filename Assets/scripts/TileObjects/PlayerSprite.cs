using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VGDC_RPG.TileObjects
{
    public class PlayerSprite : MonoBehaviour
    {
        private Texture2D[] IdleFramesFront;
        private Texture2D[] IdleFramesBack;
        private Texture2D[] IdleFramesLeft;
        private Texture2D[] IdleFramesRight;

        private Texture2D[] MovingFramesFront;
        private Texture2D[] MovingFramesBack;
        private Texture2D[] MovingFramesLeft;
        private Texture2D[] MovingFramesRight;

        public int X, Y;

        public int Direction;

        public float FramesPerSecond = 2;

        private float movementLerp = 0;

        private Material material;
        private TextMesh texmex;

        public string AssetName;

        public bool IsMoving;

        private float timer;
        private int frame;
        private string _name = string.Empty;

        private List<Int2> path;

        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            texmex = GetComponentInChildren<TextMesh>();
            texmex.text = _name;
            //SetSpriteSet("Grenadier");
        }

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
                switch (Direction)
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
                switch (Direction)
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

            if (path != null)
            {
                movementLerp += Time.deltaTime * 4.0f;
                if (movementLerp >= path.Count - 1)
                {
                    var l = path[path.Count - 1];
                    path = null;
                    X = l.X;
                    Y = l.Y;
                    transform.position = new Vector3(X + 0.5f, transform.position.y, Y + 0.5f);
                    IsMoving = false;
                }
                else
                {
                    int index = Mathf.FloorToInt(movementLerp);
                    if (path[index].X < path[index + 1].X)
                        LookRight();
                    else if (path[index].X > path[index + 1].X)
                        LookLeft();
                    else if (path[index].Y < path[index + 1].Y)
                        LookBack();
                    else
                        LookForward();
                    transform.position = Vector3.Lerp(new Vector3(path[index].X + 0.5f, transform.position.y, path[index].Y + 0.5f), new Vector3(path[index + 1].X + 0.5f, transform.position.y, path[index + 1].Y + 0.5f), movementLerp - index);
                }
            }
        }

        public void LookForward()
        {
            Direction = 0;
        }

        public void LookBack()
        {
            Direction = 1;
        }

        public void LookLeft()
        {
            Direction = 2;
        }

        public void LookRight()
        {
            Direction = 3;
        }

        public void SetSpriteSet(string assetName)
        {
            AssetName = assetName;

            int i = 0;
            var s = Resources.Load<TextAsset>("Idle_" + assetName).text.Split('\n');
            IdleFramesFront = LoadTextures(s, ref i);
            IdleFramesBack = LoadTextures(s, ref i);
            IdleFramesLeft = LoadTextures(s, ref i);
            IdleFramesRight = LoadTextures(s, ref i);

            i = 0;
            s = Resources.Load<TextAsset>("Walking_" + assetName).text.Split('\n');
            MovingFramesFront = LoadTextures(s, ref i);
            MovingFramesBack = LoadTextures(s, ref i);
            MovingFramesLeft = LoadTextures(s, ref i);
            MovingFramesRight = LoadTextures(s, ref i);
        }

        public void SetName(string name)
        {
            if (texmex != null)
                texmex.text = name;
            this._name = name;
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

        public void MoveOnPath(List<Int2> path)
        {
            movementLerp = 0;
            IsMoving = true;
            this.path = path;
        }
    }
}
