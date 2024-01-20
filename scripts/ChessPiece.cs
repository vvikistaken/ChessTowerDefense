using Godot;
using PieceTypes = GlobalVariables.ChessPieceTypes;
using PieceColors = GlobalVariables.ChessPieceColors;

public partial class ChessPiece : Node2D
{
	[Signal]
	public delegate void ChessPieceClickedEventHandler(ChessPiece chessPiece);
	[Export]
	public PieceTypes PieceType { 
		get{ return _pieceType; }
		set{ 
			_pieceType = value;
			SetPiece();
		}
	}
	private PieceTypes _pieceType;
	[Export]
	public PieceColors PieceColor{ 
		get{ return _pieceColor; }
	 	set{ 
			_pieceColor = value;
			SetPiece();
		} 
	}
	private PieceColors _pieceColor;
	private GlobalVariables gVar;
	private string _pieceToLoad;
	public bool MoveState = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPiece();

		GetNode<Control>("ClickBox").GuiInput += MouseInputs;
		gVar = GetNode<GlobalVariables>("/root/GlobalVariables");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var pieceSprite = GetNode<Sprite2D>("PieceSprite");
		pieceSprite.Texture = GD.Load<Texture2D>("res://graphics/sprites/pieces/"+_pieceToLoad+".png");
		
		if(MoveState){
			if(PieceColor == PieceColors.Light)
				pieceSprite.Modulate = Color.FromHtml("#74cad6");
			else if(PieceColor == PieceColors.Dark)
				pieceSprite.Modulate = Color.FromHtml("#d5102d");
		}
		else
			pieceSprite.Modulate = Color.FromHtml("#fff");

		if(gVar.LastChessPieceClicked != this)
			MoveState = false;
	}
    private void MouseInputs(InputEvent @event)
    {
		if(Input.IsActionJustPressed("click") && gVar.CurrentRound == PieceColor){
			/*
			GD.Print("Color: "+PieceColor);
			GD.Print("Type: "+PieceType);
			GD.Print("------------------------------");
			*/
			EmitSignal(SignalName.ChessPieceClicked, this);
			MoveState = !MoveState;
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

		_pieceToLoad += "Piece" + (int)PieceType;
		//GD.Print(_pieceToLoad);
	}

}
