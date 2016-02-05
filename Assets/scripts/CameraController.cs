using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    Vector3 priorMousePosition;
    Vector3 mouseDeltaVector;

    [SerializeField]
    private float cameraSpeed;

    // Use this for initialization
    void Start()
    {
        priorMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(2))
        {
            mouseDeltaVector = (priorMousePosition - Input.mousePosition); //store vector from old pos to new pos.
            mouseDeltaVector.Normalize(); //Get the direction of the difference

            Vector3 newPosition;
            newPosition.x = gameObject.transform.position.x - mouseDeltaVector.x * Time.deltaTime * cameraSpeed;
            newPosition.y = gameObject.transform.position.y;
            newPosition.z = gameObject.transform.position.z - mouseDeltaVector.y * Time.deltaTime * cameraSpeed;

            gameObject.transform.position = newPosition;
        } //Middle mouse button down

    }


}
