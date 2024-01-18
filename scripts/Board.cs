using Godot;
using System;

public partial class Board : GridContainer
{
    [Export]
    public PackedScene BoardTileScene, ChessPieceScene;

    //public const int BoardTileSize = 64;
    char[] _aplhabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public override void _Ready()
    {
        /*
        _boardContainer.Size = new Vector2(
            x: BoardTileSize * _boardContainer.Columns,
            y: BoardTileSize * _boardContainer.Columns
        );
        */

        // board creation
        for(int i=0; i < MathF.Pow(Columns, 2); i++){
            CreateBoardTile(i);
        }

        BasicBoardSetup();

        ChangePiecePosition(GetNode<BoardTile>("E7"), GetNode<BoardTile>("E5"));
    }
    private void CreateBoardTile(int tileId){
        var boardTile = BoardTileScene.Instantiate<BoardTile>();

        AddChild(boardTile);

        boardTile.AddToGroup("BoardTiles");

        boardTile.Name = _aplhabet[tileId%8] + ((tileId/8)+1).ToString();

        if((tileId / 8) % 2 == 1 ){ 
            if(tileId % 2 == 1)
                boardTile.TileColor = BoardTile.TileColors.Dark; 
            else
                boardTile.TileColor = BoardTile.TileColors.Light; 

        }
        else{
            if(tileId % 2 == 1)
                boardTile.TileColor = BoardTile.TileColors.Light; 
            else
                boardTile.TileColor = BoardTile.TileColors.Dark; 

        }
    }
    private void CreateChessPiece(BoardTile boardTile, ChessPiece.PieceTypes pieceType, ChessPiece.PieceColors pieceColor){
        var chessPiece = ChessPieceScene.Instantiate<ChessPiece>();

        boardTile.AddChild(chessPiece);

        chessPiece.PieceType = pieceType;
        chessPiece.PieceColor = pieceColor;
    }
    private void ChangePiecePosition(BoardTile oldBoardTile, BoardTile newBoardTile){
        var chessPiece = GetNode<ChessPiece>(oldBoardTile.Name+"/ChessPiece");

        oldBoardTile.RemoveChild(chessPiece);
        
        CreateChessPiece(newBoardTile, chessPiece.PieceType, chessPiece.PieceColor);
    }
    private void BasicBoardSetup(){
        // black
        CreateChessPiece(GetNode<BoardTile>("A1"), ChessPiece.PieceTypes.Rook, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("H1"), ChessPiece.PieceTypes.Rook, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("B1"), ChessPiece.PieceTypes.Knight, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("G1"), ChessPiece.PieceTypes.Knight, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("C1"), ChessPiece.PieceTypes.Bishop, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("F1"), ChessPiece.PieceTypes.Bishop, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("D1"), ChessPiece.PieceTypes.Queen, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("E1"), ChessPiece.PieceTypes.King, ChessPiece.PieceColors.Dark);
        for(int i=0; i < 8; i++){
            CreateChessPiece(GetNode<BoardTile>(_aplhabet[i]+"2"), ChessPiece.PieceTypes.Pawn, ChessPiece.PieceColors.Dark);
        }
        //white
        CreateChessPiece(GetNode<BoardTile>("A8"), ChessPiece.PieceTypes.Rook, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("H8"), ChessPiece.PieceTypes.Rook, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("B8"), ChessPiece.PieceTypes.Knight, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("G8"), ChessPiece.PieceTypes.Knight, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("C8"), ChessPiece.PieceTypes.Bishop, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("F8"), ChessPiece.PieceTypes.Bishop, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("D8"), ChessPiece.PieceTypes.Queen, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("E8"), ChessPiece.PieceTypes.King, ChessPiece.PieceColors.Light);
        for(int i=0; i < 8; i++){
            CreateChessPiece(GetNode<BoardTile>(_aplhabet[i]+"7"), ChessPiece.PieceTypes.Pawn, ChessPiece.PieceColors.Light);
        }
    }
}
