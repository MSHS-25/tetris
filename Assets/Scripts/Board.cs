using System;
using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class Board : MonoBehaviour
{
    public AudioClip lockSound;
    public AudioClip lineClearSound;
    public AudioClip gameOverSound;
    public AudioClip rotateSound;
    public AudioClip mainThemeMusic;

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new(10, 20);
    public Vector3Int spawnPosition = new(-1, 8, 0);
    public Scoreboard scoreboard;
    public GameObject gameOverCanvas;

    public bool IsGameOver { get; private set; }

    private Tilemap tilemap;
    private Piece activePiece;
    private AudioSource audioSource;
    private RectInt bounds;

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        audioSource = GetComponent<AudioSource>();
        bounds = new RectInt(-boardSize / 2, boardSize);

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }

        if (audioSource != null && mainThemeMusic != null)
        {
            audioSource.clip = mainThemeMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    private void Start() => SpawnPiece();

    public void SpawnPiece()
    {
        activePiece.Initialize(this, spawnPosition, tetrominoes[UnityEngine.Random.Range(0, tetrominoes.Length)]);

        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        IsGameOver = true;
        tilemap.ClearAllTiles();

        if (audioSource != null)
        {
            audioSource.Stop();
        }
        PlaySound(gameOverSound);

        if (scoreboard != null)
        {
            scoreboard.SaveScore();
        }

        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);

            if (scoreboard != null && gameOverCanvas.TryGetComponent(out GameOverScreen screen))
            {
                screen.UpdateFinalScore(scoreboard.score);
            }
        }

        Time.timeScale = 0f; // pause the game; the game over sound still plays
    }

    public void Set(Piece piece) => SetTiles(piece, piece.data.tile);

    public void Clear(Piece piece) => SetTiles(piece, null);

    private void SetTiles(Piece piece, TileBase tile)
    {
        foreach (Vector3Int cell in piece.cells)
        {
            tilemap.SetTile(cell + piece.position, tile);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
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

    // Removes every full row in one pass: the board is read as a single block,
    // the remaining rows are compacted downwards and the block is written back
    public void ClearLines()
    {
        BoundsInt area = new(bounds.xMin, bounds.yMin, 0, bounds.width, bounds.height, 1);
        TileBase[] tiles = tilemap.GetTilesBlock(area);
        int width = bounds.width;
        int keptRows = 0;

        for (int row = 0; row < bounds.height; row++)
        {
            int start = row * width;

            if (Array.IndexOf(tiles, null, start, width) < 0)
            {
                continue; // full row: drop it
            }

            if (keptRows != row)
            {
                Array.Copy(tiles, start, tiles, keptRows * width, width);
            }
            keptRows++;
        }

        int lines = bounds.height - keptRows;
        if (lines == 0)
        {
            return;
        }

        Array.Clear(tiles, keptRows * width, lines * width);
        tilemap.SetTilesBlock(area, tiles);
        PlaySound(lineClearSound);

        if (scoreboard != null)
        {
            scoreboard.AddLineClear(lines);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
