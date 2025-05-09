using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float mouseSensitivity = 2f;
    public Transform cameraTransform;

    float xRotation = 0f;

    // bobbing
    public float bobTimer = 0f;
    public float bobSpeed = 6f;
    public float bobAmount = 0.05f;



    private CharacterController controller;


    Vector3 camStartPos;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        camStartPos = cameraTransform.localPosition;
    }

    void Update()
    {
        // mouse look
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        // movement
        float speed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        // head bobbing
        if (move.magnitude > 0.1f)
        {
            float sprintMultiplier = Input.GetKey(KeyCode.LeftShift) ? 1.5f : 1f;
            bobTimer += Time.deltaTime * bobSpeed * sprintMultiplier;

            float bobY = Mathf.Sin(bobTimer) * bobAmount * sprintMultiplier;
            cameraTransform.localPosition = camStartPos + new Vector3(0, bobY, 0);
        }
        else
        {
            bobTimer = 0f;
            cameraTransform.localPosition = Vector3.Lerp(
                cameraTransform.localPosition,
                camStartPos,
                Time.deltaTime * 5f
            );
        }

    }
}
