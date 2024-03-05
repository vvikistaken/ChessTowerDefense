using Godot;
using System;

public partial class GlobalVariables : Node
{
    public int PlayerCash = 0, EnemyCash = 0;
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
}
