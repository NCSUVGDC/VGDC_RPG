using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnGUI()
    {
        //Get references to the gameManager and all relevant value objects.
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Text HPValue = GameObject.Find("HPValue").GetComponent<Text>();
        Text APValue = GameObject.Find("APValue").GetComponent<Text>();
        Text NameValue = GameObject.Find("NameValue").GetComponent<Text>();

        //Update text on UI elements to reflect the stats of the active character.
        Player currentPlayer = gameManager.players[gameManager.currentPlayerIndex];
        HPValue.text = currentPlayer.HP.ToString();
        APValue.text = currentPlayer.actionPoints.ToString();
        NameValue.text = gameManager.players[gameManager.currentPlayerIndex].playerName;

    }
}
