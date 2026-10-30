using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    public TextMeshProUGUI finalScoreText;

    public void RestartGame()
    {
        Time.timeScale = 1f; // the game over screen paused the game
        SceneManager.LoadScene("Tetris");
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void UpdateFinalScore(int score)
    {
        finalScoreText.text = $"Final Score: {score}";
    }
}
