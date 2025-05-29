using System;
using ReactiveUI;

namespace BattleshipGame.ViewModels;
using Models;


public class Cell: ReactiveObject
{
    private PointStatus _state;
    public Action<Cell>? FireCommand;

    public PointStatus State
    {
        get => _state;
        set
        {
            this.RaiseAndSetIfChanged(ref _state, value);
            this.RaisePropertyChanged(nameof(CellText));
        }

    }
    public int X;
    public int Y;

    public string CellText =>
        _state switch
        {
            PointStatus.Empty => " ",
            PointStatus.Hit => "❌",
            PointStatus.Miss => "•",
            PointStatus.Ship => "*",
            PointStatus.Sunk => "✖",
            _ => "?"
        };

    public Cell(int x, int y)
    {
        X = x;
        Y = y;
        State = PointStatus.Empty;
    }
    

    public void Fire()
    {
        this.FireCommand?.Invoke(this);
    }
    

}