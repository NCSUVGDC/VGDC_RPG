/// © 2015  Individual Contributors. All Rights Reserved.
/// Contributors were members of the Video Game Development Club at North Carolina State University.
/// File Contributors: ?
/// code adapted from this tutorial: https://www.youtube.com/watch?v=qC13uS6vBfU&feature=youtu.be

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	public static GameManager instance;
	
	public GameObject TilePrefab;
	public GameObject UserPlayerPrefab;
	public GameObject AIPlayerPrefab;
	
	public int mapSizeX = 11;
    public int mapSizeY = 11;

    public List <List<Tile>> map = new List<List<Tile>>();
	public List <Player> players = new List<Player>();
	public int currentPlayerIndex = 0;
	public List <Player> turnQueue = new List<Player>();


	
	void Awake() {
		instance = this;
        map = new List<List<Tile>>(mapSizeX);
        for (int i = 0; i < mapSizeX; i++)
        {
            map.Insert(i, new List<Tile>(mapSizeY));
            for (int j = 0; j < mapSizeY; j++)
            {
                map[i].Insert(j, null);
            }
        }
		// Initial player sort by speed
		//updateTurnQueue ();
    }
	
	// Use this for initialization
	void Start () {		
		//generateMap();
		//generatePlayers();
	}
	
	// Update is called once per frame
	void Update () {
        resetMap();
        makePlayersPositionImpassible();
		// Updates the turnQueue every frame
		//updateTurnQueue();

        if (players[currentPlayerIndex].HP > 0)
        {
            players[currentPlayerIndex].TurnUpdate();
        }
        else
        {
            nextTurn();
        }
	}
	
	void OnGUI () {
		if (players[currentPlayerIndex].HP > 0) players[currentPlayerIndex].TurnOnGUI();
	}
	
	public void nextTurn() {
        resetMap();

		if (currentPlayerIndex + 1 < players.Count) {
			currentPlayerIndex++;
		} else {
			currentPlayerIndex = 0;
		}
	}
    public void makePlayersPositionImpassible()
    {
        foreach(Player p in players)
        {
            map[(int)p.gridPosition.x][(int)p.gridPosition.y].impassible = true;
        }
    }
    public void resetMap()
    {
        for (int i = 0; i < map.Count; i++)
        {
            for (int j = 0; j < map[i].Count; j++)
            {
                if (map[i][j] != null)
                {
                    map[i][j].impassible = false;
                }
              
            }
        }
    }
	
	public void highlightTilesAt(Vector2 originLocation, Color highlightColor, int distance) {
        // Make sure that attacking player can attack adjacent players
        foreach(Player p in players)
        {
            if (p.attacking || p.defending)
            {
                foreach(Player other in players)
                {
                    if(!other.Equals(p))
                    map[(int)other.gridPosition.x][(int)other.gridPosition.y].impassible = false;
                }
            }
        }

        List <Tile> highlightedTiles = TileHighlight.FindHighlight(map[(int)originLocation.x][(int)originLocation.y], distance);
		
		foreach (Tile t in highlightedTiles) {
			t.transform.GetComponent<Renderer>().material.color = highlightColor;
		}
	}
	
	public void removeTileHighlights() {
        for (int i = 0; i < mapSizeX; i++) {
			for (int j = 0; j < mapSizeY; j++) {
                if (map[i][j] != null &&  !map[i][j].impassible)
                {
                    map[i][j].transform.GetComponent<Renderer>().material.color = Color.white;
                }
			}
		}
	}
 	
	public void moveCurrentPlayer(Tile destTile) {
		if (destTile.transform.GetComponent<Renderer>().material.color != Color.white && !destTile.impassible) {
			removeTileHighlights();
			players[currentPlayerIndex].moving = false;
			foreach(Tile t in TilePathFinder.FindPath(map[(int)players[currentPlayerIndex].gridPosition.x][(int)players[currentPlayerIndex].gridPosition.y],destTile)) {
                players[currentPlayerIndex].positionQueue.Add(map[(int)t.gridPosition.x][(int)t.gridPosition.y].transform.position); //+ 1.5f * Vector3.up);
				Debug.Log("(" + players[currentPlayerIndex].positionQueue[players[currentPlayerIndex].positionQueue.Count - 1].x + "," + players[currentPlayerIndex].positionQueue[players[currentPlayerIndex].positionQueue.Count - 1].y + ")");
			}			
			players[currentPlayerIndex].gridPosition = destTile.gridPosition;
		} else {
			Debug.Log ("destination invalid");
		}
	}
	
	public void attackWithCurrentPlayer(Tile destTile) {
        // Used to fix an impassible bug when attacking but there's probably an easier way to do this without reusing code 
        foreach (Player p in players)
        {
            map[(int)p.gridPosition.x][(int)p.gridPosition.y].impassible = false;
        }
        if (destTile.transform.GetComponent<Renderer>().material.color != Color.white && !destTile.impassible) {
			
			Player target = null;
			foreach (Player p in players) {
				if (p.gridPosition == destTile.gridPosition) {
					target = p;
				}
			}
			
			if (target != null) {
								
				//Debug.Log ("p.x: " + players[currentPlayerIndex].gridPosition.x + ", p.y: " + players[currentPlayerIndex].gridPosition.y + " t.x: " + target.gridPosition.x + ", t.y: " + target.gridPosition.y);
				if (players[currentPlayerIndex].gridPosition.x >= target.gridPosition.x - 1 && players[currentPlayerIndex].gridPosition.x <= target.gridPosition.x + 1 &&
					players[currentPlayerIndex].gridPosition.y >= target.gridPosition.y - 1 && players[currentPlayerIndex].gridPosition.y <= target.gridPosition.y + 1) {
					players[currentPlayerIndex].actionPoints--;
					
					removeTileHighlights();
					players[currentPlayerIndex].moving = false;
                    players[currentPlayerIndex].attacking = false;		
					
					//attack logic
					//roll to hit
					bool hit = Random.Range(0.0f, 1.0f) <= players[currentPlayerIndex].attackChance;
					
					if (hit) {
                        // damage logic
                        int amountOfDamage;
                        if (target.defending)
                        {
                            //with defending target
                            amountOfDamage = (int)Mathf.Floor(players[currentPlayerIndex].damageBase * target.defenseReduction + Random.Range(0, players[currentPlayerIndex].damageRollSides));
                        } else {
                            //without defending target
                            amountOfDamage = (int)Mathf.Floor(players[currentPlayerIndex].damageBase + Random.Range(0, players[currentPlayerIndex].damageRollSides));
                        }						
						target.HP -= amountOfDamage;
						
						Debug.Log(players[currentPlayerIndex].playerName + " successfuly hit " + target.playerName + " for " + amountOfDamage + " damage!");
					} else {
						Debug.Log(players[currentPlayerIndex].playerName + " missed " + target.playerName + "!");
					}
				} else {
					Debug.Log ("Target is not adjacent!");
				}
			}
		} else {
			Debug.Log ("destination invalid");
		}
	}
	
	/*void generateMap() {
        if (map == null)
        {
            map = new List<List<Tile>>();
        }
		
		/*for (int i = 0; i < mapSize; i++) {
			List <Tile> row = new List<Tile>();
			for (int j = 0; j < mapSize; j++) {
				//Tile tile = ((GameObject)Instantiate(TilePrefab, new Vector3(i - Mathf.Floor(mapSize/2),0, -j + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
				//tile.gridPosition = new Vector2(i, j);
				//row.Add (null);
			}
			//map.Add(row);
		}
	}*/
	
	void generatePlayers() {
		/*UserPlayer player;
		
		player = ((GameObject)Instantiate(UserPlayerPrefab, new Vector3(0 - Mathf.Floor(mapSize/2),1.5f, -0 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<UserPlayer>();
		player.gridPosition = new Vector2(0,0);
		player.playerName = "Bob";				
		
		players.Add(player);
		
		player = ((GameObject)Instantiate(UserPlayerPrefab, new Vector3((mapSize-1) - Mathf.Floor(mapSize/2),1.5f, -(mapSize-1) + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<UserPlayer>();
		player.gridPosition = new Vector2(mapSize-1,mapSize-1);
		player.playerName = "Kyle";
		
		players.Add(player);
				
		player = ((GameObject)Instantiate(UserPlayerPrefab, new Vector3(4 - Mathf.Floor(mapSize/2),1.5f, -4 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<UserPlayer>();
		player.gridPosition = new Vector2(4,4);
		player.playerName = "Lars";
		
		players.Add(player);
		
		//AIPlayer aiplayer = ((GameObject)Instantiate(AIPlayerPrefab, new Vector3(6 - Mathf.Floor(mapSize/2),1.5f, -4 + Mathf.Floor(mapSize/2)), Quaternion.Euler(new Vector3()))).GetComponent<AIPlayer>();
		
		//players.Add(aiplayer);
        */
	}

   public static bool IsReady()
    {
        return (instance != null);
    }
	/**
	// Added by Russell, mustard on the beat hoe
	public void updateTurnQueue()
	{
		// Reset the turnQueue for updated speeds, for example, if a player's speed decreases because of a move
		turnQueue = new List<Player>();
		// temp list to remove players from when sorting
		List<Player> temp = players;
		int fastest = temp[0].attackSpeed;
		int index = 0;
		// while the list isn't empty, we want to sort it
		while (temp.Count != 0) {
			// Finds the next fastest player, saves the index and stat
			for(int i = 0; i < temp.Count; i++) {
				if(temp[i].attackSpeed > fastest)
				{
					fastest = temp[i].attackSpeed;
					index = i;
				}
			}
			// After it's done going through the list, the fastest is found and added to the turnQueue in order
			turnQueue.Add(temp[index]);
			// Removes the player to avoid duplication
			temp.RemoveAt(index);
			// Resets the iterator assuming it's not empty in order to sort again
			if(temp.Count != 0) {
				index = 0;
				fastest = temp[0].attackSpeed;
			}
		}
	} */
}
