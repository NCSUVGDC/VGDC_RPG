using UnityEngine;
using System.Collections;
using VGDC_RPG;
using UnityEngine.UI;

public class TileBarScript : MonoBehaviour
{
    RectTransform rt;
    public Text LayerText;
    public Slider LayerSlider;

    // Use this for initialization
    void Start()
    {
        rt = GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(85, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (InputManager.InEditMode && rt.anchoredPosition.x > 0)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x - 1, 0);
        if (!InputManager.InEditMode && rt.anchoredPosition.x < 85)
            rt.anchoredPosition = new Vector2(rt.anchoredPosition.x + 1, 0);
    }

    public void EmptyChanged(bool v)
    {
        if (v)
            TileSelected(0);
    }

    public void GrassChanged(bool v)
    {
        if (v)
            TileSelected(1);
    }

    public void StoneChanged(bool v)
    {
        if (v)
            TileSelected(2);
    }

    public void LampChanged(bool v)
    {
        if (v)
            TileSelected(3);
    }

    public void WaterChanged(bool v)
    {
        if (v)
            TileSelected(4);
    }

    public void WoodChanged(bool v)
    {
        if (v)
            TileSelected(20);
    }

    public void GrassDecChanged(bool v)
    {
        if (v)
            TileSelected(21);
    }

    public void TileSelected(ushort id)
    {
        GameLogic.Map.TileIDToSet = id;
    }

    public void LayerSelected(float layer)
    {
        GameLogic.Map.TileLayerToSet = (int)layer;
        LayerText.text = "Layer: " + GameLogic.Map.TileLayerToSet;
        LayerSlider.maxValue = GameLogic.Map.Layers.Length;
    }
}
