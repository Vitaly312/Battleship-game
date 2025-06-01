using System;
using System.Collections.Generic;
using System.Linq;
using BattleshipGame.Models.Logic;
namespace BattleshipGame.Models;

static class GameUtils
{

    private static int GetPossibleShipLocationsCount(BattleShipField field, int x, int y)
    {
        int count = 0;
        foreach (var size in field.ShipSizes)
        {
            for (int i = 0; i < size; i++)
            {
                if (y - i >= 0)
                {
                    var points = BoardOperations.GetShipPoints(Direction.Horizontal, x, y - i, size);
                    if (BoardOperations.CanLocate(field, points)) count++;
                }

                if (x - i >= 0)
                {
                    var points = BoardOperations.GetShipPoints(Direction.Vertical, x - i, y, size);
                    if (BoardOperations.CanLocate(field, points)) count++;
                }
            }
        }

        return count;
    }

    private static (int x, int y) GetRandomShotCoordinates(BattleShipField field)
    {
        if (field.GameIsFinished()) throw new ArgumentException("Field have not available fields");
        while (true)
        {
            int x = Random.Shared.Next(0, 10);
            int y = Random.Shared.Next(0, 10);
            if (field.IsCellShootable((x, y))) return (x, y);
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
                if (field.IsCellShootable((x: i, y: j)))
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

    
    public static bool IsInBounds(int x, int y)
    {
        return (0 <= x && x <= 9 && 0 <= y && y <= 9);
    }

    public static List<(int x, int y)> GetPossibleShipPointsByPoint(BattleShipField field, (int x, int y) point)
    {
        (int x, int y) = point;
        var shipPoints = ShipUtils.GetShipPoints(field, x, y, []);
        List<(int x, int y)> possiblePoints;
        if (shipPoints.Count == 1)
        {
            possiblePoints = [(x + 1, y), (x, y + 1), (x - 1, y), (x, y - 1)];
        }
        else
        {
            if (shipPoints[0].x == shipPoints[1].x)
            {
                var yCoordinates = shipPoints.Select(p => p.y).ToArray();
                possiblePoints = [ (x, yCoordinates.Min() - 1), (x, yCoordinates.Max() + 1)];
            }
            else
            {
                var xCoordinates = shipPoints.Select(p => p.x).ToArray();
                possiblePoints = [(xCoordinates.Min() - 1, y), (xCoordinates.Max() + 1, y)];
            }
        }

        return possiblePoints.FindAll(p => IsInBounds(p.x, p.y) && field.IsCellShootable(p));
    }
}

public class PlayerShotResult
{
    public enum GameMembers
    {
        Computer,
        Player
    };

    public GameMembers? Winner;
    public required IEnumerable<(int x, int y)> AffectedPoints;
    public bool GameIsFinished = false;
    public GameMembers Turn;
}