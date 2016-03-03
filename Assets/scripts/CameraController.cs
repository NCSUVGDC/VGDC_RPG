using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    Vector3 priorMousePosition;
    Vector3 mouseDeltaVector;

    [SerializeField]
    private float cameraSpeed = 1;
    public float dampening = 0.95f;
    public float maxVel = 2;

    private Vector2 vel;

    private Camera cam;

    public static float Zoom = 1.0f;

    // Use this for initialization
    void Start()
    {
        //priorMousePosition = Input.mousePosition;
        cam = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        cam.orthographicSize = Screen.height / 128.0f / Zoom;

        if (Input.GetMouseButtonDown(1))
            priorMousePosition = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            mouseDeltaVector = (priorMousePosition - Input.mousePosition) / Zoom;
            vel = Vector2.zero;//+= new Vector2(mouseDeltaVector.x, mouseDeltaVector.y) * Time.smoothDeltaTime;
            gameObject.transform.position += new Vector3(mouseDeltaVector.x, 0, mouseDeltaVector.y) * cameraSpeed;

            /*Vector3 newPosition;
            newPosition.x = gameObject.transform.position.x + mouseDeltaVector.x * Time.deltaTime * cameraSpeed;
            newPosition.y = gameObject.transform.position.y;
            newPosition.z = gameObject.transform.position.z + mouseDeltaVector.y * Time.deltaTime * cameraSpeed;

            gameObject.transform.position = newPosition;*/

            priorMousePosition = Input.mousePosition;
        } //Middle mouse button down
        if (Input.GetMouseButtonUp(1))
        {
            mouseDeltaVector = (priorMousePosition - Input.mousePosition);
            vel = new Vector2(mouseDeltaVector.x, mouseDeltaVector.y) * Time.smoothDeltaTime * 64;
        }
        if (Input.mouseScrollDelta.y > 0)
            Zoom *= 2;
        else if (Input.mouseScrollDelta.y < 0)
            Zoom /= 2;
        Zoom = Mathf.Clamp(Zoom, 1 / 8f, 8f);

        vel = Vector3.ClampMagnitude(vel, maxVel);
        gameObject.transform.position += new Vector3((vel * Time.deltaTime).x, 0, (vel * Time.smoothDeltaTime).y);
        vel *= dampening;
    }


}
