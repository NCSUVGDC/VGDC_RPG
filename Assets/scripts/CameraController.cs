using UnityEngine;

namespace VGDC_RPG
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public static float Zoom = 1.0f;

        private Vector3 priorMousePosition;
        private Vector3 mouseDeltaVector;

        public float CameraSpeed = 1;

        private float targetSpeed = 1.0f;

        private Camera cam;
        public Camera lightCam, warpCam, mergeCam;

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
                targetSpeed = Vector3.Distance(targetPosition, transform.localPosition) * CameraSpeed;
            }
        }

        // Use this for initialization
        void Start()
        {
            cam = GetComponent<Camera>();
            targetPosition = transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            cam.orthographicSize = Screen.height / 128.0f / Zoom;
            lightCam.orthographicSize = cam.orthographicSize;
            warpCam.orthographicSize = cam.orthographicSize;
            mergeCam.orthographicSize = cam.orthographicSize;
            if (lightCam.targetTexture == null || RTVs.Width != Screen.width || RTVs.Height != Screen.height)
            {
                /*lightCam.targetTexture.width = Screen.width;
                lightCam.targetTexture.height = Screen.height;*/
                RTVs.ResizeBuffers(cam, lightCam, warpCam);
            }
            var dt = Mathf.Min(Time.smoothDeltaTime, 1 / 30f);

            if (Input.GetMouseButtonDown(1))
                priorMousePosition = Input.mousePosition;
            if (Input.GetMouseButton(1))
            {
                mouseDeltaVector = (priorMousePosition - Input.mousePosition) / Zoom;
                targetPosition += new Vector3(mouseDeltaVector.x, 0, mouseDeltaVector.y) / 64f;
                transform.localPosition = targetPosition;
                priorMousePosition = Input.mousePosition;
            }
            if (Input.mouseScrollDelta.y > 0)
                Zoom *= 2;
            else if (Input.mouseScrollDelta.y < 0)
                Zoom /= 2;
            Zoom = Mathf.Clamp(Zoom, 1 / 4f, 4f);

            if (Vector3.SqrMagnitude(transform.localPosition - targetPosition) > targetSpeed * targetSpeed * dt * dt)
            {
                Vector3 n = (targetPosition - transform.localPosition).normalized;
                transform.localPosition += n * targetSpeed * Time.deltaTime;
            }
            else
                transform.localPosition = targetPosition;
            //transform.position = Vector3.Lerp(transform.position, TargetPosition, 0.05f);
        }

        public void SetPosition(Vector3 pos)
        {
            transform.localPosition = pos;
            targetPosition = pos;
        }

        void OnGUI()
        {
            /*if (GUI.Button(new Rect(200, 200, 60, 20), "Toggle Effects"))
                if (RTVs.EffectsEnabled)
                    RTVs.DisableEffects(cam, lightCam, warpCam);
            else
                    RTVs.EnableEffects(cam, lightCam, warpCam);*/
        }
    }
}
