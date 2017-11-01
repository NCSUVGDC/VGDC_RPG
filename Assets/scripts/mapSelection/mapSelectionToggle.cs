using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class mapSelectionToggle : MonoBehaviour {

    public mapSelectScript mapSScript;

    private bool oldToggleStatus;

    public void MapToggleClicked(bool b) {
        if (b == oldToggleStatus) {
            return;
        } else {
            mapSScript.mapPreviewQuestionMark.text = "";
            if (b && this.gameObject.name == "Perlin Landscape") {
                mapSScript.mapPreview.sprite = Resources.Load<Sprite>("mapPreviews/plPreview");
                mapSScript.mapPreview.color = Color.white;
            } else if (b && this.gameObject.name == "Drunkwalk Cave") {
                mapSScript.mapPreview.sprite = Resources.Load<Sprite>("mapPreviews/dwcPreview");
                mapSScript.mapPreview.color = Color.white;
            }
            oldToggleStatus = this.gameObject.GetComponent<Toggle>().isOn;
        }
    }

}
