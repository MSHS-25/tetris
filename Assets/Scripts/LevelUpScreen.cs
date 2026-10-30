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
        int currentScore = PlayerPrefs.GetInt("LastScore", 0);

        levelText.text = $"Level {currentLevel}";
        scoreText.text = $"Score: {currentScore}";

        resumeButton.onClick.AddListener(OnResumeClick);
    }

    private void OnResumeClick()
    {
        // Continue playing: back to the Tetris scene
        SceneManager.LoadScene("Tetris");
    }
}
