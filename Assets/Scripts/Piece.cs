using UnityEngine;

public class Piece : MonoBehaviour
{
    private const int SoftDropPointsPerCell = 1;
    private const int HardDropPointsPerCell = 2;

    public Board board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }

    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;
    public ParticleSystem landEffect;

    private float stepTime;
    private float moveTime;
    private float lockTime;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        this.data = data;
        this.board = board;
        this.position = position;

        rotationIndex = 0;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        if (board.IsGameOver())
        {
            return;
        }

        board.Clear(this);

        // The lock timer gives the player time to adjust the piece before it locks
        lockTime += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
            return; // a new piece has spawned; nothing left to do for this one
        }

        // Held keys only repeat after the move delay so the piece does not move too fast
        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        if (Time.time > stepTime)
        {
            Step();

            if (board.IsGameOver())
            {
                return;
            }
        }

        board.Set(this);
    }

    private void HandleMoveInputs()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down))
            {
                board.scoreboard?.AddDropPoints(SoftDropPointsPerCell);
                stepTime = Time.time + stepDelay; // avoid a double move this frame
            }
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

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        Move(Vector2Int.down);

        // Once the piece has been inactive for too long it locks
        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    private void HardDrop()
    {
        int cellsDropped = 0;

        while (Move(Vector2Int.down))
        {
            cellsDropped++;
        }

        board.scoreboard?.AddDropPoints(cellsDropped * HardDropPointsPerCell);
        Lock();
    }

    public void Lock()
    {
        board.PlaySound(board.lockSound);

        if (landEffect != null)
        {
            landEffect.Play();
        }

        board.Set(this);
        board.ClearLines();
        board.scoreboard?.PieceLocked();
        board.SpawnPiece();
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
        // Remember the rotation so it can be reverted if every wall kick fails
        int originalRotation = rotationIndex;

        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (TestWallKicks(rotationIndex, direction))
        {
            board.PlaySound(board.rotateSound);
        }
        else
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];
            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" rotate around an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    // Tries each wall kick offset so a rotation near a wall or stack can still succeed
    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            if (Move(data.wallKicks[wallKickIndex, i]))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }

        return min + (input - min) % (max - min);
    }
}
