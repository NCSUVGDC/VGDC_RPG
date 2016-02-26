/// © 2015  Individual Contributors. All Rights Reserved.
/// Contributors were members of the Video Game Development Club at North Carolina State University.
/// File Contributors: ?

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour
{

    [Tooltip("This tile's x and y position on the grid.")]
    public Vector2 gridPosition = Vector2.zero;

    [Tooltip("The cost for a unit to cross this tile.")]
    public int movementCost = 1;

    [Tooltip("Should this tile be impassible? \n True blocks movement through this tile.")]
    public bool impassible = false;

    [Tooltip("Tile parameterized List containing adjacent grid tiles.")]
    public List<Tile> neighbors = new List<Tile>();

    //Awake is called first. ALL object's awake methods are called before all other methods.
    void Awake()
    {
        //WaitForGameManagerReady();



    }
    void WaitForGameManagerReady()
    {
        while (!GameManager.IsReady())
        {
            StartCoroutine(WaitForSecondsWrapper(1f));
        }
    }

    //This method was influenced by: http://forum.unity3d.com/threads/control-the-gameobject-start-order.19976/, along with the other start order stuff.
    private IEnumerator WaitForSecondsWrapper(float seconds)
    {
        yield return new UnityEngine.WaitForSeconds(seconds);
    }

    // Use this for initialization. Start is called after all Awake method have been called.
    void Start()
    {
        GameManager.instance.map[(int)transform.position.x][(int)transform.position.z] = this;
        gridPosition.x = (int)transform.position.x;
        gridPosition.y = (int)transform.position.z;
        //generateNeighbors();
    }

    public void generateNeighbors()
    {

        neighbors = new List<Tile>();

        /// Populate neighbors tile list with adjacent tiles.
        //up
        if (gridPosition.y > 0)
        {
            Vector2 n = new Vector2(gridPosition.x, gridPosition.y - 1);
            neighbors.Add(GameManager.instance.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
        }
        //down
        if (gridPosition.y < GameManager.instance.mapSizeY - 1)
        {
            Vector2 n = new Vector2(gridPosition.x, gridPosition.y + 1);
            neighbors.Add(GameManager.instance.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
        }

        //left
        if (gridPosition.x > 0)
        {
            Vector2 n = new Vector2(gridPosition.x - 1, gridPosition.y);
            neighbors.Add(GameManager.instance.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
        }
        //right
        if (gridPosition.x < GameManager.instance.mapSizeX - 1)
        {
            Vector2 n = new Vector2(gridPosition.x + 1, gridPosition.y);
            neighbors.Add(GameManager.instance.map[(int)Mathf.Round(n.x)][(int)Mathf.Round(n.y)]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        generateNeighbors();
    }

    void OnMouseEnter()
    {
        /*
        if (GameManager.instance.players[GameManager.instance.currentPlayerIndex].moving) {
            transform.renderer.material.color = Color.blue;
        } else if (GameManager.instance.players[GameManager.instance.currentPlayerIndex].attacking) {
            transform.renderer.material.color = Color.red;
        }
        */
        //Debug.Log("my position is (" + gridPosition.x + "," + gridPosition.y);
    }

    void OnMouseExit()
    {
        //transform.renderer.material.color = Color.white;
    }


    void OnMouseDown()
    {
        if (GameManager.instance.players[GameManager.instance.currentPlayerIndex].moving)
        {
            GameManager.instance.moveCurrentPlayer(this);
        }
        else if (GameManager.instance.players[GameManager.instance.currentPlayerIndex].attacking)
        {
            GameManager.instance.attackWithCurrentPlayer(this);
        }
        else
        {
            impassible = impassible ? false : true;
            /**if (impassible)
            {
                transform.GetComponent<Renderer>().material.color = new Color(.5f, .5f, 0.0f);
            }
            else
            {
                transform.GetComponent<Renderer>().material.color = Color.white;
            }*/
        }

    }

}
