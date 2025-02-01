using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load the game scene (replace "GameScene" with your scene name)
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Quit the application
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void OpenOptions()
    {
        // Open options menu (implement your options logic here)
        Debug.Log("Options Menu Opened");
    }
}
