using UnityEngine;
using System.Collections;

public class Cleric : UserPlayer {

	//initialize base Cleric Stats
	public int buff = 10;
	public int healAmount = 10;
	public int curse = 10;


	//Constructor for Cleric
	public Cleric ()
	{

		moveSpeed = 10.0f;

		movementPerActionPoint = 5;
		attackRange = 3;

		playerName = "Cleric";
		HP = 25;

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

		//If air stone, buff Curse and Heal
		case (int) StoneTypes.AirStone:
			curse = curse * 3;
			healAmount = healAmount * 3;
			break;

		// If Earth Stone, buff Buff and Heal
		case (int) StoneTypes.EarthStone:
			buff = buff * 3;
			healAmount = healAmount * 3;
			break;

		//If Fire Stone, buff Buff and Curse
		case (int) StoneTypes.FireStone:
			buff = buff * 3;
			curse = curse * 3;
			break;

		//If Water Stone, buff Buff, Curse, and Heal
		case (int) StoneTypes.WaterStone:
			buff = buff * 2;
			curse = curse * 2;
			healAmount = healAmount * 2;
			break;

		}


	}

}