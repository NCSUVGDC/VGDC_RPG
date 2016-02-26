using UnityEngine;
using System.Collections;

namespace VGDC_RPG.Tiles
{
    public class Tile : MonoBehaviour
    {
        public bool Obstacle;
        public Color NormalColor = Color.white;
        public Color HighlightColor = new Color(0.5f, 0.5f, 1.0f);
        public Color SelectedColor = new Color(1.0f, 0.5f, 0.5f);

        public bool ObjectOnTile { get; set; }

        public bool Walkable
        {
            get { return !Obstacle && !ObjectOnTile; }
        }

        private TileState state;
        public TileState State
        {
            get { return state; }
            set
            {
                state = value;
                switch (state)
                {
                    case TileState.Normal:
                        material.color = NormalColor;
                        break;
                    case TileState.Highlighted:
                        material.color = HighlightColor;
                        break;
                    case TileState.Selected:
                        material.color = SelectedColor;
                        break;
                }
            }
        }

        public int MovementCost = 1;

        public int X, Y;

        protected Material material;

        // Use this for initialization
        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}