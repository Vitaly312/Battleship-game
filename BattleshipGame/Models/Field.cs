using System.Collections.Generic;

namespace BattleshipGame.Models;

public enum PointStatus
{
    Empty,
    Ship, // живой корабль
    Hit, // подбит 
    Miss,
    Sunk // уже убит (были попадания во все клетки)
    
}

public enum Direction
{
    Horizontal,
    Vertical
}

public class BattleShipField
{
    public PointStatus[,] Field {get; } = new PointStatus[10, 10];
    public List<int> ShipSizes = new() { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };

    public bool IsCellShootable((int x, int y) point)
    {
        return Field[point.x, point.y] != PointStatus.Miss
               && Field[point.x, point.y] != PointStatus.Hit
               && Field[point.x, point.y] != PointStatus.Sunk;
    }
    public bool GameIsFinished()
    {
        return ShipSizes.Count == 0;
        // for (int i = 0; i < 10; i++)
        // for (int j = 0; j < 10; j++)
        // {
        //     if (Field[i, j] == PointStatus.Ship) return false;
        // }
        //
        // return true;
    }
}
