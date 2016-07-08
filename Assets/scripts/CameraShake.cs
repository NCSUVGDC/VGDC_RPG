using UnityEngine;

namespace VGDC_RPG
{
    public class CameraShake : MonoBehaviour
    {
        private SimplexNoise noise = new SimplexNoise();
        public float Intensity = 0;
        public float MaxIntensity = 8;
        public float Dampener = 0.99f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Intensity > MaxIntensity)
                Intensity = MaxIntensity;
            if (Intensity < 0)
                Intensity = 0;
            transform.position = new Vector3(noise.Noise(Time.time * 5, 0, 0), 0, noise.Noise(Time.time * 5, 200, 0)) * Intensity;
            Intensity *= Dampener;
            if (Intensity < 0.05f)
                Intensity = 0;
        }
    }
}
