using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public AudioClip lockSound;
    public AudioClip lineClearSound;
    public AudioClip gameOverSound;
    public AudioClip rotateSound;
    public AudioClip mainThemeMusic;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);
    public Scoreboard scoreboard;
    public GameObject gameOverCanvas;

    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    private AudioSource audioSource;
    private bool isGameOver;

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
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        audioSource = GetComponent<AudioSource>();

        PlayBackgroundMusic();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    public void SpawnPiece()
    {
        TetrominoData data = tetrominoes[Random.Range(0, tetrominoes.Length)];

        activePiece.Initialize(this, spawnPosition, data);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public bool IsGameOver() => isGameOver;

    public void GameOver()
    {
        isGameOver = true;
        tilemap.ClearAllTiles();
        StopBackgroundMusic();
        PlaySound(gameOverSound);

        if (scoreboard != null)
        {
            scoreboard.SaveScoreOnGameOver();
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);

            GameOverScreen gameOverScreen = gameOverCanvas.GetComponent<GameOverScreen>();
            if (gameOverScreen != null && scoreboard != null)
            {
                gameOverScreen.UpdateFinalScore(scoreboard.GetScore());
            }
        }

        Time.timeScale = 0f; // pause the game; the game over sound still plays
    }

    public void RestartGame()
    {
        tilemap.ClearAllTiles();
        scoreboard.ResetScore();
        ResumeGame();
    }

    public void ResumeGame()
    {
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(false);
        }

        Time.timeScale = 1f;
        isGameOver = false;
        SpawnPiece();
        PlayBackgroundMusic();
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Set(Piece piece)
    {
        SetOrClearTiles(piece, piece.data.tile);
    }

    public void Clear(Piece piece)
    {
        SetOrClearTiles(piece, null);
    }

    private void SetOrClearTiles(Piece piece, TileBase tile)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            tilemap.SetTile(piece.cells[i] + piece.position, tile);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        foreach (Vector3Int cell in piece.cells)
        {
            Vector3Int tilePosition = cell + position;

            if (!bounds.Contains((Vector2Int)tilePosition) || tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    // Clears every full row, awards points and returns the number of rows cleared
    public int ClearLines()
    {
        RectInt bounds = Bounds;
        int lines = 0;
        int row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row); // rows above shift down, so check the same row again
                lines++;
            }
            else
            {
                row++;
            }
        }

        if (lines > 0)
        {
            PlaySound(lineClearSound);

            if (scoreboard != null)
            {
                scoreboard.AddLineClear(lines);
            }
        }

        return lines;
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            if (!tilemap.HasTile(new Vector3Int(col, row, 0)))
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            tilemap.SetTile(new Vector3Int(col, row, 0), null);
        }

        MoveLinesDown(row);
    }

    private void MoveLinesDown(int row)
    {
        RectInt bounds = Bounds;

        for (; row < bounds.yMax; row++)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                TileBase above = tilemap.GetTile(new Vector3Int(col, row + 1, 0));
                tilemap.SetTile(new Vector3Int(col, row, 0), above);
            }
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void PlayBackgroundMusic()
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
}
