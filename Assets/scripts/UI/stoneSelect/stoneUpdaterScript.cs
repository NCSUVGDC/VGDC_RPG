/**
 * This class checks to see when ready button should be enabled. It also reads in the text file
 * containing the percentages for the GUI in the stone selection scene. It updates the GUI as needed
 * and manages the 3d arrays containing all relevant data. 
 * 
 * As with stoneSelectionUIActions, this class probably is very inefficient and can easily be improved upon.
 * Many decisions were made for the sake of time.
 * 
 * @author Andrew Karcher
 */

using UnityEngine;
using System.Collections;
using System;
using VGDC_RPG;
using UnityEngine.UI;

public class stoneUpdaterScript : MonoBehaviour {

    /** this is responsible for determing if start button should be enabled */
    private bool readyToStart = false;
    /** reference to start button for enabling/disabling */
    public Button startButton;
    /** reference to grey start button */
    public Sprite notReadyButtonImage;
    /** reference to green start button */
    public Sprite readyButtonImage;
    /** reference to start button (?) */
    private Image startButtonImage;
    /** This contains the array of banner buttons. Reference helps enable all banner buttons at
     * start, and changes color of all to white when ready to start */
    public Button[] characterArray;
    /** reference of all stone buttons again - for disabling all at start */
    public Button[] stoneArray;
    /** IMPORTANT: this actually contains all of the text boxes for percent viewing in GUI. The order they are in
     * is how they appear if the entire hiearchy is expanded in scene view. the gameObjects for these each have ID cards
     * which link them to specific values in the 3d arrays below */
    public Text[] percentValues;
    /** IMPORANT: these are the meters that reflect the percentages above. They are also in this array in the same order
     * they are in the fully expanded hiearchy. Unlike percentValues, these have no ID cards but are simply inserted in 
     * the same order. This was done to save time. Their width value is directly equal to the % change each stone gives.
     * EX if a stone gives a +10 increase, the width of the appropriate Image in this array will be 10. */
    public Image[] percentBarImages;
    /** the 3d array containing each percentage for each possible stone combo for Warrior. dimensions are [stat][stone][team]. */
    public int[][][] warriorPercentageArray;
    /** the 3d array containing each percentage for each possible stone combo for Warrior. dimensions are [stat][stone][team]. */
    public int[][][] grenadierPercentageArray;
    /** the 3d array containing each percentage for each possible stone combo for Warrior. dimensions are [stat][stone][team]. */
    public int[][][] clericPercentageArray;
    /** the 3d array containing each percentage for each possible stone combo for Warrior. dimensions are [stat][stone][team]. */
    public int[][][] rangerPercentageArray;

    /**this just contains a reference to stoneSelectionUIActions */
    public stoneSelectionUIActions uithing;

