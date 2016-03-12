/// © 2015  Individual Contributors. All Rights Reserved.
/// Contributors were members of the Video Game Development Club at North Carolina State University.
/// File Contributors: ?


using UnityEngine;

public class AIPlayer : Player {

	void Start () {
		attackSpeed = 4;
	}

	// Update is called once per frame
	void Update () {
        if (GameManager.instance.players[GameManager.instance.currentPlayerIndex] == this)
        {
            transform.GetComponent<Renderer>().material.color = Color.green;
        }
        else
        {
            transform.GetComponent<Renderer>().material.color = Color.white;
        }

        if (HP <= 0)
        {
            transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
            transform.GetComponent<Renderer>().material.color = Color.red;
        }
    }
	public override void TurnUpdate ()
	{
		/*if (Vector3.Distance(moveDestination, transform.position) > 0.1f) 
        {
			transform.position += (moveDestination - transform.position).normalized * moveSpeed * Time.deltaTime;
			
			if (Vector3.Distance(moveDestination, transform.position) <= 0.1f) 
            {
				transform.position = moveDestination;
				actionPoints--;
			}
		}
        else 
        {
			moveDestination = new Vector3(0 - Mathf.Floor(GameManager.instance.mapSizeX/2),1.5f, -0 + Mathf.Floor(GameManager.instance.mapSizeY/2));
		}*/
		base.TurnUpdate ();
	}
	
	public override void TurnOnGUI () {
		base.TurnOnGUI ();
	}
}
