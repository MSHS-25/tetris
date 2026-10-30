using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;  // UI Text for the score and level
    private Scoreboard scoreboard;

    private void Start()
    {
        scoreboard = FindObjectOfType<Scoreboard>(); // Find the Scoreboard script in the scene
    }

    // Method to restart the game
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Tetris");  // Make sure the correct scene is loaded
    }

    // Method to go to the main menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");  // Make sure the name of your main menu scene is correct
    }

    public void UpdateFinalScore(int score)
    {
        // Display the final score in the UI
        finalScoreText.text = "Final Score: " + score.ToString();
    }
}
