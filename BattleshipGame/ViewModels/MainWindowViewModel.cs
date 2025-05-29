using System;
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

    public string TurnMessage => IsPlayerTurn ? "Ход игрока" : $"Ход компьютера: {_computerLastMove}";
    private readonly GameStrategy _gameStrategy;
    public bool IsMiss { get; set; } = true;
    public bool IsSunk { get; set; }
    public bool IsHit { get; set; }

    private void DoComputerShot()
    {
        (int x, int y) = _gameStrategy.GetShotCoordinates();
        _computerLastMove = (char)(1040 + x) + y.ToString();
    }

    public MainWindowViewModel()
    {
        var cells = new ObservableCollection<Cell>();
        for(int i=0;i<10;i++)
        for(int j=0;j<10;j++)
        {
            cells.Add(new Cell(x: i, y: j) { FireCommand = FireCommand});
        }
        
        Cells = cells;
        _gameStrategy = new GameStrategy();
        DoComputerShot();
    }

    private void FireCommand(Cell cell)
    {
        _gameStrategy.ShotAndSunkIfNeed((x: cell.X, y: cell.Y));
        if (!IsPlayerTurn || cell.State != PointStatus.Empty) return;
        foreach (var point in Cells)
        {
            if (_gameStrategy.Field.Field[point.X, point.Y] == PointStatus.Sunk ||
                _gameStrategy.Field.Field[point.X, point.Y] == PointStatus.Miss)
            {
                point.State = _gameStrategy.Field.Field[point.X, point.Y];
            }
        }
        
        if (_gameStrategy.Field.Field[cell.X, cell.Y] == PointStatus.Empty)
        {
            _gameStrategy.Field.Field[cell.X, cell.Y] = PointStatus.Miss;
        }

        cell.State = _gameStrategy.Field.Field[cell.X, cell.Y];
        if (cell.State != PointStatus.Sunk) IsPlayerTurn = false;
    }

    public void SendUserAction()
    {
        IsPlayerTurn = true;
        PointStatus status;
        if (IsMiss) status = PointStatus.Miss;
        else status = IsSunk ? PointStatus.Sunk : PointStatus.Hit;
        _gameStrategy.SetShotResult(status);
        DoComputerShot();
    }
   
}