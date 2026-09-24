using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Distance")]
    public float distance = 6f;
    public float height = 3f;

    [Header("Mouse")]
    public float sensitivityX = 200f;
    public float sensitivityY = 150f;

    [Header("Vertical Rotation")]
    public float minY = -30f;
    public float maxY = 60f;

    private float yaw = 0f;
    private float pitch = 20f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (target == null)
            return;

        // Mouse input
        yaw += Input.GetAxis("Mouse X") * sensitivityX * Time.deltaTime;
        pitch -= Input.GetAxis("Mouse Y") * sensitivityY * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, minY, maxY);

        // Rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

        // Camera position
        Vector3 offset = rotation * new Vector3(0, height, -distance);

        transform.position = target.position + offset;

        // Look at player
        transform.LookAt(target.position + Vector3.up * 1.5f);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
