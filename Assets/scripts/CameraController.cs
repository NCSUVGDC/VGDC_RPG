using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    Vector3 priorMousePosition;
    Vector3 mouseDeltaVector;

    [SerializeField]
    private float cameraSpeed;
    GameManager gameManager;
    Player currentPlayer;

    // Use this for initialization
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        priorMousePosition = Input.mousePosition;
        currentPlayer = gameManager.players[gameManager.currentPlayerIndex];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = gameObject.transform.position;
        if (Input.GetMouseButton(2)) //Middle mouse button down
        {
            mouseDeltaVector = (priorMousePosition - Input.mousePosition); //store vector from old pos to new pos.
            mouseDeltaVector.Normalize(); //Get the direction of the difference
            newPosition.x = gameObject.transform.position.x - mouseDeltaVector.x * Time.deltaTime * cameraSpeed;
            newPosition.y = gameObject.transform.position.y;
            newPosition.z = gameObject.transform.position.z - mouseDeltaVector.y * Time.deltaTime * cameraSpeed;

            
        }
        else
        {
            if (currentPlayer) newPosition.x = currentPlayer.transform.position.x;
            if (currentPlayer) newPosition.y = gameObject.transform.position.y;
            if (currentPlayer) newPosition.z = currentPlayer.transform.position.z;
        }
        gameObject.transform.position = newPosition;

    }

    public void UpdateCharacterFocus()
    {
        currentPlayer = gameManager.players[gameManager.currentPlayerIndex];
    }

}
