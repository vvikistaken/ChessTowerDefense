using Godot;
using PieceTypes = GlobalVariables.ChessPieceTypes;
using PieceColors = GlobalVariables.ChessPieceColors;
using System.Linq.Expressions;

public partial class ChessPiece : Node2D
{
	[Signal]
	public delegate void MoveChessPieceEventHandler(ChessPiece chessPiece);
	[Signal]
	public delegate void InspectChessPieceEventHandler(ChessPiece chessPiece);
	private PieceTypes _pieceType;
	private PieceColors _pieceColor;
	private bool _moveState = false;
	[Export]
	public PieceTypes PieceType { 
		get{ return _pieceType; }
		set{ 
			_pieceType = value;
			SetPiece();
		}
	}
	[Export]
	public PieceColors PieceColor{ 
		get{ return _pieceColor; }
	 	set{ 
			_pieceColor = value;
			SetPiece();
		} 
	}
	private bool MoveState
	{
		get { return _moveState; }
		set 
		{ 
			_moveState = value;
			Highlight(_moveState);
		}
	}
	public bool FirstMove = true;
	private string _pieceToLoad;

	private GlobalVariables gVar;
	private Control _clickBox;
	private Sprite2D _pieceSprite;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_clickBox = GetNode<Control>("ClickBox");
		_pieceSprite = GetNode<Sprite2D>("PieceSprite");

		SetPiece();

		_clickBox.GuiInput += MouseInputs;
		gVar = GetNode<GlobalVariables>("/root/GlobalVariables");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_pieceSprite.Texture = GD.Load<Texture2D>("res://graphics/sprites/pieces/"+_pieceToLoad+".png");
		
		if(gVar.LastPieceClicked != this){
			MoveState = false;
		}
	}
    public override void _Input(InputEvent @event)
    {
        if((Input.IsActionJustPressed("left_click") |
		   Input.IsActionJustPressed("right_click")))
			Highlight(false);
    }
    // DO NOT PUT IT TO _INPUT IT WILL FUCK IT UP

    private void MouseInputs(InputEvent @event)
    {
		if( @event is InputEventMouseButton & gVar.CurrentRound == PieceColor ){
			_clickBox.MouseFilter = Control.MouseFilterEnum.Stop;

			if(Input.IsActionJustPressed("left_click")){
				//GD.Print("left click works!");
				/*
				GD.Print("Color: "+PieceColor);
				GD.Print("Type: "+PieceType);
				GD.Print("------------------------------");
				*/
				MoveState = !MoveState;
				
				EmitSignal(SignalName.MoveChessPiece, this);
			}
			if(Input.IsActionJustPressed("right_click")){
				//GD.Print("right click works!");
				Highlight(!IsHighlighed());

				EmitSignal(SignalName.InspectChessPiece, this);
				if(gVar.LastPieceClicked != this)
					// opening
					gVar.EmitSignal(GlobalVariables.SignalName.InspectChessPieceUI, this, true);
				else
					// closing
					gVar.EmitSignal(GlobalVariables.SignalName.InspectChessPieceUI, this, false);
			}
		}
		else{
			_clickBox.MouseFilter = Control.MouseFilterEnum.Pass;
		}
    }
    private void SetPiece(){
		_pieceToLoad = "";
		switch(PieceColor){
			case PieceColors.Light:
				_pieceToLoad += "white";
			break;
			case PieceColors.Dark:
				_pieceToLoad += "black";
			break;
		}

		_pieceToLoad += "Piece" + ((int)PieceType+1);
		//GD.Print(_pieceToLoad);
	}
	private void Highlight(bool isInverted){
		// shader doesnt work for some reason
		//((ShaderMaterial)_pieceSprite.Material).SetShaderParameter("Invert", isInverted);
		if(isInverted){
			if(PieceColor == PieceColors.Light)
				_pieceSprite.Modulate = Color.FromHtml("#7ff");
			if(PieceColor == PieceColors.Dark)
				_pieceSprite.Modulate = Color.FromHtml("#f77");
		}
			//_pieceSprite.Modulate = _pieceSprite.Modulate.Inverted();
			
		else
			_pieceSprite.Modulate = Color.FromHtml("#fff");
		//GD.Print("Highlight works");
	}
	private bool IsHighlighed(){
		if(_pieceSprite.Modulate == Color.FromHtml("#7ff") | _pieceSprite.Modulate == Color.FromHtml("#f77"))
			return true;
		else
			return false;
	}

}
