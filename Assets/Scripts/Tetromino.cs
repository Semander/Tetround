using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    O,
    T,
    I,
    V,
    Y,

    Z,
    L,
    S,
    J,
    R,
    P,
}

[System.Serializable]
public struct TetrominoData
{
    public Tile tile;
    public Tetromino tetromino;

    public Vector2Int[] cells { get; private set; }
    public Vector2Int[] wallKicks { get; private set; }

    public void Initialize()
    {
        cells = Data.Cells[tetromino];
        wallKicks = Data.WallKicks_list;
    }

}