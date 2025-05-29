using System.Collections.Generic;


namespace BattleshipGame.Models.Logic;

public static class ShipUtils
{

    /// <summary>
    /// Kill ship(sunk all points on ship and miss all neighbor points) by one point of this ship
    /// </summary>
    /// <param name="field"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>Size of ship</returns>
    public static int KillShip(BattleShipField field, int x, int y)
    {
        var shipPoints = GetShipPoints(field, x, y, new HashSet<(int x, int y)>() { (x, y) });
        FillShipPoints(field, shipPoints);
        FillNearbyPoints(field, shipPoints);
        return shipPoints.Count;
    }

    private static void FillNearbyPoints(BattleShipField field, List<(int x, int y)> points)
    {
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
                if (pointToFill.x > 9 || pointToFill.x < 0 || pointToFill.y > 9 || pointToFill.y < 0) continue;
                if (field.Field[pointToFill.x, pointToFill.y] == PointStatus.Empty)
                {
                    field.Field[pointToFill.x, pointToFill.y] = PointStatus.Miss;
                }
            }
        }
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