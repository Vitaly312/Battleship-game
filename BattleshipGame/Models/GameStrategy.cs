using System;
using System.Collections.Generic;

namespace BattleshipGame.Models;

public class GameStrategy
{
    public BattleShipField ComputerField;
    private (int x, int y) _lastShot, _lastHit;
    private bool _shootingAtShip;
    private BattleShipField _userField;
    

    public GameStrategy()
    {
        ComputerField = new BattleShipField();
        _userField = new BattleShipField();
        Logic.BoardOperations.LocateShipsOnBoard(ComputerField);
    }

    public (int x, int y) GetShotCoordinates()
    {
        if (_shootingAtShip)
        {
            var possiblePoints = GameUtils.GetPossibleShipPointsByPoint(_userField, _lastHit);
            _lastShot = possiblePoints.Count > 0 ?
                possiblePoints[Random.Shared.Next(possiblePoints.Count)]
                : GameUtils.GetPlausibleShotCoordinates(_userField);
        } else
        {
            _lastShot = GameUtils.GetPlausibleShotCoordinates(_userField);
        }
        return _lastShot;
    }

    public void SetShotResult(PointStatus status) // Only Hit, Miss, Sunk
    {
        if (status == PointStatus.Hit)
        {
            _lastHit = _lastShot;
            _shootingAtShip = true;
        }
        else if (status == PointStatus.Sunk)
        {
            Logic.ShipUtils.KillShip(_userField, Logic.ShipUtils.GetShipPoints(
                _userField, _lastShot.x, _lastShot.y, new HashSet<(int x, int y)>()));
            _shootingAtShip = false;
        }
        _userField.Field[_lastShot.x, _lastShot.y] = status;
    }

    public PlayerShotResult ProcessPlayerShot((int x, int y) point)
    {
        var turn = PlayerShotResult.GameMembers.Computer;
        HashSet<(int x, int y)> affectedPoints = [point];
        if (ComputerField.Field[point.x, point.y] == PointStatus.Ship)
        {
            turn = PlayerShotResult.GameMembers.Player;
            var shipPoints = Logic.ShipUtils.GetShipPoints(ComputerField, point.x, point.y,
                new HashSet<(int x, int y)>());
            ComputerField.Field[point.x, point.y] = PointStatus.Hit;
            if (shipPoints.FindAll(p => ComputerField.Field[p.x, p.y] == PointStatus.Ship).Count == 0)
            {
                affectedPoints = Logic.ShipUtils.KillShip(ComputerField, shipPoints);
            }
        }
        else if (ComputerField.Field[point.x, point.y] == PointStatus.Empty)
        {
            ComputerField.Field[point.x, point.y] = PointStatus.Miss;
        }

        PlayerShotResult.GameMembers? winner = null;
        if (_userField.GameIsFinished()) winner = PlayerShotResult.GameMembers.Computer;
        else if (ComputerField.GameIsFinished()) winner = PlayerShotResult.GameMembers.Player;
        return new PlayerShotResult()
        {
            AffectedPoints = affectedPoints,
            Winner = winner,
            GameIsFinished = winner != null,
            Turn = turn
        };
    }
}