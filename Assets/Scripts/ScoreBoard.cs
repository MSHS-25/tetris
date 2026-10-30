using UnityEngine;
using TMPro;

public class Scoreboard : MonoBehaviour
{
    public int score { get; private set; }
    public int linesCleared { get; private set; }
    public int piecesPlaced { get; private set; }

    private int[] scorePerLine = { 0, 100, 300, 500, 800 };

    private int miniTSpinScore = 100;
    private int miniTSpinSingleScore = 200;
    private int tSpinScore = 400;
    private int tSpinDoubleScore = 1200;

    private float backToBackBonus = 1.5f;

    private int softDropPointsPerLine = 1;
    private int hardDropPointsPerLine = 2;

    public TextMeshProUGUI scoreText; // UI element to display the score

    private void Start()
    {
        UpdateScoreUI(); // Update the UI at the start
    }

    public void ResetScore()
    {
        score = 0;
        linesCleared = 0;
        piecesPlaced = 0;
        UpdateScoreUI(); // Update the UI after resetting
    }

    public void AddScore(int lines, bool isTSpin = false, bool isMiniTSpin = false, bool isBackToBack = false, bool isHardDrop = false, bool isSoftDrop = false)
    {
        int baseScore = 0;

        // Determine base score based on lines cleared
        if (lines >= 1 && lines <= 4)
        {
            baseScore = scorePerLine[lines];
        }

        // Handle T-Spins
        if (isTSpin)
        {
            baseScore = tSpinScore;
            if (lines == 1)
            {
                baseScore = isBackToBack ? tSpinDoubleScore : tSpinScore;
            }
        }
        else if (isMiniTSpin)
        {
            baseScore = miniTSpinScore;
            if (lines == 1)
            {
                baseScore = miniTSpinSingleScore;
            }
        }

        // Apply back-to-back bonus
        if (isBackToBack)
        {
            baseScore = Mathf.FloorToInt(baseScore * backToBackBonus);
        }

        // Add points for drops
        if (isSoftDrop)
        {
            baseScore += softDropPointsPerLine * lines;
        }
        if (isHardDrop)
        {
            baseScore += hardDropPointsPerLine * lines;
        }

        // Update score and lines cleared
        score += baseScore;  // Removed level multiplication
        linesCleared += lines;
        piecesPlaced++;

        UpdateScoreUI(); // Update the UI with the new score
    }

    public void PieceLocked()
    {
        piecesPlaced++;
        UpdateScoreUI(); // Update the UI when a piece is locked
    }

    private void UpdateScoreUI()
    {
        // Update the score display in the UI
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }
    }

    public void SaveScoreOnGameOver()
    {
        // Sla de score op bij game over
        PlayerPrefs.SetInt("LastScore", score);  // Gebruik de juiste scorevariabele
        PlayerPrefs.Save();
    }

    public int GetScore()
    {
        return score; // Return the current score
    }
}
