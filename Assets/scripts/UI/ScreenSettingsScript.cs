using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public void mouseOver(GameObject t) {
        if (t.GetComponent<Text>()) {
            t.GetComponent<Text>().color = Color.grey;
        } else if (t.GetComponent<Image>()) {
            t.GetComponent<Image>().color = Color.grey;
        }
    }

    public void mouseOut(GameObject t) {
        if (t.GetComponent<Text>()) {
            t.GetComponent<Text>().color = Color.white;
        } else if (t.GetComponent<Image>()) {
            t.GetComponent<Image>().color = Color.white;
        }
    }

}
