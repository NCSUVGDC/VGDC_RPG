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
		case EquippedStone.AirStone:
			defenseReduction = defenseReduction * 2;
			attackRange = attackRange * 2;

			// If Earth Stone, buff AOE and Defense
		case EquippedStone.EarthStone:
			AreaOfEffect = AreaOfEffect * 2;
			defenseReduction = defenseReduction * 2;

			//If Fire Stone, buff AOE, Defense, and Range
		case EquippedStone.FireStone:
			AreaOfEffect = Mathf.Ceil(AreaOfEffect * 1.5);
			defenseReduction = Mathf.Ceil(defenseReduction * 1.5);
			attackRange = Mathf.Ceil(attackRange * 1.5);

			//If Water Stone, buff AOE and Range
		case EquippedStone.WaterStone:
			AreaOfEffect = AreaOfEffect * 2;
			attackRange = attackRange * 2;
		}
	}


}