    /**
     * this reads intializes the arrays and reads in the text file containing the correct values. Also ensures all is enabled,
     * nothing is assigned, and all percents and bars are at 0%.
     */
    void Start () {
        createPercentArrays();
        readTextValues();
        startButtonImage = startButton.GetComponent<Image>();
        for(int i = 0; i < GameLogic.stoneArray.Length; i++) {
            GameLogic.stoneArray[i] = -1;
        }
        for(int i = 0; i < percentValues.Length; i++) {
            percentValues[i].text = "";
        }
        for(int i = 0; i < percentBarImages.Length; i++) {
            percentBarImages[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        }
        for(int i = 0; i < characterArray.Length; i++) {
            characterArray[i].interactable = true;
        }
        for(int i = 0; i < stoneArray.Length; i++)
        {
            stoneArray[i].interactable = false;
        }
    }
	
	/**
     * this checks whether to enable the start button. PROBABLY A VERY INEFFICIENT METHOD.
     */
	void Update () {
        readyToStart = checkIfReady();
        if(readyToStart) {
            ColorBlock c = characterArray[0].colors;
            c.disabledColor = Color.white;
            for(int i = 0; i < characterArray.Length; i++) {
                characterArray[i].colors = c;
            }
            startButtonImage.sprite = readyButtonImage;
            startButton.interactable = true;
        } else {
            startButtonImage.sprite = notReadyButtonImage;
            startButton.interactable = false;
        }
	}

    /**
     * if all stones are assigned in GameLogic, then enable the start button.
     */
    bool checkIfReady() {
        bool ready = true;
        for(int i = 0; i < GameLogic.stoneArray.Length; i++) {
            if (GameLogic.stoneArray[i] == -1) {
                ready = false;
                break;
            }
        }
        return ready;
    }

    /**
     * this initializes all 4 3d arrays. It is called by start().
     */
    private void createPercentArrays() {
        warriorPercentageArray = new int[5][][];
        for(int i = 0; i < warriorPercentageArray.Length; i++) {
            warriorPercentageArray[i] = new int[4][];
            for(int j = 0; j < 4; j++) {
                warriorPercentageArray[i][j] = new int[2];
            }
        }
        grenadierPercentageArray = new int[5][][];
        for (int i = 0; i < grenadierPercentageArray.Length; i++) {
            grenadierPercentageArray[i] = new int[4][];
            for (int j = 0; j < 4; j++) {
                grenadierPercentageArray[i][j] = new int[2];
            }
        }
        clericPercentageArray = new int[3][][];
        for (int i = 0; i < clericPercentageArray.Length; i++) {
            clericPercentageArray[i] = new int[4][];
            for (int j = 0; j < 4; j++) {
                clericPercentageArray[i][j] = new int[2];
            }
        }
        rangerPercentageArray = new int[5][][];
        for (int i = 0; i < rangerPercentageArray.Length; i++) {
            rangerPercentageArray[i] = new int[4][];
            for (int j = 0; j < 4; j++) {
                rangerPercentageArray[i][j] = new int[2];
            }
        }
    }

    /**
     * IMPORTANT. this is the most complicated part. This method reads in the text file located in
     * assets/resources/textFileForUIPercents.txt.  More info for that file's formatting can be found
     * in the file itself. it starts at line 6 to skip the info at the start of that file.
     * */
    private void readTextValues() {
        string[] lines = Resources.Load<TextAsset>("textFileForUIPercents").text.Split('\n');
        //this is a reference to the current character array (warrior, grenadier, cleric, or ranger) that we are
        //currently reading values into the array for.
        int[][][] currentArr;

        //these values keep track of which character we are reading in, which stat we are  reading values for, and which
        //stone is having values read.
        int currentChar = 0;
        int currentStat = 0;
        int currentStone = 0;

        //for each line, if there is no number to start, we know to move to the next stone, or the next stat if all stones are read in.
        //this makes more sense if you look at the text file being read. If the line starts with a number, we break it into its two tokens
        //and add it to the current array. note it parses ints[1] then ints[0] - this is because in the UI, team 1 appears on the right
        for (int i = 6; i < lines.Length; i++) {
            currentArr = getCharacterArray(currentChar);
            if(!Char.IsDigit(lines[i][0])) {
                if (currentStone >= 4) {
                    currentStat++;
                    currentStone = 0;
                }
                if (currentStat >= currentArr.Length) {
                    currentChar++;
                    currentStat = 0;
                }
                continue;
            } else {
                string[] ints = lines[i].Split(' ');
                Debug.Log(currentChar + ", " + currentStat + ", " + currentStone);
                currentArr[currentStat][currentStone][0] = int.Parse(ints[1]);
                currentArr[currentStat][currentStone][1] = int.Parse(ints[0]);
                currentStone++;
            }
        }

    }

    /**
     * the previous method calls this one to update the reference to which character
     * is currently being read in. currentChar is the same value from previous method,
     * readTextValues.
     */
    private int[][][] getCharacterArray(int currentChar) {
        if (currentChar == 0) {
            return warriorPercentageArray;
        } else if (currentChar == 1) {
            return grenadierPercentageArray;
        } else if (currentChar == 2) {
            return clericPercentageArray;
        } else {
            return rangerPercentageArray;
        }
    }

    /**
     * when a button is moused over, call updatePercents. Due to the way buttons are enabled/disabled when clicked,
     * this also maintains the percentages in each banner when a stone is assigned - there is no need for a seperate
     * "Set percentages permanently when stone assigned" behavior.
     */
    public void buttonMouseOver(int stoneId) {
        if (uithing.activeCharacter == -1) {
            return;
        } else {
            updatePercents(stoneId);
        }

    }

    /**
     * when the stone is unassigned, and we mouse out of it, we want to reset all percent values for active character
     * back to 0.
     */
    public void buttonMouseOut() {
        if(uithing.activeCharacter == -1) {
            return;
        }
        int start = 0;
        int limit = percentValues.Length;
        if (uithing.activeCharacter == 0) {
            start = 0;
            limit = 10;
        } else if (uithing.activeCharacter == 1) {
            start = 10;
            limit = 20;
        } else if (uithing.activeCharacter == 2){ 
            start = 20;
            limit = 26;
        } else {
            start = 26;
            limit = 36;
        }
        for (int i = start; i < limit; i++) {
            percentValues[i].text = "";
            percentBarImages[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        }
    }

    /**
     * this updates the percents in the GUI, as well as the relative "meters" next to them. The seemingly random
     * varibles like 20, 26, 36, etc are just the parts in the array where we move betwen warrior meters, grenadier meters,
     * cleric meters, and ranger meters. This was done to save time as well.
     */
    private void updatePercents(int stoneId) {
        int start = 0;
        int limit = percentValues.Length;
        if (uithing.activeCharacter == 0) {
            start = 0;
            limit = 10;
        } else if (uithing.activeCharacter == 1) {
            start = 10;
            limit = 20;
        } else if (uithing.activeCharacter == 2) {
            start = 20;
            limit = 26;
        } else {
            start = 26;
            limit = 36;
        }

        //percentUpdater contains the unique ID cards for each text box. If we take the values from that ID card and 
        //plug them into our appropriate 3d array, it should change the values for that text box accordingly. This is a 
        //bit convoluted but was also done in the name of time.
        for (int i = start; i < limit; i++) {
            percentUpdater p = percentValues[i].gameObject.GetComponent<percentUpdater>();
            if (uithing.activeCharacter == 0) {
                percentValues[i].text = "+" + warriorPercentageArray[p.statId][stoneId - 1][p.teamId] + "%";
                //the width of each meter is equal to the percentage value. It just conveniently worked out that way.
                percentBarImages[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, warriorPercentageArray[p.statId][stoneId - 1][p.teamId]);
            } else if (uithing.activeCharacter == 1) {
                percentValues[i].text = "+" + grenadierPercentageArray[p.statId][stoneId - 1][p.teamId] + "%";
                percentBarImages[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, grenadierPercentageArray[p.statId][stoneId - 1][p.teamId]);
            } else if (uithing.activeCharacter == 2) {
                percentValues[i].text = "+" + clericPercentageArray[p.statId][stoneId - 1][p.teamId] + "%";
                percentBarImages[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clericPercentageArray[p.statId][stoneId - 1][p.teamId]);
            } else {
                percentValues[i].text = "+" + rangerPercentageArray[p.statId][stoneId - 1][p.teamId] + "%";
                percentBarImages[i].GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rangerPercentageArray[p.statId][stoneId - 1][p.teamId]);
            }
        }
    }
}
