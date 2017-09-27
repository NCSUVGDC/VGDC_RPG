using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using VGDC_RPG;

public class mouseOver : MonoBehaviour {
    public Image im;

    public void mouseOverEffect(Sprite i) {
        im.sprite = i;
        GameLogic.mouseIsOverUI = true;
    }

    public void mouseOutEffect(Sprite i) {
        im.sprite = i;
        GameLogic.mouseIsOverUI = false;
    }
}
