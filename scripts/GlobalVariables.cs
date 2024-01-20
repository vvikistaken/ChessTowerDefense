using Godot;
using System;

public partial class GlobalVariables : Node
{
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
    public BoardTile LastBoardTileClicked;
    public ChessPiece LastChessPieceClicked;
    public ChessPieceColors CurrentRound = ChessPieceColors.Light;
}
