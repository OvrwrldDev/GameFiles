using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OptionsMenu : MonoBehaviour
{
    public GameObject optionsMenuUI; // Reference to the Options Menu UI
    public Slider crosshairSizeSlider; // Reference to the crosshair size slider
    public GrapplingHook grapplingHookScript; // Reference to the grappling hook script

    private void Start()
    {
        // Initialize the slider with the default crosshair size
        if (grapplingHookScript != null && crosshairSizeSlider != null)
        {
            crosshairSizeSlider.value = grapplingHookScript.crosshair.sizeDelta.x;
            crosshairSizeSlider.onValueChanged.AddListener(UpdateCrosshairSize);
        }
    }

    // Method to update the crosshair size from the slider
    public void UpdateCrosshairSize(float size)
    {
        if (grapplingHookScript != null)
        {
            grapplingHookScript.SetCrosshairSize(size);
        }
    }

    // Show the options menu
    public void OpenOptionsMenu()
    {
        optionsMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pause the game while in options
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor
    }

    // Hide the options menu and resume the game
    public void CloseOptionsMenu()
    {
        optionsMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
    }

    // Button for returning to the main menu
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); // Replace with the name of your main menu scene
    }
}
