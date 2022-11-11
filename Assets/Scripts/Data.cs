using System.Collections.Generic;
using UnityEngine;

public static class Data
{
    public static readonly float cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float sin = Mathf.Sin(Mathf.PI / 2f);
    public static readonly float[] RotationMatrix = new float[] { 0f, 1f, -1f, 0f };
    public static readonly float[] RotationMatrix45 = new float[] { 0.7f, 0.7f, -0.7f, 0.7f };


    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new Dictionary<Tetromino, Vector2Int[]>()
    {
        { Tetromino.O, new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int( 0, 1), new Vector2Int( 1, 0), new Vector2Int( 1, 1) } },
        { Tetromino.T, new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 1, 0) } },
        { Tetromino.I, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int(-1, 1), new Vector2Int( 1, 1), new Vector2Int( 2, 1) } },
        { Tetromino.V, new Vector2Int[] { new Vector2Int( 1, 1), new Vector2Int( 2, 1), new Vector2Int( 0, 0), new Vector2Int( 0,-1) } },
        { Tetromino.Y, new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 1, -1) } },

        { Tetromino.Z, new Vector2Int[] { new Vector2Int( 1, 1), new Vector2Int( 0, 1), new Vector2Int( 1, 0), new Vector2Int( 2, 0) } },
        { Tetromino.L, new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int( 1, 0), new Vector2Int(-1, 0), new Vector2Int(-1, 1) } },
        { Tetromino.S, new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(-1, 0), new Vector2Int( 1, 1), new Vector2Int( 2, 1) } },
        { Tetromino.J, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int(-1, 1), new Vector2Int( 1, 1), new Vector2Int( 2, 0) } },
        { Tetromino.R, new Vector2Int[] { new Vector2Int( 0, 1), new Vector2Int(-1, 0), new Vector2Int( 1, 1), new Vector2Int( 1, 0) } },
        { Tetromino.P, new Vector2Int[] { new Vector2Int( 0, 0), new Vector2Int(-1,-1), new Vector2Int( 1, 0), new Vector2Int( 1, 1) } },
    };

    public static readonly Vector2Int[] WallKicks_list = new Vector2Int[]                          // The order of attempts after rotation:
    {                                                                                              //               12
        new Vector2Int( 0, 0), new Vector2Int( 0,-1), new Vector2Int( 1,-1), new Vector2Int(-1,-1),//             9  7  8
        new Vector2Int( 0,-2), new Vector2Int( 1, 0), new Vector2Int(-1, 0), new Vector2Int( 0, 1),//          11 6  0  5 10
        new Vector2Int( 1, 1), new Vector2Int(-1, 1), new Vector2Int( 2, 0), new Vector2Int(-2, 0),//             3  1  2
        new Vector2Int( 0, 2)                                                                      //                4
    };

}