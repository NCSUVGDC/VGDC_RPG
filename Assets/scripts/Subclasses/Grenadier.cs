using UnityEngine;
using System.Collections;

public class Grenadier : UserPlayer {

	//Initialize base Grenadier stats

	public int AreaOfEffect = 10;

	public Grenadier ()
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

		//If air stone, buff Defense and Range
		case (int) StoneTypes.AirStone:
			defenseReduction = defenseReduction * 3;
			attackRange = attackRange * 3;
			break;

			// If Earth Stone, buff AOE and Defense
		case (int) StoneTypes.EarthStone:
			AreaOfEffect = AreaOfEffect * 3;
			defenseReduction = defenseReduction * 3;
			break;

			//If Fire Stone, buff AOE, Defense, and Range
		case (int) StoneTypes.FireStone:
			AreaOfEffect = AreaOfEffect * 2;
			defenseReduction = defenseReduction * 2;
			attackRange = attackRange * 2;
			break;

			//If Water Stone, buff AOE and Range
		case (int) StoneTypes.WaterStone:
			AreaOfEffect = AreaOfEffect * 3;
			attackRange = attackRange * 3;
			break;
		}
	}

}