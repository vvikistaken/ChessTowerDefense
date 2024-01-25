using Godot;
using PieceTypes = GlobalVariables.ChessPieceTypes;
using PieceColors = GlobalVariables.ChessPieceColors;

public partial class ChessPiece : Node2D
{
	[Signal]
	public delegate void ChessPieceClickedEventHandler(ChessPiece chessPiece);
	// for get/set
	private PieceTypes _pieceType;
	private PieceColors _pieceColor;
	private bool _moveState;
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
	private bool MoveState { 
		get{
			return _moveState;
		}	 
		set{
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
		
		if(gVar.LastPieceClicked != this)
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
			_clickBox.MouseFilter = Control.MouseFilterEnum.Stop;

			MoveState = !MoveState;
			EmitSignal(SignalName.ChessPieceClicked, this);
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

		_pieceToLoad += "Piece" + (int)PieceType;
		//GD.Print(_pieceToLoad);
	}
	private void Highlight(bool isInverted){
		// shader doesnt work for some reason
		//((ShaderMaterial)_pieceSprite.Material).SetShaderParameter("Invert", isInverted);
		if(isInverted)
			_pieceSprite.Modulate = _pieceSprite.Modulate.Inverted();
		else
			_pieceSprite.Modulate = Color.FromHtml("#fff");
		//GD.Print("Highlight works");
	}

}
