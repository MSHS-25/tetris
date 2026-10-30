using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    // Points for clearing 0-4 lines at once
    private static readonly int[] ScorePerLine = { 0, 100, 300, 500, 800 };

    public TextMeshProUGUI scoreText;

    public int score { get; private set; }

    private void Start() => UpdateScoreUI();

    public void AddLineClear(int lines) => AddPoints(ScorePerLine[Mathf.Clamp(lines, 0, ScorePerLine.Length - 1)]);

    public void AddPoints(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt("LastScore", score);
        PlayerPrefs.Save();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}
