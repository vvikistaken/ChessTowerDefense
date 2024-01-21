using Godot;
using System;
using PieceTypes = GlobalVariables.ChessPieceTypes;
using PieceColors = GlobalVariables.ChessPieceColors;

public partial class Board : GridContainer
{
    [Export]
    public PackedScene BoardTileScene, ChessPieceScene;
    private GlobalVariables gVar;

    //public const int BoardTileSize = 64;
    char[] _aplhabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

    public override void _Ready()
    {
        gVar = GetNode<GlobalVariables>("/root/GlobalVariables");
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

        StandardBoardSetup();

        //ChangePiecePosition(GetNode<BoardTile>("E7"), GetNode<BoardTile>("E5"));
    }
    public override void _Process(double delta)
    {
        if(gVar.LastChessPieceClicked != null && gVar.LastBoardTileClicked != null)
            MovePiece(gVar.LastBoardTileClicked, gVar.LastChessPieceClicked);
    }
    // for the board tiles creation
    private void CreateBoardTile(int tileId){
        var boardTile = BoardTileScene.Instantiate<BoardTile>();

        AddChild(boardTile);

        boardTile.BoardTileClicked += (BoardTile boardTile) => gVar.LastBoardTileClicked = boardTile;
        boardTile.BoardTileClicked += (nothing) => GD.Print(gVar.LastBoardTileClicked.Name+" clicked");


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
    // creates a specified chess piece to a board tile
    private void CreateChessPiece(BoardTile boardTile, PieceTypes pieceType, PieceColors pieceColor){
        var chessPiece = ChessPieceScene.Instantiate<ChessPiece>();

        boardTile.AddChild(chessPiece);

        chessPiece.ChessPieceClicked += (ChessPiece chessPiece) => gVar.LastChessPieceClicked = chessPiece;
        chessPiece.ChessPieceClicked += (ChessPiece chessPiece) => gVar.LastBoardTileClicked = null;

        chessPiece.PieceType = pieceType;
        chessPiece.PieceColor = pieceColor;
    }
    // pretty self explanatory, i think
    private void ChangePiecePosition(BoardTile oldBoardTile, BoardTile newBoardTile){
        var chessPiece = GetNode<ChessPiece>(oldBoardTile.Name+"/ChessPiece");

        oldBoardTile.RemoveChild(chessPiece);
        
        CreateChessPiece(newBoardTile, chessPiece.PieceType, chessPiece.PieceColor);
    }
    // only gets called when theres is successful movement
    private void MovePiece(BoardTile boardTile, ChessPiece movedPiece){
        // can be null
        var pieceOnTile = GetBoardTilePiece(boardTile);
        var lastPiecePos = movedPiece.GetParent<BoardTile>();
        
        if(pieceOnTile is not null && pieceOnTile.PieceColor != movedPiece.PieceColor){
            boardTile.RemoveChild(pieceOnTile);
        }
        ChangePiecePosition(lastPiecePos, boardTile);
        

        if(gVar.CurrentRound == PieceColors.Light)
            gVar.CurrentRound = PieceColors.Dark;
        else
            gVar.CurrentRound = PieceColors.Light;
        gVar.LastChessPieceClicked=null;
        gVar.LastBoardTileClicked=null;
        
    }
    private ChessPiece GetBoardTilePiece(BoardTile boardTile){
        return boardTile.GetNodeOrNull<ChessPiece>("ChessPiece");
    }
    // sets up pieces in a way a normal game would
    private void StandardBoardSetup(){
        // black
        CreateChessPiece(GetNode<BoardTile>("A1"), PieceTypes.Rook, PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("H1"), PieceTypes.Rook, PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("B1"), PieceTypes.Knight, PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("G1"), PieceTypes.Knight, PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("C1"), PieceTypes.Bishop, PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("F1"), PieceTypes.Bishop, PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("D1"), PieceTypes.Queen, PieceColors.Dark);
        CreateChessPiece(GetNode<BoardTile>("E1"), PieceTypes.King, PieceColors.Dark);
        for(int i=0; i < 8; i++){
            CreateChessPiece(GetNode<BoardTile>(_aplhabet[i]+"2"), PieceTypes.Pawn, PieceColors.Dark);
        }
        //white
        CreateChessPiece(GetNode<BoardTile>("A8"), PieceTypes.Rook, PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("H8"), PieceTypes.Rook, PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("B8"), PieceTypes.Knight, PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("G8"), PieceTypes.Knight, PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("C8"), PieceTypes.Bishop, PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("F8"), PieceTypes.Bishop, PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("D8"), PieceTypes.Queen, PieceColors.Light);
        CreateChessPiece(GetNode<BoardTile>("E8"), PieceTypes.King, PieceColors.Light);
        for(int i=0; i < 8; i++){
            CreateChessPiece(GetNode<BoardTile>(_aplhabet[i]+"7"), PieceTypes.Pawn, PieceColors.Light);
        }
    }
}
