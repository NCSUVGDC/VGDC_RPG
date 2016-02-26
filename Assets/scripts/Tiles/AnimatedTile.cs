using UnityEngine;
using System.Collections;

namespace VGDC_RPG.Tiles
{
    public class AnimatedTile : Tile
    {
        public Texture2D[] TextureFrames;
        public float FramesPerSecond;

        private float timer;
        private int frame = 0;

        // Use this for initialization
        void Start()
        {
            material = GetComponent<MeshRenderer>().material;
            if (TextureFrames.Length > 0)
                material.SetTexture(0, TextureFrames[frame]);
            timer = 0;
        }

        // Update is called once per frame
        void Update()
        {
            timer += Time.deltaTime;
            if (TextureFrames.Length > 1 && timer > 1 / FramesPerSecond)
            {
                frame = (frame + 1) % TextureFrames.Length;
                timer = Mathf.Repeat(timer, 1 / FramesPerSecond);
                material.SetTexture(0, TextureFrames[frame]);
            }
        }
    }
}