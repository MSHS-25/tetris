using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    // Saves the chosen level and loads the game scene
    public void StartGameWithLevel(int level)
    {
        PlayerPrefs.SetInt("SelectedLevel", level);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Tetris");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
