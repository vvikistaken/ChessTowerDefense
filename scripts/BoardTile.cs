using Godot;
using System;

public partial class BoardTile : ColorRect
{
    [Signal]
    public delegate void BoardTileClickedEventHandler(BoardTile boardTile);
    [Export]
    public TileColors TileColor { 
        get{ return _tileColor; } 
        set{
            _tileColor = value;
            ChangeTileColor();
        } 
    }
    private TileColors _tileColor;
    public enum TileColors{
        Light,
        Dark 
    }
    
    private Color _darkTile = Color.FromHtml("#769656"),
                 _lightTile = Color.FromHtml("#eeeed2");

    public override void _Ready()
    {
        ChangeTileColor();

        GuiInput += MouseInputs;
    }
    public override void _Process(double delta)
    {
        GetNode<Label>("TileId").Text = Name;
    }
    private void MouseInputs(InputEvent @event)
    {
		if(Input.IsActionJustPressed("click")){
            //GD.Print(this.Name+" clicked");
            EmitSignal(SignalName.BoardTileClicked, this);
		}
        	
    }
    private void ChangeTileColor(){
        this.Color = TileColor == TileColors.Dark ? _darkTile : _lightTile;
    }
}
