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
    public int X, Y;
    
    private Color _darkTile = Color.FromHtml("#afc3dc"),
                 _lightTile = Color.FromHtml("#eceef0");

    private TextureRect _marker;

    public override void _Ready()
    {
        ChangeTileColor();

        GuiInput += MouseInputs;
        _marker = GetNode<TextureRect>("Marker");
    }
    public override void _Process(double delta)
    {
        GetNode<Label>("TileId").Text = Name;
    }
    public void MarkerVisibility(bool visible){
        _marker.Visible = visible;
    }
    private void MouseInputs(InputEvent @event)
    {
		if( @event is InputEventMouseButton &&
			 @event.IsPressed() ){
            GD.Print(this.Name+" - X: "+X+", Y: "+Y);
            EmitSignal(SignalName.BoardTileClicked, this);
		}
    }
    private void ChangeTileColor(){
        this.Color = TileColor == TileColors.Dark ? _darkTile : _lightTile;
    }
}
