using System;

namespace BattleshipGame.Models.Logic;

public class BoardOperations
{
    public static bool CanLocate(BattleShipField field, (int x, int y)[] shipPoints,
        bool isPlacementCheck = false)
    {
        foreach ((int x, int y) point in shipPoints)
        {
            if (!GameUtils.IsInBounds(point.x, point.y)) return false;
            if (!field.IsCellShootable(point)) return false;
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
                    if (GameUtils.IsInBounds(neighbor.x, neighbor.y))
                    {
                        if (field.Field[neighbor.x, neighbor.y] == PointStatus.Ship) return false;
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
                if (CanLocate(field, points, true))
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