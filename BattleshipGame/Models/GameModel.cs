using System;
using System.Collections.Generic;
using BattleshipGame.Models.Logic;
namespace BattleshipGame.Models;

static class GameUtils
{
    static int GetPossibleShipLocationsCount(BattleShipField field, int x, int y)
    {
        int count = 0;
        foreach (var size in field.ShipSizes)
        {
            for (int i = 0; i < size; i++)
            {
                if (y - i >= 0)
                {
                    var points = BoardOperations.GetShipPoints(Direction.Horizontal, x, y - i, size);
                    if (BoardOperations.CanLocate(field.Field, points)) count++;
                }

                if (x - i >= 0)
                {
                    var points = BoardOperations.GetShipPoints(Direction.Vertical, x - i, y, size);
                    if (BoardOperations.CanLocate(field.Field, points)) count++;
                }
            }
        }

        return count;
    }

    public static (int x, int y) GetRandomShotCoordinates(BattleShipField field)
    {
        while (true)
        {
            int x = Random.Shared.Next(0, 10);
            int y = Random.Shared.Next(0, 10);
            if (field.Field[x, y] != PointStatus.Miss && field.Field[x, y] != PointStatus.Hit) return (x, y);

            if (GameIsFinished(field)) return (1, 1);
        }
    }

    public static (int x, int y) GetPlausibleShotCoordinates(BattleShipField field)
    {
        int maxPlacements = 0;
        (int x, int y) bestPoint = GetRandomShotCoordinates(field);
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (field.Field[i, j] != PointStatus.Miss && field.Field[i, j] != PointStatus.Hit)
                {
                    int placements = GetPossibleShipLocationsCount(field, i, j);
                    if (placements > maxPlacements)
                    {
                        maxPlacements = placements;
                        bestPoint = (i, j);
                    }
                }
            }
        }

        return bestPoint;
    }

    static public bool GameIsFinished(BattleShipField field)
    {
        for (int i = 0; i < 10; i++)
        for (int j = 0; j < 10; j++)
        {
            if (field.Field[i, j] == PointStatus.Ship) return false;
        }

        return true;
    }
    public static bool IsInBounds(int x, int y)
    {
        return (0 <= x && x <= 9 && 0 <= y && y <= 9);
    }

    public static List<(int x, int y)> GetPossibleShipPointsByPoint(BattleShipField field, (int x, int y) point)
    {
        (int x, int y) = point;
        var shipPoints = ShipUtils.GetShipPoints(field, x, y, new HashSet<(int x, int y)>());
        List<(int x, int y)> possiblePoints;
        if (shipPoints.Count == 1)
        {
            possiblePoints = new List<(int x, int y)>()
                { (x + 1, y), (x, y + 1), (x - 1, y), (x, y - 1) };
        }
        else
        {
            possiblePoints = shipPoints[0].x == shipPoints[1].x ?
                new List<(int x, int y)>() { (x, y+1), (x, y-1) }
                : new List<(int x, int y)>() { (x+1, y), (x-1, y) };
        }

        return possiblePoints.FindAll(
            p => IsInBounds(p.x, p.y)
                 && field.Field[p.x, p.y] != PointStatus.Miss
                 && field.Field[p.x, p.y] != PointStatus.Hit);
    }
}