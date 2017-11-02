/**
 * The code in this class is not particularly efficient or well written. It was written under time
 * constraints and thus can be improved. This class is responsible for some of the Stone Selection
 * scene UI things. It has the code for the start, reset, and back buttons, and the code for
 * selecting characters then selecting stones. NOTE: stoneUpdaterScript is responsible for
 * populating the UI percentages and enabling the start button.
 * 
 * @author Andrew Karcher
 * */

using UnityEngine;
using VGDC_RPG;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class stoneSelectionUIActions : MonoBehaviour {

    /** the current active character - -1 means none are currently active.
     * 0 = warrior, 1 = grenadier, 2 = cleric, 3 = ranger */
    public int activeCharacter = -1;
    /** The banners are each its own button. This is the array of banner buttons */
    public Button[] characterArray;
    /** this is the array of the 4 stone buttons in the middle of the UI */
    public Button[] stoneArray;
    /** this array holds the transforms that the stones are "telepoorted" to when assigned */
    public Transform[] newStonePosArray;
    /**this is the holder for the half transparent disabled color used to indicate which banner is selectd*/
    private Color greyedOut;

    public void Start() {
        greyedOut = characterArray[0].colors.disabledColor;
    }

    /**
     * Loads next scene when start is ENABLED and pressed
     */
    public void startPressed() {
        SceneManager.LoadScene("scenes/mapSelect");
    }

    /**
     * resets the current stone assignments managed by GameLogic then reloads scene
     */
    public void resetPressed() {
        for(int i = 0; i < GameLogic.stoneArray.Length; i++) {
            GameLogic.stoneArray[i] = -1;
        }
        SceneManager.LoadScene("newStoneSelection");
    }

    /** 
     * just returns to main menu
     */
    public void backPressed() {
        SceneManager.LoadScene("scenes/newMainMenu");
    } 

    /**
     * this class looks for when a character is clicked that has no stone currently assigned to it.
     * basically, when a banner is selectable and someone clicks on it. It affects the colors as
     * needed, and enables all stone buttons while disabling other banner buttons.
     * 
     * @param g the banner button gameObject itself
     */    
    public void unassignedCharacterPressed(GameObject g) {
        for(int i = 0; i < characterArray.Length; i++) {
                characterArray[i].interactable = false;
        }

        for(int i = 0; i < stoneArray.Length; i++) {
            if (stoneArray[i].GetComponent<isAssignedBool>().isAssigned == false) {
                stoneArray[i].interactable = true;
            }
        }

        activeCharacter = g.GetComponent<idCard>().characterId;
        ColorBlock c = characterArray[activeCharacter].colors;
        c.disabledColor = greyedOut;
        for(int i = 0; i < stoneArray.Length; i++) {
            if(i != activeCharacter) {
                characterArray[i].colors = c;
            }
        }
        c.disabledColor = c.normalColor;
        characterArray[activeCharacter].colors = c;
    }

    /**
     * this enables all banner buttons that arent currently assigned when a stone is clicked.
     * it also disables all stone buttons and makes sure no banner is active.
     * 
     * @param g the stone button itself
     */
    public void stonePressed(GameObject g)
    {
        g.transform.position = newStonePosArray[activeCharacter].transform.position;
        characterArray[activeCharacter].GetComponent<isAssignedBool>().isAssigned = true;
        ColorBlock c = characterArray[activeCharacter].colors;
        c.disabledColor = Color.white;

        for (int i = 0; i < characterArray.Length; i++) {
            characterArray[i].colors = c;
            if (characterArray[i].GetComponent<isAssignedBool>().isAssigned == false) {
                characterArray[i].interactable = true;
            }
        }
        for(int i = 0; i < stoneArray.Length; i++)  {
            stoneArray[i].interactable = false;
        }
        g.GetComponent<isAssignedBool>().isAssigned = true;
        GameLogic.stoneArray[activeCharacter] = g.GetComponent<idCard>().characterId;

        activeCharacter = -1;
    }
}
