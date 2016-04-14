using UnityEngine;
using System.Collections;

namespace VGDC_RPG
{
    public class MergingScript : MonoBehaviour
    {
        private Material mat;

        // Use this for initialization
        void Start()
        {
            mat = GetComponent<MeshRenderer>().material;
            RTVs_BuffersResized();

            RTVs.BuffersResized += RTVs_BuffersResized;
        }

        private void RTVs_BuffersResized()
        {
            mat.SetTexture("_MainTex", RTVs.MainRTV);
            mat.SetTexture("_WarpTex", RTVs.WarpRTV);
            mat.SetTexture("_LightTex", RTVs.LightsRTV);
        }

        void OnDestroy()
        {
            RTVs.BuffersResized -= RTVs_BuffersResized;
        }
    }
}
