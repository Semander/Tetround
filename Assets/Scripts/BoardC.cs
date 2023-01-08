using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = System.Random;
using System;
using UnityEngine.SceneManagement;

public class BoardC : MonoBehaviour
{
    public Random rnd = new Random();
    public Tilemap tilemap { get; private set; }
    public Piece activePiece { get; private set; }

    public TetrominoData[] tetrominoes;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    public Vector3Int spawnPosition = new Vector3Int(-1, 8, 0);

    public Sprite[] blockSprites;
    public SpriteRenderer[] nextBlocks;
    public SpriteRenderer holdBlock;

    public GameSettings myGameSettings = new GameSettings();
    public GameMode myGameMode = new GameMode();
    public List<Wave> myWaveList = new List<Wave>();
    public Wave myWave = new Wave();

    [System.NonSerialized]
    public List<List<int>> Packets = new List<List<int>>()
    {
        new List<int>() { },
    };

    public List<int> PossibleShapesPackets = new List<int>();

    public List<int> possibleShapes = new List<int>();

    [System.NonSerialized]
    public List<int> pieceWaves = new List<int> { };
    public int currentWave = 0;
    public int holdPiece = -1;
    public int nextPiece = 0;

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

        addPieceWaves();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
        SpawnPieceFromList();

    }

    private void Start()
    {
    }

    int i = 0;
    private void addPieceWaves()
    {
        string json = File.ReadAllText(Application.dataPath + "/SaveData/" + FileNameController.filePath + ".json");
        myGameSettings = JsonUtility.FromJson<GameSettings>(json);

        Debug.Log(FileNameController.filePath);


        myGameMode = myGameSettings.gameMode;
        myWaveList = myGameSettings.waveList;

        if (myWaveList == null) SceneManager.LoadScene("User Level");
        else
        {
            for (int j = 0; j < myWaveList.Count; j++)
            {
                PossibleShapesPackets.Clear();
                possibleShapes.Clear();

                myWave = myWaveList[j];

                int waveAmount = myWave.waveAmount;
                long number = myWave.shapes;

                //Debug.Log("Wave amount: " + waveAmount + ", number: " + number);


                for (i = 0; number > 0; number >>= 1)// parse into bits positions
                {
                    if (number % 2 == 1)
                    {
                        PossibleShapesPackets.Add(i);
                    }
                    i += 1;
                }

                int amountOfShapesInPacket = PossibleShapesPackets.Count;
                if (amountOfShapesInPacket == 0) continue;

                int repeatPacket = (waveAmount + amountOfShapesInPacket - 1) / amountOfShapesInPacket;

                for (i = 0; i < repeatPacket; i++) // repeat packet if amount of waves is more than shapes in the packet
                {
                    possibleShapes.AddRange(PossibleShapesPackets);
                }
                pieceWaves.AddRange(possibleShapes.OrderBy(x => rnd.Next()).Take(waveAmount));// get "waveAmount" amount of pieces from the list of possible shapes
            }
        }
        

    }

    public void SpawnPieceFromList()
    {
        int pieceNumber = rnd.Next(0, 34);


        if (nextPiece >= pieceWaves.Count)
        {
            if (holdPiece >= 0) // If there is a block in the "hold" section
            {
                SpawnPiece(holdPiece);
                setHoldPiece(-1);
            }
            else
            {
                nextPiece = 0;
                GameWin();
            }
        }
        else
        {
            pieceNumber = pieceWaves[nextPiece];
            nextPiece++;
            changeNext();

            SpawnPiece(pieceNumber);
        }
    }

    public void SpawnPiece(int pieceIndex)
    {
        int shape;
        int variant;

        if (pieceIndex < 10) // O,T,I,V,Y blocks have 2 rotations
        {
            shape = pieceIndex / 2;
            variant = pieceIndex % 2;
        }
        else // Z,L,S,J,R,P blocks have 2 rotations and 2 mirror variants (4 variants)
        {
            pieceIndex -= 10; // Getting rid of first 10 indexes (5 shapes with 2 rot)
            shape = 5 + pieceIndex / 4;
            variant = pieceIndex % 4;
            pieceIndex += 10; // Resetting index
        }
        TetrominoData data = tetrominoes[shape];

        activePiece.Initialize(this, spawnPosition, data, pieceIndex);

        // Handle start variant:
        activePiece.TestWallKicks(1);
        if (variant % 2 == 1) // Rotate if 1 or 3
        {
            if (activePiece.data.tetromino == Tetromino.J || activePiece.data.tetromino == Tetromino.Z)
            {
                if (!activePiece.Rotate45(-1))
                    GameLost();
            }
            else
            {
                if (!activePiece.Rotate45(1))
                    GameLost();
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
            GameLost();
        }
    }

    private void changeNext()
    {
        int pieceNumber;

        int maxNextShapes = Mathf.Min(pieceWaves.Count - nextPiece, 4);
        for (int i = 0; i < 4; i++)
        {
            if (i < maxNextShapes)
            {
                pieceNumber = pieceWaves[nextPiece + i];
                nextBlocks[i].sprite = blockSprites[pieceNumber];
            }
            else
            {
                nextBlocks[i].sprite = null;
            }
        }
    }

    public void Hold(int newHold)
    {
        Clear(activePiece); // Delete last piece that was on the board
        if (holdPiece < 0)
        {
            SpawnPieceFromList(); // Add next piece if hold is empty
        }
        else
        {
            SpawnPiece(holdPiece); // Add holded piece if possible
        }
        setHoldPiece(newHold);
    }

    public void setHoldPiece(int newHold)
    {
        if (newHold >= 0)
            holdBlock.sprite = blockSprites[newHold];
        else
            holdBlock.sprite = null;
        holdPiece = newHold;

    }

    public void GameLost()
    {
        tilemap.ClearAllTiles();

        // Do anything else you want on game over here..
    }

    public void GameWin()
    {
        SceneManager.LoadScene("Menu");
        Debug.Log("Finish the G");
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