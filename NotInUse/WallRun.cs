using UnityEngine;

public class WallRun : MonoBehaviour
{
    public float wallRunSpeed = 10f;             // Speed of movement along the wall
    public float wallRunTime = 2f;               // How long the wall run lasts
    public float wallDistance = 1.5f;            // Distance to check for walls
    public float wallRunGravityMultiplier = 0.1f; // Gravity multiplier during wall run
    public float normalGravity = 9.81f;          // Normal gravity when not wall running
    public LayerMask wallLayer;                  // Layer mask to detect walls

    private bool isWallRunning = false;          // Flag for checking if the player is wall running
    private bool isTouchingWall = false;         // Flag for checking if the player is close to a wall
    private Vector3 wallNormal;                  // Normal of the wall for movement direction
    private float originalStepOffset;            // To store original step offset of CharacterController
    private CharacterController characterController; // CharacterController reference

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalStepOffset = characterController.stepOffset;  // Store original step offset for later
    }

    void Update()
    {
        RaycastForWall(); // Detect if the player is near a wall

        if (isTouchingWall && Input.GetKeyDown(KeyCode.Space) && !isWallRunning)
        {
            StartWallRun();
        }

        if (isWallRunning)
        {
            WallRunMovement();
            ApplyGravity(wallRunGravityMultiplier); // Apply reduced gravity during wall run
        }
        else
        {
            ApplyGravity(1f); // Apply normal gravity when not wall running
        }
    }

    private void RaycastForWall()
    {
        RaycastHit hit;
        // Cast ray to the right side of the player
        if (Physics.Raycast(transform.position, transform.right, out hit, wallDistance, wallLayer))
        {
            isTouchingWall = true;
            wallNormal = hit.normal; // Store the normal of the wall
        }
        // Cast ray to the left side of the player
        else if (Physics.Raycast(transform.position, -transform.right, out hit, wallDistance, wallLayer))
        {
            isTouchingWall = true;
            wallNormal = hit.normal;
        }
        else
        {
            isTouchingWall = false;
        }
    }

    private void StartWallRun()
    {
        isWallRunning = true;
        characterController.stepOffset = 0.1f; // Small step offset to prevent sticking to the wall
        Invoke("StopWallRun", wallRunTime); // Stop wall run after a set time
    }

    private void WallRunMovement()
    {
        // The direction to move along the wall is the cross product of wall normal and the player's up direction
        Vector3 wallRunDirection = Vector3.Cross(wallNormal, transform.up).normalized;

        // Move the character along the wall
        characterController.Move(wallRunDirection * wallRunSpeed * Time.deltaTime);

        // Make the character stick to the wall
        Vector3 moveDirection = new Vector3(0, 0, 0);
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void ApplyGravity(float gravityMultiplier)
    {
        // Apply gravity manually (we multiply by gravityMultiplier for reduced gravity during wall running)
        Vector3 gravity = new Vector3(0, -normalGravity * gravityMultiplier, 0);
        characterController.Move(gravity * Time.deltaTime); // Apply gravity force
    }

    private void StopWallRun()
    {
        isWallRunning = false;
        characterController.stepOffset = originalStepOffset; // Restore original step offset
    }
}
