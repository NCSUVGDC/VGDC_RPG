using UnityEngine;

namespace VGDC_RPG
{
    public static class RTVs
    {
        public delegate void BuffersResizedDV();

        public static event BuffersResizedDV BuffersResized;

        public static RenderTexture MainRTV, LightsRTV, WarpRTV;
        public static int Width { get; private set; }
        public static int Height { get; private set; }

        public static void ResizeBuffers(Camera mainCam, Camera lightCam, Camera warpCam)
        {
            if (mainCam.targetTexture != null)
            {
                var t = mainCam.targetTexture;
                mainCam.targetTexture = null;
                t.Release();
            }

            if (lightCam.targetTexture != null)
            {
                var t = lightCam.targetTexture;
                lightCam.targetTexture = null;
                t.Release();
            }

            if (warpCam.targetTexture != null)
            {
                var t = warpCam.targetTexture;
                warpCam.targetTexture = null;
                t.Release();
            }

            MainRTV = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            LightsRTV = new RenderTexture(Screen.width / 4, Screen.height / 4, 0, RenderTextureFormat.ARGBHalf);
            WarpRTV = new RenderTexture(Screen.width / 4, Screen.height / 4, 0, RenderTextureFormat.RGHalf);
            MainRTV.Create();
            LightsRTV.Create();
            WarpRTV.Create();
            mainCam.targetTexture = MainRTV;
            lightCam.targetTexture = LightsRTV;
            warpCam.targetTexture = WarpRTV;

            Width = Screen.width;
            Height = Screen.height;

            if (BuffersResized != null)
                BuffersResized();
        }

        private static bool _effectsEnabled = true;
        public static bool EffectsEnabled
        {
            get { return _effectsEnabled; }
        }

        public static void DisableEffects(Camera mainCam, Camera lightCam, Camera warpCam)
        {
            mainCam.targetTexture = null;
            mainCam.cullingMask = 16385;
            lightCam.enabled = false;
            warpCam.enabled = false;
            GameObject.Find("Merging Camera").GetComponent<Camera>().enabled = false;
            _effectsEnabled = false;
        }

        public static void EnableEffects(Camera mainCam, Camera lightCam, Camera warpCam)
        {
            mainCam.targetTexture = MainRTV;
            mainCam.cullingMask = 1;
            lightCam.enabled = true;
            warpCam.enabled = true;
            GameObject.Find("Merging Camera").GetComponent<Camera>().enabled = true;
            _effectsEnabled = true;
        }
    }
}
