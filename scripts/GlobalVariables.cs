using Godot;
using System;

public partial class GlobalVariables : Node
{
    [Signal]
    public delegate void InspectChessPieceUIEventHandler(ChessPiece chessPiece, bool OpenState);
    public int PlayerCash = 500, EnemyCash = 0;
    public enum ChessPieceTypes{
        Pawn,
		Bishop,
		Knight,
		Rook,
		Queen,
		King
    }
    public enum ChessPieceColors{
        Light,
        Dark
    }
    public BoardTile LastTileClicked;
    public ChessPiece LastPieceClicked;
    public ChessPieceColors CurrentRound = ChessPieceColors.Light;
    public override void _Input(InputEvent @event)
    {
        if(Input.IsKeyPressed(Key.Up))
            PlayerCash += 100;
        if(Input.IsKeyPressed(Key.Down))
            PlayerCash -= 100;
    }
}