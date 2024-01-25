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
        if(gVar.LastPieceClicked != null && gVar.LastTileClicked != null)
            if(CheckMove())
                MovePiece(gVar.LastTileClicked, gVar.LastPieceClicked);
            else
                gVar.LastTileClicked = null;
    }
    // for the board tiles creation
    private void CreateBoardTile(int tileId){
        var boardTile = BoardTileScene.Instantiate<BoardTile>();

        AddChild(boardTile);

        boardTile.BoardTileClicked += (BoardTile boardTile) => gVar.LastTileClicked = boardTile;

        boardTile.X = tileId%8;
        boardTile.Y = tileId/8;
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
    private void CreateChessPiece(BoardTile boardTile, PieceTypes pieceType, PieceColors pieceColor, bool isNew){
        var chessPiece = ChessPieceScene.Instantiate<ChessPiece>();

        boardTile.AddChild(chessPiece);

        chessPiece.FirstMove = isNew;

        chessPiece.ChessPieceClicked += OnChessPieceClicked;

        chessPiece.PieceType = pieceType;
        chessPiece.PieceColor = pieceColor;
    }
    // pretty self explanatory, i think
    private void ChangePiecePosition(BoardTile oldBoardTile, BoardTile newBoardTile){
        var chessPiece = GetNode<ChessPiece>(oldBoardTile.Name+"/ChessPiece");

        oldBoardTile.RemoveChild(chessPiece);
        
        CreateChessPiece(newBoardTile, chessPiece.PieceType, chessPiece.PieceColor, false);
    }
    // only gets called when theres is successful movement
    private void MovePiece(BoardTile boardTile, ChessPiece movedPiece){
        // can be null
        var pieceOnTile = CheckForPiece(boardTile);
        var lastPiecePos = movedPiece.GetParent<BoardTile>();
        
        if(pieceOnTile is not null && pieceOnTile.PieceColor != movedPiece.PieceColor){
            boardTile.RemoveChild(pieceOnTile);
        }
        ChangePiecePosition(lastPiecePos, boardTile);

        if(gVar.CurrentRound == PieceColors.Light)
            gVar.CurrentRound = PieceColors.Dark;
        else
            gVar.CurrentRound = PieceColors.Light;
        gVar.LastPieceClicked=null;
        gVar.LastTileClicked=null;
        
    }
    private ChessPiece CheckForPiece(BoardTile boardTile){
        return boardTile.GetNodeOrNull<ChessPiece>("ChessPiece");
    }
    // this is where the piece movement limitation happends
    private bool CheckMove(){
        var piece = gVar.LastPieceClicked;
        var curPiecePos = piece.GetParent<BoardTile>();
        var toMoveTile = gVar.LastTileClicked;
        
        GD.Print("x move: "+(curPiecePos.X - toMoveTile.X));
        GD.Print("y move: "+(curPiecePos.Y - toMoveTile.Y));
        switch(piece.PieceType){
            
            case PieceTypes.Pawn:
                if(curPiecePos.X == toMoveTile.X){
                    if(piece.FirstMove){
                        if((curPiecePos.Y - toMoveTile.Y) <= 2 &&
                            (curPiecePos.Y - toMoveTile.Y) > 0 &&
                            piece.PieceColor == PieceColors.Light
                        )
                            return true;
                        if((curPiecePos.Y - toMoveTile.Y) >= -2 &&
                            (curPiecePos.Y - toMoveTile.Y) < 0 &&
                            piece.PieceColor == PieceColors.Dark)
                            return true;
                    }
                    else if(CheckForPiece(toMoveTile) != null &&
                            ((curPiecePos.X - toMoveTile.X) == -1 ||
                            (curPiecePos.X - toMoveTile.X) == 1)
                    )
                        return true;
                    else {
                        if((curPiecePos.Y - toMoveTile.Y) <= 1 &&
                            (curPiecePos.Y - toMoveTile.Y) > 0 &&
                            piece.PieceColor == PieceColors.Light
                        )
                            return true;
                        if((curPiecePos.Y - toMoveTile.Y) >= -1 &&
                            (curPiecePos.Y - toMoveTile.Y) < 0 &&
                             piece.PieceColor == PieceColors.Dark)
                            return true;
                    }
                }
            return false;
            /*
            case PieceTypes.Bishop:

            break;
            case PieceTypes.Knight:

            break;
            case PieceTypes.Rook:

            break;
            case PieceTypes.Queen:

            break;
            case PieceTypes.King:

            break;
            */
            default:
            return true;
        }
    }
    private void OnChessPieceClicked(ChessPiece chessPiece){
        if(gVar.LastPieceClicked is null)
            gVar.LastPieceClicked = chessPiece;
        else
            gVar.LastPieceClicked = null;

        gVar.LastTileClicked = null;
    }
    // sets up pieces in a way a normal game would
    private void StandardBoardSetup(){
        // black
        CreateChessPiece(GetNode<BoardTile>("A1"), PieceTypes.Rook, PieceColors.Dark, true);
        CreateChessPiece(GetNode<BoardTile>("H1"), PieceTypes.Rook, PieceColors.Dark, true);
        CreateChessPiece(GetNode<BoardTile>("B1"), PieceTypes.Knight, PieceColors.Dark, true);
        CreateChessPiece(GetNode<BoardTile>("G1"), PieceTypes.Knight, PieceColors.Dark, true);
        CreateChessPiece(GetNode<BoardTile>("C1"), PieceTypes.Bishop, PieceColors.Dark, true);
        CreateChessPiece(GetNode<BoardTile>("F1"), PieceTypes.Bishop, PieceColors.Dark, true);
        CreateChessPiece(GetNode<BoardTile>("D1"), PieceTypes.Queen, PieceColors.Dark, true);
        CreateChessPiece(GetNode<BoardTile>("E1"), PieceTypes.King, PieceColors.Dark, true);
        for(int i=0; i < 8; i++){
            CreateChessPiece(GetNode<BoardTile>(_aplhabet[i]+"2"), PieceTypes.Pawn, PieceColors.Dark, true);
        }
        //white
        CreateChessPiece(GetNode<BoardTile>("A8"), PieceTypes.Rook, PieceColors.Light, true);
        CreateChessPiece(GetNode<BoardTile>("H8"), PieceTypes.Rook, PieceColors.Light, true);
        CreateChessPiece(GetNode<BoardTile>("B8"), PieceTypes.Knight, PieceColors.Light, true);
        CreateChessPiece(GetNode<BoardTile>("G8"), PieceTypes.Knight, PieceColors.Light, true);
        CreateChessPiece(GetNode<BoardTile>("C8"), PieceTypes.Bishop, PieceColors.Light, true);
        CreateChessPiece(GetNode<BoardTile>("F8"), PieceTypes.Bishop, PieceColors.Light, true);
        CreateChessPiece(GetNode<BoardTile>("D8"), PieceTypes.Queen, PieceColors.Light, true);
        CreateChessPiece(GetNode<BoardTile>("E8"), PieceTypes.King, PieceColors.Light, true);
        for(int i=0; i < 8; i++){
            CreateChessPiece(GetNode<BoardTile>(_aplhabet[i]+"7"), PieceTypes.Pawn, PieceColors.Light, true);
        }
    }
}
