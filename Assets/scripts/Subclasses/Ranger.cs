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


	//Only called at the start of the game or when a stone is equipped; will not be used for curse or buff effects
	public override void UpdateStatsForNewStone () {

		switch (EquippedStone) 
		{

		//If air stone, buff Accuracy, Damage, and Range
		case EquippedStone.AirStone:
			attackChance = attackChance + (1f - attackChance) / 3; //move attack chance to 1/3 between 100% and current chance
			damageBase = Mathf.Ceil(damageBase * 1.5);
			attackRange = Mathf.Ceil(attackRange * 1.5);

			// If Earth Stone, buff Accuracy and Range
		case EquippedStone.EarthStone:
			attackChance = attackChance + (1f - attackChance) / 2; //move attack chance to halfway between 100% and current chance
			attackRange = attackRange * 2;

			//If Fire Stone, buff Accuracy and Damage
		case EquippedStone.FireStone:
			attackChance = attackChance + (1f - attackChance) / 2; //move attack chance to halfway between 100% and current chance
			damageBase = damageBase * 2;

			//If Water Stone, buff Damage and Range
		case EquippedStone.WaterStone:
			damageBase = damageBase * 2;
			attackRange = attackRange * 2;

		}
	}

}