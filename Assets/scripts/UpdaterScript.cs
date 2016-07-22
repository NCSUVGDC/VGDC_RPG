using UnityEngine;
using System.Collections;
using VGDC_RPG;
using VGDC_RPG.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class UpdaterScript : MonoBehaviour
{
    public Image MenuPanel, GraphicsPanel;
    public Dropdown ResDropdown;
    public Toggle FSToggle, VSToggle;
    public Text LoadingText;

    Resolution[] res;
    bool fullscreen;
    bool vsync;

    // Use this for initialization
    void Start()
    {
        res = Screen.resolutions;
        var rl = new List<string>();
        for (int i = 0; i < res.Length; i++)
            rl.Add(res[i].width + "x" + res[i].height + "@" + res[i].refreshRate);
        ResDropdown.AddOptions(rl);
        FSToggle.isOn = Screen.fullScreen;
        VSToggle.isOn = QualitySettings.vSyncCount == 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameLogic.Map == null && GameLogic.mapConstructionData != null)
        {
            GameLogic.BuildMap();
            LoadingText.gameObject.SetActive(false);
        }

        if (GameLogic.IsHost && GameLogic.IsServer)
            MatchServer.Update();
        else
            MatchClient.Update();

        if (Input.GetKeyDown(KeyCode.Escape))
            if (GraphicsPanel.gameObject.activeSelf)
                GraphicsPanel.gameObject.SetActive(false);
            else
                MenuPanel.gameObject.SetActive(!MenuPanel.gameObject.activeSelf);

        if (Input.GetMouseButtonDown(0))
        {
            GameLogic.ClickTile(GameLogic.GetScreenTile(Input.mousePosition.x, Input.mousePosition.y));
        }
    }

    //Menu Events
    public void GraphicsSettingsClicked()
    {
        MenuPanel.gameObject.SetActive(false);
        GraphicsPanel.gameObject.SetActive(true);
    }

    public void MainBackClicked()
    {
        MenuPanel.gameObject.SetActive(false);
    }

    public void GraphicsBackClicked()
    {
        GraphicsPanel.gameObject.SetActive(false);
    }

    public void VSyncChanged(bool state)
    {
        QualitySettings.vSyncCount = state ? 1 : 0;
    }

    public void FullscreenChanged(bool state)
    {
        Screen.fullScreen = state;
    }

    public void ResChanged(int state)
    {
        Screen.SetResolution(res[state].width, res[state].height, Screen.fullScreen, res[state].refreshRate);
    }
}
