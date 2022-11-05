using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking.Types;

public class Piece : MonoBehaviour
{
    public GameObject otherGameObject;
    
    public BoardC board { get; private set; }
    public Transform center { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int[] cellsRelative { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }
    public int mirrorIndex { get; private set; }

    public float stepDelay = 1f;
    public float moveDelay = 0.1f;
    public float lockDelay = 0.5f;

    private float stepTime;
    private float moveTime;
    private float lockTime;

    public void Initialize(BoardC board, Vector3Int position, TetrominoData data, int variant)
    {
        center = otherGameObject.GetComponent<Transform>();
        this.data = data;
        this.board = board;
        this.position = position;

        MoveCenterPos();

        rotationIndex = 0;
        mirrorIndex = 1;
        stepTime = Time.time + stepDelay;
        moveTime = Time.time + moveDelay;
        lockTime = 0f;

        if (cells == null)
        {
            cells = new Vector3Int[data.cells.Length];
            cellsRelative = new Vector3Int[data.cells.Length - 1];
        }

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Update()
    {
        board.Clear(this);

        MoveCenterPos();

        // We use a timer to allow the player to make adjustments to the piece
        // before it locks in place
        lockTime += Time.deltaTime;

        if      (Input.GetKeyDown(KeyCode.Z)) // Handle 45 degree rotation
        {
            Rotate45(-1);
            Debug.Log(rotationIndex);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Rotate45(1);
            Debug.Log(rotationIndex);
        }
        else if (Input.GetKeyDown(KeyCode.C)) // Handle 90 degree rotation
        {
            Rotate(-1);
            Debug.Log(rotationIndex);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            Rotate(1);
            Debug.Log(rotationIndex);
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            Rotate180();
            Debug.Log(rotationIndex);
        }

        // Handle mirror reflecction
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Mirror();
        }

        // Handle hard drop
        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }

        // Allow the player to hold movement keys but only after a move delay
        // so it does not move too fast
        if (Time.time > moveTime)
        {
            HandleMoveInputs();
        }

        // Advance the piece to the next row every x seconds
        if (Time.time > stepTime)
        {
            Step();
        }
        
        board.Set(this);
    }

    private void HandleMoveInputs()
    {
        // Soft drop movement
        if (Input.GetKey(KeyCode.DownArrow))
        {
            if (Move(Vector2Int.down))
            {
                // Update the step time to prevent double movement
                stepTime = Time.time + stepDelay;
            }
            else Lock();
        }

        // Left/right movement
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (!Move(Vector2Int.left))
            {
                if (!Move(new Vector2Int(-1,-1))) // Try left and down
                {
                    Move(new Vector2Int(-1, 1)); // Try left and up
                }
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (!Move(Vector2Int.right))
            {
                if (!Move(new Vector2Int( 1,-1))) // Try right and down
                {
                    Move(new Vector2Int( 1, 1)); // Try right and up
                }
            }
        }
    }

