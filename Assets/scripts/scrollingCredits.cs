using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class scrollingCredits : MonoBehaviour {

    public float scrollSpeed;
    private Transform objectToScroll;
    public float timeTillBackToMainMenu;
    private float timeElapsed;

    private void Start() {
        objectToScroll = this.gameObject.transform;
        timeElapsed = 0;
    }

    void Update() {
        if(Input.GetKeyDown("escape")) {
            SceneManager.LoadScene("scenes/newMainMenu");
        }
        timeElapsed += Time.deltaTime;
        objectToScroll.Translate(0.0f, scrollSpeed, 0.0f);
        if(timeElapsed >= timeTillBackToMainMenu) {
            SceneManager.LoadScene("scenes/newMainMenu");
        }
    }

}