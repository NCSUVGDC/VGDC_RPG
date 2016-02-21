using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugUI : MonoBehaviour
{

    private GameManager gameManager;
    private Text healthValue;
    private Text characterName;
    private Text actionMode;

    // Use this for initialization
    void Start ()
    {
	    gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        healthValue = GameObject.Find("UI_HPValue").GetComponent<Text>();
        characterName = GameObject.Find("UI_CharacterName").GetComponent<Text>();
        actionMode = GameObject.Find("UI_ActionMode").GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log(gameManager);
        UserPlayer playerRef = (UserPlayer) gameManager.players[gameManager.currentPlayerIndex];
        healthValue.text = playerRef.HP.ToString();
        characterName.text = playerRef.playerName;

        //Assumes that only one of the moving, attacking, defending booleans are true at a time.
        //i
        if (playerRef.attacking)
        {
            actionMode.text = "Attacking...";
        }
        else if (playerRef.defending)
        {
            actionMode.text = "Defending.";
        }
        else if (playerRef.moving)
        {
            actionMode.text = "Movng...";
        }
        else
        {
            actionMode.text = "No action selected.";
        }
    }
    
}
