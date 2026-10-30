using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelUpScreen : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public Button resumeButton;

    private void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("SelectedLevel", 1);
        int currentScore = PlayerPrefs.GetInt("CurrentScore", 0);

        // Toon het huidige niveau en score in de LevelUpScreen
        levelText.text = $"Level {currentLevel}";
        scoreText.text = $"Score: {currentScore}";

        resumeButton.onClick.AddListener(OnResumeClick);
    }

    private void OnResumeClick()
    {
        // Als je verder gaat, ga terug naar het Tetris-spel
        SceneManager.LoadScene("Tetris");
    }
}
