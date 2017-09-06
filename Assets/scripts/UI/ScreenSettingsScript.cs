using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenSettingsScript : MonoBehaviour {
    
    public void fsToggle(bool b) {
        Screen.fullScreen = b;
    }

    public void vsToggle(bool b) {
        if (b)
            QualitySettings.vSyncCount = 1;
        else
            QualitySettings.vSyncCount = 0;
    }
    
    public void backButton() {
        SceneManager.LoadScene("newMainMenu");
    }

}
