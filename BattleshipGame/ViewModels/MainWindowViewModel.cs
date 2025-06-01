using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ReactiveUI;
using BattleshipGame.Models;

namespace BattleshipGame.ViewModels;

public class MainWindowViewModel: ViewModelBase
{
    public ObservableCollection<Cell> Cells { get; set;  }
    public bool IsUserSelectorsVisible { get; private set; }
    private bool _isPlayerTurn = true;
    private bool IsPlayerTurn
    {
        get => _isPlayerTurn;
        set
        {
            if (_isPlayerTurn != value)
            {
                _isPlayerTurn = value;
                this.RaisePropertyChanged(nameof(TurnMessage));
                IsUserSelectorsVisible = !IsUserSelectorsVisible;
                this.RaisePropertyChanged(nameof(IsUserSelectorsVisible));
            }
        }
    }
    

    private string _computerLastMove = String.Empty;
    private PlayerShotResult.GameMembers? _winner;
    private readonly Dictionary<(int x, int y), Cell> _cellsDictionary;
    
    public string TurnMessage
    {
        get
        {
            if (_winner == null)
            {
                return IsPlayerTurn ? "Ход игрока" : $"Ход компьютера: {_computerLastMove}";
            }
            return _winner == PlayerShotResult.GameMembers.Player ? "Вы выиграли": "Выиграл компьютер";
        }
    }

    private readonly GameStrategy _gameStrategy;
    public bool IsMiss { get; set; } = true;
    public bool IsSunk { get; set; }
    public bool IsHit { get; set; }

    private void DoComputerShot()
    {
        (int x, int y) = _gameStrategy.GetShotCoordinates();
        _computerLastMove = (char)(1040 + (y == 9 ? 10 : y)) + (x+1).ToString();
        this.RaisePropertyChanged(nameof(TurnMessage));
    }

    public MainWindowViewModel()
    {
        var cells = new ObservableCollection<Cell>();
        _cellsDictionary = new Dictionary<(int x, int y), Cell>();
        for(int i=0;i<10;i++)
        for(int j=0;j<10;j++)
        {
            var cell = new Cell(x: i, y: j) { FireCommand = FireCommand };
            cells.Add(cell);
            _cellsDictionary[(i, j)] = cell;
        }
        Cells = cells;
        _gameStrategy = new GameStrategy();
        DoComputerShot();
    }


    private void FireCommand(Cell cell)
    {
        if (!IsPlayerTurn || cell.State != PointStatus.Empty) return;
        var shotResult = _gameStrategy.ProcessPlayerShot((x: cell.X, y: cell.Y));
        foreach (var point in shotResult.AffectedPoints)
        {
            _cellsDictionary[(point.x, point.y)].State = _gameStrategy.ComputerField.Field[point.x, point.y];
        }

        IsPlayerTurn = shotResult.Turn == PlayerShotResult.GameMembers.Player;
        if (shotResult.GameIsFinished)
        {
            _isPlayerTurn = false;
            _winner = shotResult.Winner;
        }
        else if (!IsPlayerTurn) DoComputerShot();
    }

    public void SendUserAction()
    {
        PointStatus status;
        if (IsMiss) status = PointStatus.Miss;
        else status = IsSunk ? PointStatus.Sunk : PointStatus.Hit;
        IsPlayerTurn = status == PointStatus.Miss;
        _gameStrategy.SetShotResult(status);
        DoComputerShot();
    }
   
}