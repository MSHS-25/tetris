using UnityEngine;

public class Piece : MonoBehaviour
{
    private const int SoftDropPointsPerCell = 1;
    private const int HardDropPointsPerCell = 2;

    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;
    public ParticleSystem landEffect;

    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    private Board board;
    private int rotationIndex;
    private float stepTime;
    private float moveTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        cells ??= new Vector3Int[data.cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        if (board.IsGameOver)
        {
            return;
        }

        // The lock timer gives the player time to adjust the piece before it locks
        lockTime += Time.deltaTime;

        int rotation = Input.GetKeyDown(KeyCode.Q) ? -1 : Input.GetKeyDown(KeyCode.E) ? 1 : 0;
        bool hardDrop = Input.GetKeyDown(KeyCode.Space);
        bool move = Time.time > moveTime && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S));
        bool step = Time.time > stepTime;

        // Most frames nothing happens, so skip redrawing the piece on the tilemap
        if (rotation == 0 && !hardDrop && !move && !step)
        {
            return;
        }

        board.Clear(this);

        if (rotation != 0)
        {
            Rotate(rotation);
        }

        if (hardDrop)
        {
            HardDrop();
            return;
        }

        if (move)
        {
            HandleMoveInputs();
        }

        // Step returns true when the piece locked and a new one has already been placed
        if (step && Step())
        {
            return;
        }

        board.Set(this);
    }

    private void HandleMoveInputs()
    {
        // Holding a key repeats the move every moveDelay seconds
        if (Input.GetKey(KeyCode.S) && Move(Vector2Int.down))
        {
            AddPoints(SoftDropPointsPerCell);
            stepTime = Time.time + stepDelay; // avoid a double move this frame
        }

        if (Input.GetKey(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }

    private bool Step()
    {
        stepTime = Time.time + stepDelay;
        Move(Vector2Int.down);

        // Once the piece has been inactive for too long it locks
        if (lockTime < lockDelay)
        {
            return false;
        }

        Lock();
        return true;
    }

    private void HardDrop()
    {
        int cellsDropped = 0;
        while (Move(Vector2Int.down))
        {
            cellsDropped++;
        }

        AddPoints(cellsDropped * HardDropPointsPerCell);
        Lock();
    }

    private void Lock()
    {
        board.PlaySound(board.lockSound);

        if (landEffect != null)
        {
            landEffect.Play();
        }

        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
    }

    private void AddPoints(int points)
    {
        if (board.scoreboard != null)
        {
            board.scoreboard.AddPoints(points);
        }
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position + (Vector3Int)translation;

        if (!board.IsValidPosition(this, newPosition))
        {
            return false;
        }

        position = newPosition;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;
        return true;
    }

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;
        rotationIndex = Mod(rotationIndex + direction, 4);
        ApplyRotation(direction);

        if (TestWallKicks(direction))
        {
            board.PlaySound(board.rotateSound);
        }
        else
        {
            rotationIndex = originalRotation;
            ApplyRotation(-direction);
        }
    }

    // Rotates every cell 90 degrees: (x, y) -> (y, -x) clockwise, (-y, x) counter-clockwise
    private void ApplyRotation(int direction)
    {
        bool offsetCenter = data.tetromino is Tetromino.I or Tetromino.O;

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int cell = cells[i];

            // "I" and "O" rotate around the corner between four cells instead of a cell's center
            cells[i] = offsetCenter
                ? new(Mathf.CeilToInt((cell.y - 0.5f) * direction), Mathf.CeilToInt((0.5f - cell.x) * direction), 0)
                : new(cell.y * direction, -cell.x * direction, 0);
        }
    }

    // Tries each wall kick offset so a rotation near a wall or the stack can still succeed
    private bool TestWallKicks(int direction)
    {
        Vector2Int[,] kicks = data.wallKicks;
        int kickIndex = Mod(rotationIndex * 2 - (direction < 0 ? 1 : 0), kicks.GetLength(0));

        for (int i = 0; i < kicks.GetLength(1); i++)
        {
            if (Move(kicks[kickIndex, i]))
            {
                return true;
            }
        }

        return false;
    }

    private static int Mod(int value, int length) => (value % length + length) % length;
}
