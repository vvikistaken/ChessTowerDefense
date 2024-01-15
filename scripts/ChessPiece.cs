using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class ChessPiece : Node2D
{
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
	public enum PieceTypes{
		Pawn,
		Bishop,
		Knight,
		Rook,
		Queen,
		King
	}
	public enum PieceColors{
		Light,
		Dark
	}
	
	private string _pieceToLoad;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetPiece();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Sprite2D>("PieceSprite").Texture = GD.Load<Texture2D>("res://graphics/sprites/pieces/"+_pieceToLoad+".png");
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
