using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Reference to the PauseMenu Canvas
    public GameObject playerController; // Reference to the player or camera controller script
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); // Hide the Pause Menu
        Time.timeScale = 1f;          // Resume game time
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor to the screen
        Cursor.visible = false;      // Hide the cursor
        playerController.SetActive(true); // Enable player controls
        isPaused = false;
    }

    public void PauseGame()
    {
        pauseMenuUI.SetActive(true);  // Show the Pause Menu
        Time.timeScale = 0f;          // Freeze game time
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true;       // Show the cursor
        playerController.SetActive(false); // Disable player controls
        isPaused = true;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;          // Reset time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;          // Reset time scale
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
