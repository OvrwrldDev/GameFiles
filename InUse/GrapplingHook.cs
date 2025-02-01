using UnityEngine;
using UnityEngine.UI;

public class GrapplingHook : MonoBehaviour
{
    public Camera playerCamera;        // Reference to the player's camera
    public LayerMask grappleLayer;     // Layer mask for grappleable surfaces
    public LayerMask obstructionLayer; // Layer mask for obstacles that block the grapple
    public float maxDistance = 20f;    // Max distance of the grappling hook
    public float grappleSpeed = 10f;   // Speed at which the player is pulled to the grapple point
    public float gravity = -9.8f;      // Gravity value for the player
    public float jumpForce = 5f;       // Jump force applied after grappling
    public float gravityLerpSpeed = 2f; // Speed at which gravity transitions after grappling
    public KeyCode grappleKey = KeyCode.Mouse1; // Key to activate grappling hook
    public KeyCode jumpKey = KeyCode.Space;     // Key to jump

    public Image crosshair;            // Reference to the crosshair UI element
    public Color defaultCrosshairColor = Color.white;
    public Color grappleableCrosshairColor = Color.green;

    private LineRenderer lineRenderer; // For visualizing the grappling hook
    private Vector3 grapplePoint;      // Target point for the grappling hook
    private bool isGrappling = false;  // Is the player currently grappling?
    private CharacterController characterController; // Player's CharacterController for movement
    private Vector3 velocity;          // Current player velocity
    private bool isTransitioningFromGrapple = false; // Is the player transitioning from grappling?

    void Start()
    {
        // Initialize components
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;

        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError("No CharacterController found! Add a CharacterController component to the player.");
        }

        if (crosshair == null)
        {
            Debug.LogError("No crosshair assigned! Please assign a UI Image for the crosshair.");
        }
    }

    void Update()
    {
        UpdateCrosshair();

        if (Input.GetKeyDown(grappleKey))
        {
            StartGrapple();
        }

        if (isGrappling)
        {
            Grapple();
        }
        else
        {
            ApplyGravity();
        }

        if (Input.GetKeyUp(grappleKey))
        {
            StopGrapple();
        }

        if (Input.GetKeyDown(jumpKey) && characterController.isGrounded)
        {
            Jump();
        }
    }

    void UpdateCrosshair()
    {
        // Perform a raycast from the crosshair's position
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, grappleLayer))
        {
            // Perform a secondary raycast to check if there's an obstruction
            if (!Physics.Raycast(ray, hit.distance, obstructionLayer))
            {
                // Change crosshair color when aiming at an unobstructed grappleable surface
                crosshair.color = grappleableCrosshairColor;
                return;
            }
        }

        // Revert to the default crosshair color if not aiming at an unobstructed grappleable surface
        crosshair.color = defaultCrosshairColor;
    }

    void StartGrapple()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, grappleLayer))
        {
            // Perform a secondary raycast to check for obstructions
            if (!Physics.Raycast(ray, hit.distance, obstructionLayer))
            {
                grapplePoint = hit.point;
                isGrappling = true;
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, grapplePoint);

                // Temporarily disable gravity and upward velocity
                velocity.y = 0;
            }
        }
    }

    void Grapple()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, grapplePoint);

        Vector3 direction = (grapplePoint - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, grapplePoint);

        // Move the player towards the grapple point
        Vector3 move = direction * grappleSpeed * Time.deltaTime;
        characterController.Move(move);

        // Stop grappling when close to the point
        if (distance < 1f)
        {
            StopGrapple();
            velocity.y = jumpForce; // Add upward force to simulate jumping over a wall
            isTransitioningFromGrapple = true;
        }
    }

    void StopGrapple()
    {
        isGrappling = false;
        lineRenderer.enabled = false;
    }

    void ApplyGravity()
    {
        if (isTransitioningFromGrapple)
        {
            // Gradually transition from no gravity to full gravity
            velocity.y = Mathf.Lerp(velocity.y, gravity, gravityLerpSpeed * Time.deltaTime);
            if (Mathf.Abs(velocity.y - gravity) < 0.1f)
            {
                isTransitioningFromGrapple = false; // End the transition
            }
        }
        else if (!characterController.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = 0; // Reset velocity when grounded
        }

        characterController.Move(velocity * Time.deltaTime);
    }

    void Jump()
    {
        velocity.y = jumpForce;
    }
}
