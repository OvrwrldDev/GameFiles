using UnityEngine;
using UnityEngine.UI;

public class HealthAndFallDamage : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    public Image healthBarFill;

    [Header("Fall Damage Settings")]
    public float fallDamageThreshold = 10f; // Minimum fall height to take damage
    public float damageMultiplier = 2f;    // Damage per unit of fall height

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 1.1f; // Distance to check for ground below the player
    public LayerMask groundLayer;            // Layer for ground detection

    private float lastGroundedHeight;
    private bool isGrounded;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    private void Update()
    {
        HandleFallDamage();
    }

    private void HandleFallDamage()
    {
        // Check if the player is grounded
        if (IsGrounded())
        {
            if (!isGrounded) // Player just landed
            {
                float fallHeight = lastGroundedHeight - transform.position.y;

                if (fallHeight > fallDamageThreshold)
                {
                    ApplyFallDamage(fallHeight);
                }
            }

            // Update grounded status and record grounded height
            isGrounded = true;
            lastGroundedHeight = transform.position.y;
        }
        else
        {
            if (isGrounded) // Player just started falling
            {
                isGrounded = false;
            }
        }
    }

    private void ApplyFallDamage(float fallHeight)
    {
        float damage = (fallHeight - fallDamageThreshold) * damageMultiplier;
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        Debug.Log("Player has died.");
        // Implement death logic (e.g., reload scene, show game over screen)
    }

    private bool IsGrounded()
    {
        // Grounded check using Raycast
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
}