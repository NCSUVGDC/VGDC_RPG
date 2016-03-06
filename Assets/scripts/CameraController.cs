using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public static float Zoom = 1.0f;

    private Vector3 priorMousePosition;
    private Vector3 mouseDeltaVector;

    public float CameraSpeed = 1;

    private float targetSpeed = 1.0f;

    private Camera cam;

    private Vector3 targetPosition;
    public Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }
        set
        {
            targetPosition = value;
            targetSpeed = Vector3.Distance(targetPosition, transform.position) * CameraSpeed;
        }
    }

    // Use this for initialization
    void Start()
    {
        cam = GetComponent<Camera>();
        targetPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        cam.orthographicSize = Screen.height / 128.0f / Zoom;
        var dt = Mathf.Min(Time.smoothDeltaTime, 1 / 30f);

        if (Input.GetMouseButtonDown(1))
            priorMousePosition = Input.mousePosition;
        if (Input.GetMouseButton(1))
        {
            mouseDeltaVector = (priorMousePosition - Input.mousePosition) / Zoom;
            targetPosition += new Vector3(mouseDeltaVector.x, 0, mouseDeltaVector.y) / 64f;
            transform.position = targetPosition;
            priorMousePosition = Input.mousePosition;
        }
        if (Input.mouseScrollDelta.y > 0)
            Zoom *= 2;
        else if (Input.mouseScrollDelta.y < 0)
            Zoom /= 2;
        Zoom = Mathf.Clamp(Zoom, 1 / 4f, 4f);

        if (Vector3.SqrMagnitude(transform.position - targetPosition) > targetSpeed * targetSpeed * dt * dt)
        {
            Vector3 n = (targetPosition - transform.position).normalized;
            transform.position += n * targetSpeed * Time.deltaTime;
        }
        else
            transform.position = targetPosition;
        //transform.position = Vector3.Lerp(transform.position, TargetPosition, 0.05f);
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        targetPosition = pos;
    }
}
