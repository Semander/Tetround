using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using static UnityEditor.UIElements.ToolbarMenu;

public class BoardC : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    public int[] pieceWaves = new int[] { 0, 22, 0, 0, 0, 0, 22, 22, 6, 6 };
    public int nextPiece;

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
        int pieceNumber = Random.Range(0, 34);


        if (nextPiece >= pieceWaves.Length) { 
            nextPiece = 0;
            GameOver(); 
        } 

        pieceNumber = pieceWaves[nextPiece];
        nextPiece++;

        int shape;
        int variant;

        if (pieceNumber < 10) // O,T,I,V,Y blocks have 2 rotations
        {
            shape = pieceNumber / 2;
            variant = pieceNumber % 2;
        }
        else // Z,L,S,J,R,P blocks have 2 rotations and 2 mirror variants
        {
            pieceNumber -= 10; // Getting rid of first 10 indexes (5 shapes with 2 rot)
            shape = 5 + pieceNumber / 4;
            variant = pieceNumber % 4;
        }
        TetrominoData data = tetrominoes[shape];


        activePiece.Initialize(this, spawnPosition, data, variant);

        // Handle start variant:
        activePiece.TestWallKicks(1);
        if (variant % 2 == 1) // Rotate if 1 or 3
        {
            if (activePiece.data.tetromino == Tetromino.J || activePiece.data.tetromino == Tetromino.Z)
            {
                if (!activePiece.Rotate45(-1))
                    GameOver();
            }
            else
            {
                if (!activePiece.Rotate45(1))
                    GameOver();
            }
            activePiece.Move(Vector2Int.up);
            activePiece.Move(Vector2Int.up);
        }

        if ((variant / 2) % 2 == 1) // Mirror if 2 or 3
            activePiece.Mirror();




        if (IsValidPosition(activePiece, activePiece.position))
        {
            Set(activePiece);
        }
        else
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        tilemap.ClearAllTiles();

        // Do anything else you want on game over here..
    }

    public void Set(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void MoveBoard(int shift)
    {
        RectInt bounds = Bounds;

        int start = bounds.xMin;
        int end = bounds.xMax;
        int down = bounds.yMin;
        int up = bounds.yMax;
        int height = boardSize.y;

        bool neg = shift > 0;
        if (!neg)
        {
            shift = -shift;
        }

        TileBase[,] teleport = new TileBase[shift, height];
        if (neg)
        {
            // Create a set of the part that will be teleported
            for (int col = 0; col < shift; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    Vector3Int position = new Vector3Int(start + col, down + row, 0);
                    teleport[col, row] = tilemap.GetTile(position);
                }
            }

            // Shift all the other tiles on the 'shift' distance
            for (int col = start + shift; col < end; col++)
            {
                for (int row = down; row < up; row++)
                {
                    Vector3Int oldPosition = new Vector3Int(col, row, 0);
                    Vector3Int newPosition = new Vector3Int(col - shift, row, 0);
                    tilemap.SetTile(newPosition, tilemap.GetTile(oldPosition));
                }
            }

            // Shift all the other tiles on the 'shift' distance
            for (int col = 0; col < shift; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    Vector3Int position = new Vector3Int(end - shift + col, down + row, 0);
                    tilemap.SetTile(position, teleport[col, row]);
                }
            }
        }
        else
        {
            // Create a set of the part that will be teleported
            for (int col = 0; col < shift; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    Vector3Int position = new Vector3Int(end - shift + col, down + row, 0);
                    teleport[col, row] = tilemap.GetTile(position);
                }
            }

            // Shift all the other tiles on the 'shift' distance
            for (int col = end - shift - 1; col >= start; col--)
            {
                for (int row = down; row < up; row++)
                {
                    Vector3Int oldPosition = new Vector3Int(col, row, 0);
                    Vector3Int newPosition = new Vector3Int(col + shift, row, 0);
                    tilemap.SetTile(newPosition, tilemap.GetTile(oldPosition));
                }
            }

            // Shift all the other tiles on the 'shift' distance
            for (int col = 0; col < shift; col++)
            {
                for (int row = 0; row < height; row++)
                {
                    Vector3Int position = new Vector3Int(start + col, down + row, 0);
                    tilemap.SetTile(position, teleport[col, row]);
                }
            }
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        // The position is only valid if every cell is valid
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            // An out of bounds tile is invalid
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            // A tile already occupies the position, thus invalid
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        RectInt bounds = Bounds;
        int row = bounds.yMin;

        // Clear from bottom to top
        while (row < bounds.yMax)
        {
            // Only advance to the next row if the current is not cleared
            // because the tiles above will fall down when a row is cleared
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    public bool IsLineFull(int row)
    {
        RectInt bounds = Bounds;

        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);

            // The line is not full if a tile is missing
            if (!tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    public void LineClear(int row)
    {
        RectInt bounds = Bounds;

        // Clear all tiles in the row
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int position = new Vector3Int(col, row, 0);
            tilemap.SetTile(position, null);
        }

        // Shift every row above down one
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

}