using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace VGDC_RPG
{
    public static class RTVs
    {
        public delegate void BuffersResizedDV();

        public static event BuffersResizedDV BuffersResized;

        public static RenderTexture MainRTV, LightsRTV, WarpRTV;

        public static void ResizeBuffers(Camera mainCam, Camera lightCam, Camera warpCam)
        {
            if (mainCam.targetTexture != null)
            {
                mainCam.targetTexture.Release();
                mainCam.targetTexture = null;
            }

            if (lightCam.targetTexture != null)
            {
                lightCam.targetTexture.Release();
                lightCam.targetTexture = null;
            }

            MainRTV = new RenderTexture(Screen.width, Screen.height, 16, RenderTextureFormat.ARGB32);
            LightsRTV = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGBHalf);
            WarpRTV = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.RGHalf);
            MainRTV.Create();
            LightsRTV.Create();
            WarpRTV.Create();
            mainCam.targetTexture = MainRTV;
            lightCam.targetTexture = LightsRTV;
            warpCam.targetTexture = WarpRTV;

            if (BuffersResized != null)
                BuffersResized();
        }
    }
}
