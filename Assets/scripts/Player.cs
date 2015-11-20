/// � 2015  Individual Contributors. All Rights Reserved.
/// Contributors were members of the Video Game Development Club at North Carolina State University.
/// File Contributors: ?

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	
	public Vector2 gridPosition = Vector2.zero;
	
	public Vector3 moveDestination;
	public float moveSpeed = 10.0f;
	
	public int movementPerActionPoint = 5;
	public int attackRange = 1;
	
	public bool moving = false;
	public bool attacking = false;
	
	public string playerName = "George";
	public int HP = 25;
	
	public float attackChance = 0.75f;
	public float defenseReduction = 0.15f;
	public int damageBase = 5;
	public float damageRollSides = 6; //d6
	
	public int actionPoints = 2;
    public int maxActionPoints = 2;
	
	//movement animation
	public List<Vector3> positionQueue = new List<Vector3>();	
	//
	
	void Awake () {
		moveDestination = transform.position;
	}
	
	// Use this for initialization
	void Start () {
        gridPosition = new Vector2(transform.position.x, transform.position.y);
        GameManager.instance.players.Add(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public virtual void TurnUpdate () {
		if (actionPoints <= 0) {
			actionPoints = maxActionPoints;
			moving = false;
			attacking = false;			
			GameManager.instance.nextTurn();
		}
	}
	
	public virtual void TurnOnGUI () {
		
	}
}
