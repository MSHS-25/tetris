using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
using System.Collections;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour // All variables are defined above
{
    private AudioSource audioSource;

    public AudioClip lockSound;
    public AudioClip lineClearSound;
    public AudioClip gameOverSound;
    public AudioClip rotateSound;
    public AudioClip mainThemeMusic;

    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Scoreboard scoreboard;
    public GameObject gameOverCanvas;
    private bool isGameOver = false;
    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        // Initialize components and music
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        audioSource = GetComponent<AudioSource>();

        PlayBackgroundMusic(); // Start background music

        // Initialize Tetrominoes
        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        // Spawn the first piece when the game starts
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        // Randomly select a tetromino and spawn it at the specified position
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece); // Set the piece on the board
        }
        else
        {
            GameOver(); // End the game 
        }
    }

    public bool IsGameOver() => isGameOver; // Return game over state

    public void GameOver()
    {
        tilemap.ClearAllTiles(); // Clear all tiles when the game is over
        StopBackgroundMusic(); // Stop the background music

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true); // Show game over screen
            SaveScoreAndUpdateUI(); // Save and display final score
        }

        Time.timeScale = 0f; // Pause the game
        isGameOver = true; // Set the game over flag

        StartCoroutine(WaitForSoundAndResumeGame()); // Wait for the game over sound to finish before resuming
    }

    private void SaveScoreAndUpdateUI()
    {
        // Save the score and update the UI with the final score
        if (scoreboard != null)
        {
            scoreboard.SaveScoreOnGameOver();
        }

        int finalScore = scoreboard.GetScore();

        GameOverScreen gameOverScreen = gameOverCanvas.GetComponent<GameOverScreen>();
        if (gameOverScreen != null)
        {
            gameOverScreen.UpdateFinalScore(finalScore);
        }
    }

    private IEnumerator WaitForSoundAndResumeGame()
    {
        // Play the game over sound and wait for it to finish
        if (gameOverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }

        yield return new WaitForSeconds(gameOverSound.length); // Wait until sound finishes
    }

    public void RestartGame()
    {
        // Restart the game, resetting the board and score
        tilemap.ClearAllTiles();
        scoreboard.ResetScore();
        SpawnPiece();
    }

    public void GoToMainMenu()
    {
        // Go back to the main menu
        SceneManager.LoadScene("MainMenu");
    }

    public void Set(Piece piece)
    {
        // Set the piece on the board
        SetOrClearTiles(piece, true);
    }

    public void Clear(Piece piece)
    {
        // Clear the piece from the board
        SetOrClearTiles(piece, false);
    }

    private void SetOrClearTiles(Piece piece, bool set)
    {
        // Set or clear tiles for the piece based on the flag
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;

            if (set)
            {
                tilemap.SetTile(tilePosition, piece.data.tile);
            }
            else
            {
                tilemap.SetTile(tilePosition, null);
            }
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        // Check if the piece can be placed at the given position
        RectInt bounds = Bounds;

        foreach (var cell in piece.cells)
        {
            Vector3Int tilePosition = cell + position;

            if (!bounds.Contains((Vector2Int)tilePosition) || tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        // Clear full lines and update the score
        int lines = 0;
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row); // Clear the line
                lines++;
            }
            else
            {
                row++;
            }
        }

        if (lines > 0)
        {
            PlayLineClearSound(); // Play sound when clearing lines
            scoreboard.AddScore(lines, false, false, false, false, false); // Update score
        }
    }

    private void PlayLineClearSound()
    {
        // Play the line clear sound
        if (lineClearSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(lineClearSound);
        }
    }

    public bool IsLineFull(int row)
    {
        // Check if the row is completely filled
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        // Clear the specified line and move the rest down
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        MoveLinesDown(row);
    }

    private void MoveLinesDown(int row)
    {
        // Move all lines down after clearing a line
        RectInt bounds = Bounds;

        while (row < bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int position = new Vector3Int(col, row + 1, 0);
                TileBase above = tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                tilemap.SetTile(position, above);
            }

            row++;
        }
    }

    public void ResumeGame()
    {
        // Resume the game after game over
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
            Time.timeScale = 1f;
        }

        isGameOver = false;
        SpawnPiece();

        PlayBackgroundMusic();
    }

    public AudioSource GetAudioSource() // Get the audio
    {
        return audioSource;
    }

    private void PlayBackgroundMusic() // Play the theme
    {
        if (mainThemeMusic != null && audioSource != null)
        {
            audioSource.clip = mainThemeMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void StopBackgroundMusic()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
    }

    private void Update()
    {
        if (!isGameOver)
        {
        }
    }
}
