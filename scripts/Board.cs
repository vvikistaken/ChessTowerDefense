using Godot;
using System;
using PieceTypes = GlobalVariables.ChessPieceTypes;
using PieceColors = GlobalVariables.ChessPieceColors;
using System.Collections;

public partial class Board : GridContainer
{
    [Signal]
    public delegate void GameEndEventHandler(GlobalVariables.ChessPieceColors whichSide);
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

       // CreateChessPiece(GetNode<BoardTile>("D5"), PieceTypes.Bishop, PieceColors.Light, true);

        //ChangePiecePosition(GetNode<BoardTile>("E7"), GetNode<BoardTile>("E5"));
    }
    public override void _Process(double delta)
    {
        if(gVar.LastPieceClicked != null && gVar.LastTileClicked != null)
            if(CheckMove()){
                MovePiece(gVar.LastTileClicked, gVar.LastPieceClicked);
                ClearMarkers();
            }
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
        var pieceOnTile = CheckTileForPiece(boardTile);
        var lastPiecePos = movedPiece.GetParent<BoardTile>();
        
        if(pieceOnTile is not null && pieceOnTile.PieceColor != movedPiece.PieceColor){
            if(pieceOnTile.PieceType == PieceTypes.King){
                pieceOnTile.Visible = false;
                EmitSignal(SignalName.GameEnd, (int)movedPiece.PieceColor);
            }
            else
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
    // returns piece on tile or null
    private ChessPiece CheckTileForPiece(BoardTile boardTile){
        
        return boardTile.GetNodeOrNull<ChessPiece>("ChessPiece");
    }
    // returns piece on tile or null, also checks if board tile exists
    private ChessPiece CheckTileForPiece(int x, int y){
        // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
        #pragma warning disable CS8632 
        BoardTile? boardTile = null;
        #pragma warning restore CS8632 

        for (int i=0; i < MathF.Pow(Columns, 2); i++){
            var toCheck = GetBoardTile(i%8, i/8);
            if(toCheck.X == x && toCheck.Y == y)
                boardTile = toCheck;
        }

        if(boardTile != null)
            // returns piece on tile or null
            return boardTile.GetNodeOrNull<ChessPiece>("ChessPiece");
        else
            return null;
    }
    // returns board tile based on theirs coords
    private BoardTile GetBoardTile(int x, int y){
        if(x < 0 || x >= 26)
            return null;
        BoardTile boardTile = GetNodeOrNull<BoardTile>(_aplhabet[x] + (y+1).ToString());;

        return boardTile;
    }

    // this is where the piece movement limitation happends
    private bool CheckMove(){
        var piece = gVar.LastPieceClicked;
        var curPiecePos = piece.GetParent<BoardTile>();
        var toMoveTile = gVar.LastTileClicked;
        
        int xDistance = (int)toMoveTile.Get("X") - (int)curPiecePos.Get("X"),
            yDistance = (int)toMoveTile.Get("Y") - (int)curPiecePos.Get("Y");

        GD.Print("x move: "+xDistance+", y move: "+yDistance);
        //GD.Print(CheckTileForPiece(toMoveTile));

        // movement checks
        switch(piece.PieceType){
            case PieceTypes.Pawn:
                // default pawn moves
                if(CheckTileForPiece(toMoveTile) == null && curPiecePos.X == toMoveTile.X){
                    if(piece.FirstMove){
                        if(yDistance >= -2 && yDistance < 0 &&
                            piece.PieceColor == PieceColors.Light)
                         return true;
                        if(yDistance <= 2 && yDistance > 0 &&
                            piece.PieceColor == PieceColors.Dark)
                            return true;
                        }
                        else {
                            if(yDistance >= -1 && yDistance < 0 &&
                                piece.PieceColor == PieceColors.Light)
                                return true;
                            if(yDistance <= 1 && yDistance > 0 &&
                                piece.PieceColor == PieceColors.Dark)
                                return true;
                        }
                }
                // pawn attacking
                else if(CheckTileForPiece(toMoveTile) != null) {
                    if( xDistance == 1 || xDistance == -1 ){
                        if(yDistance >= -1 && yDistance < 0 &&
                            piece.PieceColor == PieceColors.Light)
                            return true;
                        if(yDistance <= 1 && yDistance > 0 &&
                            piece.PieceColor == PieceColors.Dark)
                            return true;
                    }
                } 
                // adding en passant will be hell  
            return false;
            case PieceTypes.Bishop:
                if(Math.Abs(xDistance) == Math.Abs(yDistance)){
                    // can use x or y, which doesn't even matter
                    for(int i=1; i < Math.Abs(xDistance); i++){
                        if(CheckTileForPiece(
                            curPiecePos.X + (xDistance > 0 ? i : -i),
                            curPiecePos.Y + (yDistance > 0 ? i : -i) ) != null
                        )
                            return false;
                    }
                    return true;
                }
            return false;
            case PieceTypes.Knight:
                if( Math.Abs(xDistance) == 1 && Math.Abs(yDistance) == 2 ||
                    Math.Abs(xDistance) == 2 && Math.Abs(yDistance) == 1 )
                    return true;
            return false;
            case PieceTypes.Rook:
                if(Math.Abs(xDistance) > 0 && Math.Abs(yDistance) == 0){
                    for(int i=1; i < Math.Abs(xDistance); i++){
                        if(CheckTileForPiece(
                            curPiecePos.X + (xDistance > 0 ? i : -i),
                            curPiecePos.Y) != null
                        )
                            return false;
                    }
                    return true;
                }
                if(Math.Abs(xDistance) == 0 && Math.Abs(yDistance) > 0){
                    for(int i=1; i < Math.Abs(yDistance); i++){
                        if(CheckTileForPiece(
                            curPiecePos.X,
                            curPiecePos.Y + (yDistance > 0 ? i : -i)) != null
                        )
                            return false;
                    }
                    return true;
                }
                // potential castling in future
                //if(piece.FirstMove && xDistance  )
            return false;
            case PieceTypes.Queen:
                if(Math.Abs(xDistance) > 0 && Math.Abs(yDistance) == 0){
                    for(int i=1; i < Math.Abs(xDistance); i++){
                        if(CheckTileForPiece(
                            curPiecePos.X + (xDistance > 0 ? i : -i),
                            curPiecePos.Y) != null
                        )
                            return false;
                    }
                    return true;
                }
                if(Math.Abs(xDistance) == 0 && Math.Abs(yDistance) > 0){
                    for(int i=1; i < Math.Abs(yDistance); i++){
                        if(CheckTileForPiece(
                            curPiecePos.X,
                            curPiecePos.Y + (yDistance > 0 ? i : -i)) != null
                        )
                            return false;
                    }
                    return true;
                }
                if(Math.Abs(xDistance) == Math.Abs(yDistance)){
                    // can use x or y, which doesn't matter here
                    for(int i=1; i < Math.Abs(xDistance); i++){
                        if(CheckTileForPiece(
                            curPiecePos.X + (xDistance > 0 ? i : -i),
                            curPiecePos.Y + (yDistance > 0 ? i : -i) ) != null
                        )
                            return false;
                    }
                    return true;
                }
            return false;
            case PieceTypes.King:
                if( Math.Abs(xDistance) <= 1 && Math.Abs(xDistance) >= 0 &&
                    Math.Abs(yDistance) <= 1 && Math.Abs(yDistance) >= 0 )
                    return true;
            return false;
            default:
            return true;
        }
    }
    // mark the tiles that a specified pieces can move
    // surely there is a better way to do it 
    // there is like 400 lines of code that could be shortenend/simplified
    private void MarkBoardTiles(ChessPiece piece){
        int xPosition = (int)piece.GetParent<BoardTile>().Get("X"),
            yPosition = (int)piece.GetParent<BoardTile>().Get("Y");
        bool top, bottom, left, right;
        bool topLeft, topRight, bottomLeft, bottomRight;

        GD.Print($"xPos: {xPosition} | yPos: {yPosition}");

        switch(piece.PieceType){
            case PieceTypes.Pawn:
                // you possibly could make it in one
                // FUCK YOU IT WILL BE SO FUCKING UNREADABLE YOU DUMB FUCK ITS SO STUPID
                // but maybe . . .
                switch(piece.PieceColor){
                    case PieceColors.Light:
                        // attack
                        if(CheckTileForPiece(xPosition-1, yPosition-1) != null)
                            if(CheckTileForPiece(xPosition-1, yPosition-1).PieceColor != piece.PieceColor)
                                MarkBoardTile(GetBoardTile(xPosition-1, yPosition-1), true);
                        if(CheckTileForPiece(xPosition+1, yPosition-1) != null)
                            if(CheckTileForPiece(xPosition+1, yPosition-1).PieceColor != piece.PieceColor)
                                MarkBoardTile(GetBoardTile(xPosition+1, yPosition-1), true);
                        // move forward
                        if(CheckTileForPiece(xPosition, yPosition-1) == null)
                            MarkBoardTile(GetBoardTile(xPosition, yPosition-1), true);
                        if(CheckTileForPiece(xPosition, yPosition-2) == null &
                            piece.FirstMove)
                            MarkBoardTile(GetBoardTile(xPosition, yPosition-2), true);
                    break;
                    case PieceColors.Dark:
                        // attack
                        if(CheckTileForPiece(xPosition-1, yPosition+1) != null)
                            if(CheckTileForPiece(xPosition-1, yPosition+1).PieceColor != piece.PieceColor)
                                MarkBoardTile(GetBoardTile(xPosition-1, yPosition+1), true);
                        if(CheckTileForPiece(xPosition+1, yPosition+1) != null)
                            if(CheckTileForPiece(xPosition+1, yPosition+1).PieceColor != piece.PieceColor)
                                MarkBoardTile(GetBoardTile(xPosition+1, yPosition+1), true);
                        // move forward
                        if(CheckTileForPiece(xPosition, yPosition+1) == null)
                            MarkBoardTile(GetBoardTile(xPosition, yPosition+1), true);
                        if(CheckTileForPiece(xPosition, yPosition+2) == null &
                            piece.FirstMove)
                            MarkBoardTile(GetBoardTile(xPosition, yPosition+2), true);
                    break;
                }
            break;
            case PieceTypes.Bishop:
                topLeft = topRight = bottomLeft = bottomRight = true;

                for(int i = 1; i < Columns; i++){
                    //GD.Print($"TL: {topLeft} | TR: {topRight} | BL: {bottomLeft} | BR: {bottomRight}");
                    if(xPosition-i >= 0 & yPosition-i >= 0 &
                        topLeft){
                            if(CheckTileForPiece(xPosition - i, yPosition - i) == null)
                                MarkBoardTile(GetBoardTile(xPosition - i, yPosition - i), true);
                            if(CheckTileForPiece(xPosition - i, yPosition - i) != null){
                                if(CheckTileForPiece(xPosition - i, yPosition - i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition - i, yPosition - i), true);
                                topLeft = false;
                            }
                        }
                    else
                        topLeft = false;
                    if(xPosition+i < Columns & yPosition-i >= 0 &
                        topRight){
                            if(CheckTileForPiece(xPosition + i, yPosition - i) == null)
                                MarkBoardTile(GetBoardTile(xPosition + i, yPosition - i), true);
                            if(CheckTileForPiece(xPosition + i, yPosition - i) != null){
                                if(CheckTileForPiece(xPosition + i, yPosition - i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition + i, yPosition - i), true);
                                topRight = false;
                            }
                        }
                    else
                        topRight = false;
                    if(xPosition-i >= 0 & yPosition+i < Columns &
                        bottomLeft){
                            if(CheckTileForPiece(xPosition - i, yPosition + i) == null)
                                MarkBoardTile(GetBoardTile(xPosition - i, yPosition + i), true);
                            if(CheckTileForPiece(xPosition - i, yPosition + i) != null){
                                if(CheckTileForPiece(xPosition - i, yPosition + i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition - i, yPosition + i), true);
                                bottomLeft = false;
                            }
                        }
                    else 
                        bottomLeft = false;
                    if(xPosition+i < Columns & yPosition+i < Columns &
                        bottomRight){
                            if(CheckTileForPiece(xPosition + i, yPosition + i) == null)
                                MarkBoardTile(GetBoardTile(xPosition + i, yPosition + i), true);
                            if(CheckTileForPiece(xPosition + i, yPosition + i) != null){
                                if(CheckTileForPiece(xPosition + i, yPosition + i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition + i, yPosition + i), true);
                                bottomRight = false;
                            }
                        }
                    else
                        bottomRight = false;
                }
            break;
            case PieceTypes.Knight:
                // 2 left, 1 up
                if(CheckTileForPiece(xPosition-2 , yPosition-1) == null)
                    MarkBoardTile(GetBoardTile(xPosition-2, yPosition-1), true);
                if(CheckTileForPiece(xPosition-2, yPosition-1) != null)
                    if(CheckTileForPiece(xPosition-2, yPosition-1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition-2, yPosition-1), true);
                // 2 right, 1 up
                if(CheckTileForPiece(xPosition+2 , yPosition-1) == null)
                    MarkBoardTile(GetBoardTile(xPosition+2, yPosition-1), true);
                if(CheckTileForPiece(xPosition+2, yPosition-1) != null)
                    if(CheckTileForPiece(xPosition+2, yPosition-1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition+2, yPosition-1), true);
                // 2 left, 1 down
                if(CheckTileForPiece(xPosition-2 , yPosition+1) == null)
                    MarkBoardTile(GetBoardTile(xPosition-2, yPosition+1), true);
                if(CheckTileForPiece(xPosition-2, yPosition+1) != null)
                    if(CheckTileForPiece(xPosition-2, yPosition+1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition-2, yPosition+1), true);
                // 2 right, 1 down
                if(CheckTileForPiece(xPosition+2 , yPosition+1) == null)
                    MarkBoardTile(GetBoardTile(xPosition+2, yPosition+1), true);
                if(CheckTileForPiece(xPosition+2, yPosition+1) != null)
                    if(CheckTileForPiece(xPosition+2, yPosition+1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition+2, yPosition+1), true);
                // 1 left, 2 up
                if(CheckTileForPiece(xPosition-1 , yPosition-2) == null)
                    MarkBoardTile(GetBoardTile(xPosition-1 , yPosition-2), true);
                if(CheckTileForPiece(xPosition-1 , yPosition-2) != null)
                    if(CheckTileForPiece(xPosition-1 , yPosition-2).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition-1 , yPosition-2), true);
                // 1 right, 2 up
                if(CheckTileForPiece(xPosition+1 , yPosition-2) == null)
                    MarkBoardTile(GetBoardTile(xPosition+1 , yPosition-2), true);
                if(CheckTileForPiece(xPosition+1 , yPosition-2) != null)
                    if(CheckTileForPiece(xPosition+1 , yPosition-2).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition+1 , yPosition-2), true);
                // 1 left, 2 down
                if(CheckTileForPiece(xPosition-1 , yPosition+2) == null)
                    MarkBoardTile(GetBoardTile(xPosition-1 , yPosition+2), true);
                if(CheckTileForPiece(xPosition-1 , yPosition+2) != null)
                    if(CheckTileForPiece(xPosition-1 , yPosition+2).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition-1 , yPosition+2), true);
                // 1 right, 2 down
                if(CheckTileForPiece(xPosition+1 , yPosition+2) == null)
                    MarkBoardTile(GetBoardTile(xPosition+1 , yPosition+2), true);
                if(CheckTileForPiece(xPosition+1 , yPosition+2) != null)
                    if(CheckTileForPiece(xPosition+1 , yPosition+2).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition+1 , yPosition+2), true);
            break;
            case PieceTypes.Rook:
                top = bottom = left = right = true;

                for(int i = 1; i < Columns; i++){
                    if(xPosition-i >= 0 & top){
                            if(CheckTileForPiece(xPosition - i, yPosition) == null)
                                MarkBoardTile(GetBoardTile(xPosition - i, yPosition), true);
                            if(CheckTileForPiece(xPosition - i, yPosition) != null){
                                if(CheckTileForPiece(xPosition - i, yPosition).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition - i, yPosition), true);
                                top = false;
                            }
                        }
                    else
                        top = false;
                    if(xPosition+i < Columns & bottom){
                            if(CheckTileForPiece(xPosition + i, yPosition) == null)
                                MarkBoardTile(GetBoardTile(xPosition + i, yPosition), true);
                            if(CheckTileForPiece(xPosition + i, yPosition) != null){
                                if(CheckTileForPiece(xPosition + i, yPosition).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition + i, yPosition), true);
                                bottom = false;
                            }
                        }
                    else
                        bottom = false;
                    if(yPosition-i >= 0 & left){
                            if(CheckTileForPiece(xPosition, yPosition - i) == null)
                                MarkBoardTile(GetBoardTile(xPosition, yPosition - i), true);
                            if(CheckTileForPiece(xPosition, yPosition - i) != null){
                                if(CheckTileForPiece(xPosition, yPosition - i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition, yPosition - i), true);
                                left = false;
                            }
                        }
                    else
                        left = false;
                    if(yPosition+i < Columns & right){
                            if(CheckTileForPiece(xPosition, yPosition + i) == null)
                                MarkBoardTile(GetBoardTile(xPosition, yPosition + i), true);
                            if(CheckTileForPiece(xPosition, yPosition + i) != null){
                                if(CheckTileForPiece(xPosition, yPosition + i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition, yPosition + i), true);
                                right = false;
                            }
                        }
                    else
                        right = false;
                }
            break;
            case PieceTypes.Queen:
                top = bottom = left = right = true;
                topLeft = topRight = bottomLeft = bottomRight = true;

                for(int i = 1; i < Columns; i++){
                    if(xPosition-i >= 0 & top){
                            if(CheckTileForPiece(xPosition - i, yPosition) == null)
                                MarkBoardTile(GetBoardTile(xPosition - i, yPosition), true);
                            if(CheckTileForPiece(xPosition - i, yPosition) != null){
                                if(CheckTileForPiece(xPosition - i, yPosition).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition - i, yPosition), true);
                                top = false;
                            }
                        }
                    else
                        top = false;
                    if(xPosition+i < Columns & bottom){
                            if(CheckTileForPiece(xPosition + i, yPosition) == null)
                                MarkBoardTile(GetBoardTile(xPosition + i, yPosition), true);
                            if(CheckTileForPiece(xPosition + i, yPosition) != null){
                                if(CheckTileForPiece(xPosition + i, yPosition).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition + i, yPosition), true);
                                bottom = false;
                            }
                        }
                    else
                        bottom = false;
                    if(yPosition-i >= 0 & left){
                            if(CheckTileForPiece(xPosition, yPosition - i) == null)
                                MarkBoardTile(GetBoardTile(xPosition, yPosition - i), true);
                            if(CheckTileForPiece(xPosition, yPosition - i) != null){
                                if(CheckTileForPiece(xPosition, yPosition - i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition, yPosition - i), true);
                                left = false;
                            }
                        }
                    else
                        left = false;
                    if(yPosition+i < Columns & right){
                            if(CheckTileForPiece(xPosition, yPosition + i) == null)
                                MarkBoardTile(GetBoardTile(xPosition, yPosition + i), true);
                            if(CheckTileForPiece(xPosition, yPosition + i) != null){
                                if(CheckTileForPiece(xPosition, yPosition + i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition, yPosition + i), true);
                                right = false;
                            }
                        }
                    else
                        right = false;
                }
                for(int i = 1; i < Columns; i++){
                    //GD.Print($"TL: {topLeft} | TR: {topRight} | BL: {bottomLeft} | BR: {bottomRight}");
                    if(xPosition-i >= 0 & yPosition-i >= 0 &
                        topLeft){
                            if(CheckTileForPiece(xPosition - i, yPosition - i) == null)
                                MarkBoardTile(GetBoardTile(xPosition - i, yPosition - i), true);
                            if(CheckTileForPiece(xPosition - i, yPosition - i) != null){
                                if(CheckTileForPiece(xPosition - i, yPosition - i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition - i, yPosition - i), true);
                                topLeft = false;
                            }
                        }
                    else
                        topLeft = false;
                    if(xPosition+i < Columns & yPosition-i >= 0 &
                        topRight){
                            if(CheckTileForPiece(xPosition + i, yPosition - i) == null)
                                MarkBoardTile(GetBoardTile(xPosition + i, yPosition - i), true);
                            if(CheckTileForPiece(xPosition + i, yPosition - i) != null){
                                if(CheckTileForPiece(xPosition + i, yPosition - i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition + i, yPosition - i), true);
                                topRight = false;
                            }
                        }
                    else
                        topRight = false;
                    if(xPosition-i >= 0 & yPosition+i < Columns &
                        bottomLeft){
                            if(CheckTileForPiece(xPosition - i, yPosition + i) == null)
                                MarkBoardTile(GetBoardTile(xPosition - i, yPosition + i), true);
                            if(CheckTileForPiece(xPosition - i, yPosition + i) != null){
                                if(CheckTileForPiece(xPosition - i, yPosition + i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition - i, yPosition + i), true);
                                bottomLeft = false;
                            }
                        }
                    else 
                        bottomLeft = false;
                    if(xPosition+i < Columns & yPosition+i < Columns &
                        bottomRight){
                            if(CheckTileForPiece(xPosition + i, yPosition + i) == null)
                                MarkBoardTile(GetBoardTile(xPosition + i, yPosition + i), true);
                            if(CheckTileForPiece(xPosition + i, yPosition + i) != null){
                                if(CheckTileForPiece(xPosition + i, yPosition + i).PieceColor != piece.PieceColor)
                                    MarkBoardTile(GetBoardTile(xPosition + i, yPosition + i), true);
                                bottomRight = false;
                            }
                        }
                    else
                        bottomRight = false;
                }
            break;
            case PieceTypes.King:
                // top left
                if(CheckTileForPiece(xPosition-1 , yPosition-1) == null)
                    MarkBoardTile(GetBoardTile(xPosition-1, yPosition-1), true);
                if(CheckTileForPiece(xPosition-1, yPosition-1) != null)
                    if(CheckTileForPiece(xPosition-1, yPosition-1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition-1, yPosition-1), true);
                // top 
                if(CheckTileForPiece(xPosition , yPosition-1) == null)
                    MarkBoardTile(GetBoardTile(xPosition, yPosition-1), true);
                if(CheckTileForPiece(xPosition, yPosition-1) != null)
                    if(CheckTileForPiece(xPosition, yPosition-1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition, yPosition-1), true);
                // top right
                if(CheckTileForPiece(xPosition+1 , yPosition-1) == null)
                    MarkBoardTile(GetBoardTile(xPosition+1, yPosition-1), true);
                if(CheckTileForPiece(xPosition+1, yPosition-1) != null)
                    if(CheckTileForPiece(xPosition+1, yPosition-1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition+1, yPosition-1), true);
                // left
                if(CheckTileForPiece(xPosition-1 , yPosition) == null)
                    MarkBoardTile(GetBoardTile(xPosition-1, yPosition), true);
                if(CheckTileForPiece(xPosition-1, yPosition) != null)
                    if(CheckTileForPiece(xPosition-1, yPosition).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition-1, yPosition), true);
                // right
                if(CheckTileForPiece(xPosition+1 , yPosition) == null)
                    MarkBoardTile(GetBoardTile(xPosition+1, yPosition), true);
                if(CheckTileForPiece(xPosition+1, yPosition) != null)
                    if(CheckTileForPiece(xPosition+1, yPosition).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition+1, yPosition), true);
                // bottom left
                if(CheckTileForPiece(xPosition-1 , yPosition+1) == null)
                    MarkBoardTile(GetBoardTile(xPosition-1, yPosition+1), true);
                if(CheckTileForPiece(xPosition-1, yPosition+1) != null)
                    if(CheckTileForPiece(xPosition-1, yPosition+1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition-1, yPosition+1), true);
                // bottom
                if(CheckTileForPiece(xPosition , yPosition+1) == null)
                    MarkBoardTile(GetBoardTile(xPosition, yPosition+1), true);
                if(CheckTileForPiece(xPosition, yPosition+1) != null)
                    if(CheckTileForPiece(xPosition, yPosition+1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition, yPosition+1), true);
                // bottom right
                if(CheckTileForPiece(xPosition+1 , yPosition+1) == null)
                    MarkBoardTile(GetBoardTile(xPosition+1, yPosition+1), true);
                if(CheckTileForPiece(xPosition+1, yPosition+1) != null)
                    if(CheckTileForPiece(xPosition+1, yPosition+1).PieceColor != piece.PieceColor)
                        MarkBoardTile(GetBoardTile(xPosition+1, yPosition+1), true);
            break;
        }
    }
    private void MarkBoardTile(BoardTile markedTile, bool visible){
        if(markedTile != null){
            markedTile.MarkerVisibility(visible);
        }
    }
    private void ClearMarkers(){
        for (int i=0; i < MathF.Pow(Columns, 2); i++){
            MarkBoardTile(GetBoardTile(i%8, i/8), false);
        }
    }
    // again, self explanatory
    private void OnChessPieceClicked(ChessPiece chessPiece){
        MarkBoardTiles(chessPiece);

        if(gVar.LastPieceClicked is null)
            gVar.LastPieceClicked = chessPiece;
        else{
            gVar.LastPieceClicked = null;
            ClearMarkers();
        }

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
