using UnityEngine;

using VGDC_RPG;
using VGDC_RPG.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles updating systems in the match scene.
/// </summary>
public class UpdaterScript : MonoBehaviour {
    public Image MenuPanel, GraphicsPanel;
    public Dropdown ResDropdown;
    public Toggle FSToggle, VSToggle;
    public Text LoadingText;
    public GameObject aPanel;
    public Text gameOverText;
    public Button endGameButton;

    public Camera mainCam, lightCam, warpCam;

    Resolution[] res;

    bool initUI = false;
    bool isLoaded = false;

    void Start() {
        gameOverText.enabled = false;
        endGameButton.gameObject.SetActive(false);
        endGameButton.interactable = false;
        res = Screen.resolutions;
        var rl = new List<string>();
        for (int i = 0; i < res.Length; i++)
            rl.Add(res[i].width + "x" + res[i].height + "@" + res[i].refreshRate);
        ResDropdown.AddOptions(rl);
        FSToggle.isOn = Screen.fullScreen;
        VSToggle.isOn = QualitySettings.vSyncCount == 1;
        GameLogic.menuScript = aPanel.GetComponent<ActionPanelScript>();
    }

    // Update is called once per frame
    void Update() {
        InputManager.Update();

        if (GameLogic.Map == null) {
            GameLogic.BuildMap();
            return;
        } else if (!isLoaded) {
            LoadingText.gameObject.SetActive(false);
            GameLogic.SpawnUnits();
            isLoaded = true;
            return;
        } else if (isLoaded && !initUI) {
            GameLogic.InitUI();
            initUI = true;
            return;
        }

        if(GameLogic.gameOver == true) {
            endGameButton.gameObject.SetActive(true);
            endGameButton.interactable = true;
            gameOverText.enabled = true;
            gameOverText.text = "Player " + GameLogic.winner + " wins!";
        }

        var t = GameLogic.GetScreenTile(InputManager.MouseX, InputManager.MouseY);

        if (Input.GetKeyDown(KeyCode.Escape))
            if (GraphicsPanel.gameObject.activeSelf)
                GraphicsPanel.gameObject.SetActive(false);
            else
                MenuPanel.gameObject.SetActive(!MenuPanel.gameObject.activeSelf);

        if (InputManager.MouseDown)
            GameLogic.ClickTile(t);

        if (GameLogic.MatchInfo.PlayerInfos[GameLogic.CurrentPlayer].AIController != null && Time.frameCount % 60 == 0)
            GameLogic.MatchInfo.PlayerInfos[GameLogic.CurrentPlayer].AIController.Update();

        GameLogic.Map.SetSelection(t.X, t.Y);
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

    public void EffectsChanged(bool state)
    {
        if (state)
            RTVs.EnableEffects(mainCam, lightCam, warpCam);
        else
            RTVs.DisableEffects(mainCam, lightCam, warpCam);
    }

    public void EndGame() {
        SceneManager.LoadScene("scenes/newMainMenu");
    }
}