    private void Step()
    {
        stepTime = Time.time + stepDelay;

        // Step down to the next row
        Move(Vector2Int.down);

        // Once the piece has been inactive for too long it becomes locked
        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    private void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            continue;
        }

    }

    private void Lock()
    {
        board.Set(this);
        board.ClearLines();
        board.SpawnPiece();
    }

    public bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        // Only save the movement if the new position is valid
        if (valid)
        {
            if (translation.x != 0)
            {
                board.MoveBoard(translation.x);
                newPosition.x -= translation.x;
            }

            position = newPosition;
            moveTime = Time.time + moveDelay;
            lockTime = 0f; // reset
        }

        return valid;
    }

    public bool Rotate(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = rotationIndex;

        // Rotate all of the cells using a rotation matrix
        if (data.tetromino != Tetromino.O)
            rotationIndex = Wrap(rotationIndex + 2 * direction * mirrorIndex, 0, 8);
        ApplyRotationMatrix(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
            return false;
        }

        return true;
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        // Rotate all of the cells using the rotation matrix
        if (data.tetromino != Tetromino.O)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3 cell = cells[i];

                int x, y;
                switch (data.tetromino)
                {
                    case Tetromino.I:
                    case Tetromino.V:
                    case Tetromino.Z:
                    case Tetromino.S:
                    case Tetromino.J:
                        // "I" is rotated from an offset center point
                        cell.x -= 0.5f;
                        cell.y -= 0.5f;
                        break;
                }
                x = Mathf.CeilToInt((cell.x * matrix[0]) + (cell.y * matrix[1] * direction));
                y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3]));
                cells[i].x = x;
                cells[i].y = y;
            }
        }
    }

    public bool Rotate45(int direction)
    {
        // Store the current rotation in case the rotation fails
        // and we need to revert
        int originalRotation = rotationIndex;

        // Rotate all of the cells using a rotation matrix
        if (data.tetromino != Tetromino.O)
            rotationIndex = Wrap(rotationIndex + direction * mirrorIndex, 0, 8);
        else
            rotationIndex = Wrap(rotationIndex + direction * mirrorIndex, 0, 2);
        ApplyRotationMatrix45(direction);

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix45(-direction);
            return false;
        }
        return true;
    }

    private void ApplyRotationMatrix45(int direction)
    {
        float[] matrix = Data.RotationMatrix45;
        float[] matrix1 = Data.RotationMatrix;

        Vector3 cent;
        Vector3Int cell1, cell2, cell3;
        int x, y;
        if (data.tetromino == Tetromino.O)
        {
            if (rotationIndex % 2 == 0)//if next rotation is normal (not diagonal)
            {
                cells = new Vector3Int[] { new Vector3Int( 0, 0, 0), new Vector3Int( 0, 1, 0), new Vector3Int( 1, 0, 0), new Vector3Int( 1, 1, 0) };
            } 
            else
            {
                cells = new Vector3Int[] { new Vector3Int(0,-1, 0), new Vector3Int(-1, 0, 0), new Vector3Int( 1, 0, 0), new Vector3Int( 0, 1, 0) };
            }
        }
        else if (data.tetromino == Tetromino.T || data.tetromino == Tetromino.Y)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3 cell = cells[i];

                x = Mathf.RoundToInt((cell.x * matrix[0]) + (cell.y * matrix[1] * direction));
                y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3]));
                cells[i] = new Vector3Int(x, y, 0);
            }
        }
        else
        {
            //relative to the block '2'
            cell3 = cells[3] - cells[2];
            x = Mathf.RoundToInt((cell3.x * matrix[0]) + (cell3.y * matrix[1] * direction));
            y = Mathf.RoundToInt((cell3.x * matrix[2] * direction) + (cell3.y * matrix[3]));
            cell3.x = x;
            cell3.y = y;

            cell2 = cells[2] - cells[0];
            x = Mathf.RoundToInt((cell2.x * matrix[0]) + (cell2.y * matrix[1] * direction));
            y = Mathf.RoundToInt((cell2.x * matrix[2] * direction) + (cell2.y * matrix[3]));
            cell2.x = x;
            cell2.y = y;

            cell1 = cells[1] - cells[0];
            x = Mathf.RoundToInt((cell1.x * matrix[0]) + (cell1.y * matrix[1] * direction));
            y = Mathf.RoundToInt((cell1.x * matrix[2] * direction) + (cell1.y * matrix[3]));
            cell1.x = x;
            cell1.y = y;

            // Rotate the central cell
            cent = cells[0];
            if (rotationIndex % 2 == (direction * mirrorIndex > 0 ? 0 : 1)) // If next rotation is not diagonal
            {
                switch (data.tetromino)
                {
                    case Tetromino.I:
                    case Tetromino.V:
                    case Tetromino.Z:
                    case Tetromino.S:
                    case Tetromino.J:
                    // "I" is rotated from an offset center point
                    cent.x -= 0.5f;
                    cent.y -= 0.5f;
                    break;
                }
                x = Mathf.CeilToInt((cent.x * matrix1[0]) + (cent.y * matrix1[1] * direction));
                y = Mathf.CeilToInt((cent.x * matrix1[2] * direction) + (cent.y * matrix1[3]));
                cells[0].x = x;
                cells[0].y = y;
            }

            // Apply the relative positions of the other cells
            cells[1] = cell1 + cells[0];
            cells[2] = cell2 + cells[0];
            //relative to the block '2'
            cells[3] = cell3 + cells[2];
        }
    }

    public bool Mirror()
    {
        
        mirrorIndex *= -1;

        ApplyMirroring();

        // Revert the rotation if the wall kick tests fail
        if (!TestWallKicks(1))
        {
            mirrorIndex *= -1;
            ApplyMirroring();
            return false;
        }
        return true;
    }

    private void ApplyMirroring()
    {
        // Rotate all of the cells using the rotation matrix
        if (data.tetromino != Tetromino.O)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3 cell = cells[i];

                switch (data.tetromino)
                {
                    case Tetromino.I:
                    case Tetromino.V:
                    case Tetromino.Z:
                    case Tetromino.S:
                    case Tetromino.J:
                        // "I", "V", "Z", "S", "J" is rotated from an offset center point
                        cell.x -= 0.5f;
                        cells[i].x = Mathf.CeilToInt(-cell.x + 0.5f);
                        break;
                    default:
                        cells[i].x = Mathf.CeilToInt(-cell.x);
                        break;
                }
            }
        }
    }

    private bool Rotate180()
    {

        if (data.tetromino != Tetromino.O)
        {
            if (data.tetromino != Tetromino.O)
                rotationIndex = Wrap(rotationIndex + 4, 0, 8);

            ApplyRotation180();

            // Revert the rotation if the wall kick tests fail
            if (!TestWallKicks(1))
            {
                if (data.tetromino != Tetromino.O)
                    rotationIndex = Wrap(rotationIndex + 4, 0, 8);

                ApplyRotation180();

                return false;
            }
        }
        return true;
    }

    private void ApplyRotation180()
    {
        // Rotate all of the cells using the rotation matrix
        if (data.tetromino != Tetromino.O)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3 cell = cells[i];

                switch (data.tetromino)
                {
                    case Tetromino.I:
                    case Tetromino.V:
                    case Tetromino.Z:
                    case Tetromino.S:
                    case Tetromino.J:
                        // "I", "V", "Z", "S", "J" is rotated from an offset center point
                        cell.x -= 0.5f;
                        cell.y -= 0.5f;
                        cells[i].x = Mathf.CeilToInt(-cell.x + 0.5f);
                        cells[i].y = Mathf.CeilToInt(-cell.y + 0.5f);
                        break;
                    default:
                        cells[i].x = Mathf.CeilToInt(-cell.x);
                        cells[i].y = Mathf.CeilToInt(-cell.y);
                        break;
                }
            }
        }
    }

    public bool TestWallKicks(int direction)
    {
        int wallKickIndex = direction * mirrorIndex;
        
        // Test all possible positions
        for (int i = 0; i < data.wallKicks.GetLength(0); i++)
        {
            Vector2Int translation = data.wallKicks[i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private void MoveCenterPos()
    {
        Vector3 centerpos = position;
        switch (data.tetromino)
        {
            case Tetromino.O:
                if (rotationIndex == 0)
                {
                    centerpos.x += 1f;
                    centerpos.y += 1f;
                }
                else
                {
                    centerpos.x += .5f;
                    centerpos.y += .5f;
                }
                break;
            case Tetromino.I:
            case Tetromino.V:
            case Tetromino.Z:
            case Tetromino.S:
            case Tetromino.J:
                centerpos.x += 1f;
                centerpos.y += 1f;
                break;
            default:
                centerpos.x += 0.5f;
                centerpos.y += 0.5f;
                break;
        }
        center.position = centerpos;
    }


    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

}