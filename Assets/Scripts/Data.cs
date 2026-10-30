using System.Collections.Generic;
using UnityEngine;

// Cell layouts and SRS wall kick tables for each tetromino, taken from the Tetris wiki.
public static class Data
{
    public static readonly Dictionary<Tetromino, Vector2Int[]> Cells = new()
    {
        { Tetromino.I, new Vector2Int[] { new(-1, 1), new( 0, 1), new( 1, 1), new( 2, 1) } },
        { Tetromino.J, new Vector2Int[] { new(-1, 1), new(-1, 0), new( 0, 0), new( 1, 0) } },
        { Tetromino.L, new Vector2Int[] { new( 1, 1), new(-1, 0), new( 0, 0), new( 1, 0) } },
        { Tetromino.O, new Vector2Int[] { new( 0, 1), new( 1, 1), new( 0, 0), new( 1, 0) } },
        { Tetromino.S, new Vector2Int[] { new( 0, 1), new( 1, 1), new(-1, 0), new( 0, 0) } },
        { Tetromino.T, new Vector2Int[] { new( 0, 1), new(-1, 0), new( 0, 0), new( 1, 0) } },
        { Tetromino.Z, new Vector2Int[] { new(-1, 1), new( 0, 1), new( 0, 0), new( 1, 0) } },
    };

    private static readonly Vector2Int[,] WallKicksI = {
        { new(0, 0), new(-2, 0), new( 1, 0), new(-2,-1), new( 1, 2) },
        { new(0, 0), new( 2, 0), new(-1, 0), new( 2, 1), new(-1,-2) },
        { new(0, 0), new(-1, 0), new( 2, 0), new(-1, 2), new( 2,-1) },
        { new(0, 0), new( 1, 0), new(-2, 0), new( 1,-2), new(-2, 1) },
        { new(0, 0), new( 2, 0), new(-1, 0), new( 2, 1), new(-1,-2) },
        { new(0, 0), new(-2, 0), new( 1, 0), new(-2,-1), new( 1, 2) },
        { new(0, 0), new( 1, 0), new(-2, 0), new( 1,-2), new(-2, 1) },
        { new(0, 0), new(-1, 0), new( 2, 0), new(-1, 2), new( 2,-1) },
    };

    private static readonly Vector2Int[,] WallKicksJLOSTZ = {
        { new(0, 0), new(-1, 0), new(-1, 1), new(0,-2), new(-1,-2) },
        { new(0, 0), new( 1, 0), new( 1,-1), new(0, 2), new( 1, 2) },
        { new(0, 0), new( 1, 0), new( 1,-1), new(0, 2), new( 1, 2) },
        { new(0, 0), new(-1, 0), new(-1, 1), new(0,-2), new(-1,-2) },
        { new(0, 0), new( 1, 0), new( 1, 1), new(0,-2), new( 1,-2) },
        { new(0, 0), new(-1, 0), new(-1,-1), new(0, 2), new(-1, 2) },
        { new(0, 0), new(-1, 0), new(-1,-1), new(0, 2), new(-1, 2) },
        { new(0, 0), new( 1, 0), new( 1, 1), new(0,-2), new( 1,-2) },
    };

    // Only the I piece has its own kick table; every other piece shares one
    public static Vector2Int[,] GetWallKicks(Tetromino tetromino) =>
        tetromino == Tetromino.I ? WallKicksI : WallKicksJLOSTZ;
}
