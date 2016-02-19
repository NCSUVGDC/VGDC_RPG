/// © 2015  Individual Contributors. All Rights Reserved.
/// Contributors were members of the Video Game Development Club at North Carolina State University.
/// File Contributors: ?


using UnityEngine;
using System.Collections;

public class UserPlayer : Player {
	

	
	// Update is called once per frame
	void Update () {
		if (GameManager.instance.players[GameManager.instance.currentPlayerIndex] == this) {
			transform.GetComponent<Renderer>().material.color = Color.green;
		} else {
			transform.GetComponent<Renderer>().material.color = Color.white;
		}
		
		if (HP <= 0) {
			transform.rotation = Quaternion.Euler(new Vector3(90,0,0));
			transform.GetComponent<Renderer>().material.color = Color.red;
		}
	}
	
	public override void TurnUpdate ()
	{
		//highlight
		//
		//
		
		if (positionQueue.Count > 0) {
			transform.position += (positionQueue[0] - transform.position).normalized * moveSpeed * Time.deltaTime;
			
			if (Vector3.Distance(positionQueue[0], transform.position) <= 0.1f) {
				transform.position = positionQueue[0];
				positionQueue.RemoveAt(0);
				if (positionQueue.Count == 0) {
					actionPoints--;
				}
			}
			
		}
		
		base.TurnUpdate ();
	}
	
	public override void TurnOnGUI () {
        // Ensure current player is not defending on next player's turn
        defending = false;
        float buttonHeight = 50;
		float buttonWidth = 150;
		
		Rect buttonRect = new Rect(0, Screen.height - buttonHeight * 4, buttonWidth, buttonHeight);
		
		
		//move button
		if (GUI.Button(buttonRect, "Move")) {
			if (!moving) {
				GameManager.instance.removeTileHighlights();
                defending = false;
                moving = true;
				attacking = false;
				GameManager.instance.highlightTilesAt(gridPosition, Color.blue, movementPerActionPoint);
			} else {
                defending = false;
                moving = false;
				attacking = false;
				GameManager.instance.removeTileHighlights();
			}
		}

        // Defend Button
        buttonRect = new Rect(0, Screen.height - buttonHeight * 3, buttonWidth, buttonHeight);

        if (GUI.Button(buttonRect, "Defend")){
            if (!defending)
            {
                GameManager.instance.removeTileHighlights();
                defending = true;
                moving = false;
                attacking = false;
                GameManager.instance.highlightTilesAt(gridPosition, Color.green, 0);
                GameManager.instance.nextTurn();
            } else {
                defending = false;
                moving = false;
                attacking = false;
                GameManager.instance.removeTileHighlights();
            }
        }

		//attack button
		buttonRect = new Rect(0, Screen.height - buttonHeight * 2, buttonWidth, buttonHeight);
		
		if (GUI.Button(buttonRect, "Attack")) {
			if (!attacking) {
				GameManager.instance.removeTileHighlights();
                defending = false;
                moving = false;
				attacking = true;
				GameManager.instance.highlightTilesAt(gridPosition, Color.red, attackRange);
			} else {
				moving = false;
				attacking = false;
                defending = false;
				GameManager.instance.removeTileHighlights();
                // Modify data here. Character is attacking, so play animation (nudge or something) 
                // and call to numerical damage methods
			}
		}
		
		//end turn button
		buttonRect = new Rect(0, Screen.height - buttonHeight * 1, buttonWidth, buttonHeight);		
		
		if (GUI.Button(buttonRect, "End Turn")) {
			GameManager.instance.removeTileHighlights();
			actionPoints = 2;
			moving = false;
			attacking = false;			
			GameManager.instance.nextTurn();
		}
		
		base.TurnOnGUI ();
	}
}
