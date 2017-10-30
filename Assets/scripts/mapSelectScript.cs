using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class mapSelectScript : MonoBehaviour {

	public void backClicked() {
        SceneManager.LoadScene("scenes/stoneSelection");
    }
}
