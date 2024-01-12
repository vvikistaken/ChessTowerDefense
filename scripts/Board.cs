using Godot;
using System;

public partial class Board : GridContainer
{
    [Export]
    public PackedScene BoardTileScene;
    public const int BoardTileSize = 64;

    public override void _Ready()
    {
        /*
        _boardContainer.Size = new Vector2(
            x: BoardTileSize * _boardContainer.Columns,
            y: BoardTileSize * _boardContainer.Columns
        );
         */
        for(int i=0; i < MathF.Pow(Columns, 2); i++){
            CreateTile(i);
        }
    }
    private void CreateTile(int tileId){
        var boardTile = BoardTileScene.Instantiate<BoardTile>();

        AddChild(boardTile);

        if((tileId / 8) % 2 == 1 )
            boardTile.SetTileColor(BoardTile.TileColors.Dark); 
        else
            boardTile.SetTileColor(BoardTile.TileColors.Light); 

    }
}
