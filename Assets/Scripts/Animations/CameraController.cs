using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform cameraTransform; 
    public float rotationSpeed = 5.0f;
    public float zoomSpeed = 10.0f;
    public float minZoom = -2.0f;
    public float maxZoom = 40.0f;
    public float minYAngle = 0.0f;
    public float maxYAngle = 80.0f;

    private float currentZoom = 10.0f;
    private float pitch = 45.0f; 
    private float yaw = 0.0f;   

    void Start()
    {
        currentZoom = Vector3.Distance(transform.position, cameraTransform.position);
        Vector3 angles = cameraTransform.eulerAngles;
        pitch = angles.x;
        yaw = angles.y;
    }

    void LateUpdate()
    {
        if (Input.GetMouseButton(1))
        {
            yaw += Input.GetAxis("Mouse X") * rotationSpeed;
            pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
            pitch = Mathf.Clamp(pitch, minYAngle, maxYAngle);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentZoom -= scroll * zoomSpeed;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        cameraTransform.position = transform.position - rotation * Vector3.forward * currentZoom;
        cameraTransform.rotation = rotation;
    }
}