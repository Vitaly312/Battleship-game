using System;

namespace BattleshipGame.Models.Logic;

public class BoardOperations
{
    public static bool CanLocate(PointStatus[,] field, (int x, int y)[] shipPoints,
        bool isPlacementCheck = false)
    {
        foreach ((int x, int y) point in shipPoints)
        {
            if (point.x < 0 || point.x > 9 || point.y < 0 || point.y > 9) return false;
            if (field[point.x, point.y] == PointStatus.Miss
                || field[point.x, point.y] == PointStatus.Hit) return false;
            if (isPlacementCheck)
            {
                var neighbors = new (int x, int y)[]
                {
                    (point.x - 1, point.y), (point.x + 1, point.y),
                    (point.x, point.y - 1), (point.x, point.y + 1),
                    (point.x - 1, point.y - 1), (point.x - 1, point.y + 1),
                    (point.x + 1, point.y - 1), (point.x + 1, point.y + 1)
                };
                foreach (var neighbor in neighbors)
                {
                    if (neighbor.x >= 0 && neighbor.x < 10 && neighbor.y >= 0 && neighbor.y < 10)
                    {
                        if (field[neighbor.x, neighbor.y] == PointStatus.Ship) return false;
                    }
                }
            }
        }

        return true;
    }

    public static (int x, int y)[] GetShipPoints(Direction direction, int x, int y, int shipLen)
    {
        (int x, int y)[] shipPoints = new (int x, int y)[shipLen];
        for (int i = 0; i < shipLen; i++)
        {
            if (direction == Direction.Horizontal)
            {
                shipPoints[i] = (x, y + i);
            }
            else
            {
                shipPoints[i] = (x + i, y);
            }
        }

        return shipPoints;
    }

    public static void LocateShipsOnBoard(BattleShipField field)
    {
        foreach (var shipSize in field.ShipSizes)
        {
            while (true)
            {
                int x = Random.Shared.Next(0, 10);
                int y = Random.Shared.Next(0, 10);
                Direction direction = (Direction)Random.Shared.Next(0, 2);
                var points = GetShipPoints(direction, x, y, shipLen: shipSize);
                if (CanLocate(field.Field, points, true))
                {
                    foreach (var point in points)
                    {
                        field.Field[point.x, point.y] = PointStatus.Ship;
                    }

                    break;
                }
            }
        }
    }
}