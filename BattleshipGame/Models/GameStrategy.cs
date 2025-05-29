using System.Collections.Generic;

namespace BattleshipGame.Models;

public class GameStrategy
{
    public BattleShipField Field;
    private (int x, int y) _lastHit;
    private bool _shootingAtShip;
    private BattleShipField _userField;
    

    public GameStrategy()
    {
        Field = new BattleShipField();
        _userField = new BattleShipField();
        Logic.BoardOperations.LocateShipsOnBoard(Field);
    }

    public (int x, int y) GetShotCoordinates()
    {
        if (_shootingAtShip)
        {
            _lastHit = GameUtils.GetPossibleShipPointsByPoint(_userField, _lastHit)[0];
        } else
        {
            _lastHit = GameUtils.GetPlausibleShotCoordinates(_userField);
        }
        return _lastHit;
    }

    public void SetShotResult(PointStatus status) // Only Hit, Miss, Sunk
    {
        if (status == PointStatus.Miss && _shootingAtShip) _shootingAtShip = false;
        if (status == PointStatus.Hit) _shootingAtShip = true;
        else if (status == PointStatus.Sunk)
        {
            Logic.ShipUtils.KillShip(_userField, _lastHit.x, _lastHit.y);
            _shootingAtShip = false;
        }
        _userField.Field[_lastHit.x, _lastHit.y] = status;
    }

    public void ShotAndSunkIfNeed((int x, int y) point)
    {
        if (Field.Field[point.x, point.y] == PointStatus.Ship)
        {
            
            var shipPoints = Logic.ShipUtils.GetShipPoints(Field, point.x, point.y,
                new HashSet<(int x, int y)>());
            Field.Field[point.x, point.y] = PointStatus.Hit;
            if (shipPoints.FindAll(p => Field.Field[p.x, p.y] == PointStatus.Ship).Count == 0)
            {
                Logic.ShipUtils.KillShip(Field, point.x, point.y);
            }
        }
    }
}