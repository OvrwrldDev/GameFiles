using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform cameraTransform;  // Reference to the camera
    public float moveSpeed = 5f;       // Normal movement speed
    public float crouchSpeed = 2.5f;  // Crouch speed
    public float lookSensitivity = 2f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    // Crouching Settings
    public float standingHeight = 2f;
    public float crouchHeight = 1f;
    public float crouchTransition = 5f;

    private CharacterController controller;
    private Vector3 velocity;
    private float yRotation = 0f;  // For vertical camera rotation (pitch)

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        LookAround();
        HandleJump();
        HandleCrouch();
    }

    private void Start()
    {
        // Ensure the camera is assigned to cameraTransform if not manually assigned
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        controller = GetComponent<CharacterController>();

        // Lock the cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void MovePlayer()
    {
        float moveDirectionX = Input.GetAxis("Horizontal");
        float moveDirectionZ = Input.GetAxis("Vertical");

        // Adjust move speed based on whether sprint key is pressed
        float currentMoveSpeed = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? moveSpeed * 2f : moveSpeed;

        // Calculate the movement direction relative to the player’s rotation
        Vector3 move = transform.right * moveDirectionX + transform.forward * moveDirectionZ;

        // Apply movement to the character controller
        controller.Move(move * currentMoveSpeed * Time.deltaTime);

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void LookAround()
    {
        // Get mouse input for look around
        float mouseX = Input.GetAxis("Mouse X") * lookSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * lookSensitivity;

        // Rotate the player horizontally (yaw)
        transform.Rotate(Vector3.up * mouseX);

        // Update vertical rotation (pitch) for the camera
        yRotation -= mouseY; // Invert Y input if necessary
        yRotation = Mathf.Clamp(yRotation, -90f, 90f); // Clamp to prevent full flip

        // Apply the vertical rotation to the camera's local rotation
        cameraTransform.localRotation = Quaternion.Euler(yRotation, 0f, 0f);
    }

    private void HandleJump()
    {
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small value to keep player grounded
        }

        if (Input.GetButtonDown("Jump") && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Toggle crouch state
            if (controller.height == standingHeight)
            {
                StartCoroutine(CrouchStand(crouchHeight));
            }
            else
            {
                StartCoroutine(CrouchStand(standingHeight));
            }
        }
    }

    private System.Collections.IEnumerator CrouchStand(float targetHeight)
    {
        // Crouch or stand transition
        float currentHeight = controller.height;
        float timeElapsed = 0f;

        while (Mathf.Abs(controller.height - targetHeight) > 0.01f)
        {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / crouchTransition);
            cameraTransform.localPosition = new Vector3(0f, controller.height / 2f, 0f);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        controller.height = targetHeight;
        cameraTransform.localPosition = new Vector3(0f, targetHeight / 2f, 0f);
    }
}
