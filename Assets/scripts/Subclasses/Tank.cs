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


	//Only called at the start of the game or when a stone is equipped; will not be used for curse or buff effects
	public override void UpdateStatsForNewStone () {

		switch (EquippedStone) 
		{

		//If air stone, buff Movement and Defense
		case (int) StoneTypes.AirStone:
			movementPerActionPoint = movementPerActionPoint * 3;
			defenseReduction = defenseReduction * 3;
			break;

			// If Earth Stone, buff Movement, Defense, and Damage
		case (int) StoneTypes.EarthStone:
			movementPerActionPoint = movementPerActionPoint * 2;
			defenseReduction = defenseReduction * 2;
			damageBase = damageBase * 2;
			break;

			//If Fire Stone, buff Movement and Damage
		case (int) StoneTypes.FireStone:
			movementPerActionPoint = movementPerActionPoint * 3;
			damageBase = damageBase * 3;
			break;

			//If Water Stone, buff Defense and Damage
		case (int) StoneTypes.WaterStone:
			defenseReduction = defenseReduction * 3;
			damageBase = damageBase * 3;
			break;

		}
	}
		
}
