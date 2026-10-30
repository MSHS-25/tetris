using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    private const string LastScoreKey = "LastScore";

    // Points for clearing 0-4 lines at once
    private static readonly int[] ScorePerLine = { 0, 100, 300, 500, 800 };

    public int score { get; private set; }
    public int linesCleared { get; private set; }
    public int piecesPlaced { get; private set; }

    public TextMeshProUGUI scoreText;

    private void Start()
    {
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        score = 0;
        linesCleared = 0;
        piecesPlaced = 0;
        UpdateScoreUI();
    }

    // Awards points for a line clear of 1-4 lines
    public void AddLineClear(int lines)
    {
        if (lines < 1 || lines >= ScorePerLine.Length)
        {
            return;
        }

        score += ScorePerLine[lines];
        linesCleared += lines;
        UpdateScoreUI();
    }

    // Awards points for cells dropped by the player (1 per soft drop cell, 2 per hard drop cell)
    public void AddDropPoints(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    public void PieceLocked()
    {
        piecesPlaced++;
    }

    public void SaveScoreOnGameOver()
    {
        PlayerPrefs.SetInt(LastScoreKey, score);
        PlayerPrefs.Save();
    }

    public int GetScore()
    {
        return score;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }
}
