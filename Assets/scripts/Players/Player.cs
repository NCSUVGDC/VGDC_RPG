using UnityEngine;
using System.Collections;

namespace VGDC_RPG.Players
{
    public class Player : MonoBehaviour
    {
        public Texture2D[] IdleFrames;
        public Texture2D[] MovingFrames;
        public float FramesPerSecond = 2;

        public int X, Y;

        private bool isMoving;

        private int frame;
        private float timer;

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
            if (isMoving)
            {
                frame %= MovingFrames.Length;
                material.mainTexture = MovingFrames[frame];
            }
            else
            {
                frame %= IdleFrames.Length;
                material.mainTexture = IdleFrames[frame];
            }
        }
    }
}