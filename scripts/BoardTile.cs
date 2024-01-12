using Godot;
using System;

public partial class BoardTile : ColorRect
{
    [Export]
    public TileColors TileColor;
    public enum TileColors{
        Light,
        Dark 
    }
    
    private Color _darkTile = Color.FromHtml("#769656"),
                 _lightTile = Color.FromHtml("#eeeed2");

    public override void _Ready()
    {
        UpdateTileColor();
    }
    public void SetTileColor(TileColors tileColor){
        TileColor = tileColor;
        UpdateTileColor();
    }
    private void UpdateTileColor(){
        this.Color = TileColor == TileColors.Dark ? _darkTile : _lightTile;
    }
}
