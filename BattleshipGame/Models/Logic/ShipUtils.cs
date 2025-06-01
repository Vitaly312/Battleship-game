using System.Collections.Generic;

namespace BattleshipGame.Models.Logic;

public static class ShipUtils
{

    /// <summary>
    /// Kill ship(sunk all points on ship and miss all neighbor points) by one point of this ship
    /// </summary>
    /// <param name="field"></param>
    /// <param name="shipPoints"></param>
    /// <returns>All affected points</returns>
    public static HashSet<(int x, int y)> KillShip(BattleShipField field,
        List<(int x, int y)> shipPoints)
    {
        var affectedPoints = new HashSet<(int x, int y)>(shipPoints);
        field.ShipSizes.Remove(affectedPoints.Count);
        FillShipPoints(field, shipPoints);
        affectedPoints.UnionWith(FillNearbyPoints(field, shipPoints));
        return affectedPoints;
    }


    /// <returns>List of points that change status(from Empty to Miss)</returns>
    private static List<(int x, int y)> FillNearbyPoints(BattleShipField field, List<(int x, int y)> points)
    {
        List<(int x, int y)> affectedPoints = [];
        foreach (var point in points)
        {
            var pointsToFill = new (int x, int y)[]
            {
                (point.x - 1, point.y), (point.x, point.y - 1), (point.x + 1, point.y), (point.x, point.y + 1),
                (point.x - 1, point.y - 1), (point.x - 1, point.y + 1), (point.x + 1, point.y - 1),
                (point.x + 1, point.y + 1)
            };
            foreach (var pointToFill in pointsToFill)
            {
                if (!GameUtils.IsInBounds(pointToFill.x, pointToFill.y)) continue;
                if (field.Field[pointToFill.x, pointToFill.y] == PointStatus.Empty)
                {
                    field.Field[pointToFill.x, pointToFill.y] = PointStatus.Miss;
                    affectedPoints.Add((pointToFill.x, pointToFill.y));
                }
            }
        }

        return affectedPoints;
    }
    public static List<(int x, int y)> GetShipPoints(
        BattleShipField field, int x, int y, HashSet<(int x, int y)> visited)
    {
        var shipPoints = new List<(int x, int y)>() { (x, y) };
        var points = new (int x, int y)[] { (x - 1, y), (x, y - 1), (x + 1, y), (x, y + 1) };
        foreach (var point in points)
        {
            if (!GameUtils.IsInBounds(point.x, point.y)) continue;
            if (field.Field[point.x, point.y] != PointStatus.Ship
                && field.Field[point.x, point.y] != PointStatus.Hit) continue;
            if (visited.Contains(point)) continue;
            shipPoints.Add(point);
            visited.Add(point);
            shipPoints.AddRange(GetShipPoints(field, point.x, point.y, visited));
        }

        return shipPoints;
    }
    

    private static void FillShipPoints(
        BattleShipField field, List<(int x, int y)> points)
    {
        foreach (var point in points)
        {
            field.Field[point.x, point.y] = PointStatus.Sunk;
        }
    }
}