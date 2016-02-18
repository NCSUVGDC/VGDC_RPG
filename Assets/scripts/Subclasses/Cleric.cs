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
		case EquippedStone.AirStone:
			curse = curse * 2;
			healAmount = healAmount * 2;

		// If Earth Stone, buff Buff and Heal
		case EquippedStone.EarthStone:
			buff = buff * 2;
			healAmount = healAmount * 2;

		//If Fire Stone, buff Buff and Curse
		case EquippedStone.FireStone:
			buff = buff * 2;
			curse = curse * 2;

		//If Water Stone, buff Buff, Curse, and Heal
		case EquippedStone.WaterStone:
			buff = Mathf.Ceil(buff * 1.5);
			curse = Mathf.Ceil(curse * 1.5);
			healAmount = Mathf.Ceil(healAmount * 1.5);

		}

	}

}