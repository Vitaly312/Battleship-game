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
    public PointStatus[,] Field = new PointStatus[10, 10];
    public List<int> ShipSizes = new() { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
    
}
