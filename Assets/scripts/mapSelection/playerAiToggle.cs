using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class playerAiToggle : MonoBehaviour {

    public mapSelectScript mapSScript;
    public ToggleGroup localGroup;
    private bool oldStatus;

    public void playerTypeToggleClicked(bool b) {
        if (b == oldStatus) {
            return;
        } else {
            if (b && this.gameObject.name == "Player") {
                mapSScript.playerCount++;
            } else if (!b && this.gameObject.name == "Player") {
                mapSScript.playerCount--;
            } else if (b && this.gameObject.name == "ai") {
                mapSScript.aiCount++;
            } else if (!b && this.gameObject.name == "ai") {
                mapSScript.aiCount--;
            }
            oldStatus = this.gameObject.GetComponent<Toggle>().isOn;
        }
    }
}
