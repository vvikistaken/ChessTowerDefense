using Godot;
using System;
using TileColors = GlobalVariables.ChessPieceColors;

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
    public int X, Y;
    
    private Color _darkTile = Color.FromHtml("#afc3dc"),
                 _lightTile = Color.FromHtml("#eceef0");

    private GlobalVariables gVar;
    private TextureRect _marker;

    public override void _Ready()
    {
        _marker = GetNode<TextureRect>("Marker");
        gVar = GetNode<GlobalVariables>("/root/GlobalVariables");

        ChangeTileColor();

        GuiInput += MouseInputs;
    }
    private void MouseInputs(InputEvent @event)
    {
		if( Input.IsActionJustPressed("left_click") ){
            GD.Print(this.Name+" - X: "+X+", Y: "+Y);
            EmitSignal(SignalName.BoardTileClicked, this);
		}
    }
    public override void _Process(double delta)
    {
        GetNode<Label>("TileId").Text = Name + "\nx:"+X+"|y:"+Y;
    }
    public void MarkerVisibility(bool visible){
        _marker.Visible = visible;
    }
    private void ChangeTileColor(){
        this.Color = TileColor == TileColors.Dark ? _darkTile : _lightTile;
    }
}
