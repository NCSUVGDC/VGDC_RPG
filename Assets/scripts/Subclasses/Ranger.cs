using UnityEngine;
using System.Collections;

public class Ranger : UserPlayer {

	public Ranger ()
	{
		
		moveSpeed = 10.0f;

		movementPerActionPoint = 5;
		attackRange = 3;

		playerName = "Hunter";
		HP = 25;

		attackChance = .75f;
		defenseReduction = .15f;
		damageBase = 5;
		damageRollSides = 6; //d6

		actionPoints = 2;
		maxActionPoints = 2;

		attackSpeed = 2;


	}


}