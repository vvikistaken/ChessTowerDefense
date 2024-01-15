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
    }
    private void CreateBoardTile(int tileId){
        var boardTile = BoardTileScene.Instantiate<BoardTile>();

        AddChild(boardTile);

        boardTile.AddToGroup("BoardTiles");

        boardTile.Name = _aplhabet[tileId/8] +((tileId%8)+1).ToString();

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
    private void BasicBoardSetup(){
        // black
        CreateChessPiece(GetNode<BoardTile>("A1"), ChessPiece.PieceTypes.Rook, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("A8"), ChessPiece.PieceTypes.Rook, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("A2"), ChessPiece.PieceTypes.Knight, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("A7"), ChessPiece.PieceTypes.Knight, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("A3"), ChessPiece.PieceTypes.Bishop, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("A6"), ChessPiece.PieceTypes.Bishop, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("A4"), ChessPiece.PieceTypes.Queen, ChessPiece.PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("A5"), ChessPiece.PieceTypes.King, ChessPiece.PieceColors.Dark);
        for(int i=1; i <= 8; i++){
            CreateChessPiece(GetNode<BoardTile>("B"+i), ChessPiece.PieceTypes.Pawn, ChessPiece.PieceColors.Dark);
        }
        //white
        CreateChessPiece(GetNode<BoardTile>("H1"), ChessPiece.PieceTypes.Rook, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("H8"), ChessPiece.PieceTypes.Rook, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("H2"), ChessPiece.PieceTypes.Knight, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("H7"), ChessPiece.PieceTypes.Knight, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("H3"), ChessPiece.PieceTypes.Bishop, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("H6"), ChessPiece.PieceTypes.Bishop, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("H4"), ChessPiece.PieceTypes.Queen, ChessPiece.PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("H5"), ChessPiece.PieceTypes.King, ChessPiece.PieceColors.Light);
        for(int i=1; i <= 8; i++){
            CreateChessPiece(GetNode<BoardTile>("G"+i), ChessPiece.PieceTypes.Pawn, ChessPiece.PieceColors.Light);
        }
    }
}
