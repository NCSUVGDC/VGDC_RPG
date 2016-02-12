using UnityEngine;
using System.Collections;

public class Tank : UserPlayer {

	//Initialize base Tank stats


	public Tank ()
	{

		moveSpeed = 10.0f;

		movementPerActionPoint = 5;
		attackRange = 1;

		playerName = "Tank";
		HP = 50;

		attackChance = .75f;
		defenseReduction = .15f;
		damageBase = 5;
		damageRollSides = 6; //d6

		actionPoints = 2;
		maxActionPoints = 2;

		attackSpeed = 2;


	}



}
