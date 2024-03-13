using Godot;
using System;

public partial class UI : CanvasLayer
{
	private GlobalVariables gVar;
	private Board _board;
	private ChessPiece _chessPiece;
	private AnimationPlayer _animationPlayer;
	private bool _isPieceMenuOpen=false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gVar = GetNode<GlobalVariables>("/root/GlobalVariables");
		_board = GetNode<Board>("../Board");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		// later do a method for that
		_animationPlayer.AnimationFinished += OnAnimationFinished;

		GetNode<Button>("EndScreen/RetryButton").Pressed += OnRetryPressed;

		_board.GameEnd += OnGameEnd;
		gVar.InspectChessPieceUI += OnInspectChessPieceUI;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Label>("TurnLabel").Text = "Current Turn:\n"+gVar.CurrentRound;
		GetNode<Label>("CashVisual/CashLabel").Text = ""+ gVar.PlayerCash;
	}
    public override void _Input(InputEvent @event)
    {
		if((Input.IsActionJustPressed("left_click") | 
		   Input.IsActionJustPressed("right_click")) &
		   _isPieceMenuOpen
		){
			//IsPieceMenuOpen = false;
			//GD.Print("piece menu open: "+IsPieceMenuOpen);
			GetNode<AnimationPlayer>("AnimationPlayer").Play("chessPieceMenu", -1,-1, true);
		}
    }
    private async void OnGameEnd(GlobalVariables.ChessPieceColors winners){
		GetNode<Label>("EndScreen/EndLabel").Text = winners + " won";
		GetNode<Control>("EndScreen").Visible = true;
		
		await ToSignal(GetTree().CreateTimer(1), Timer.SignalName.Timeout);
		GetTree().Paused = true;
	}
	private void OnRetryPressed(){
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/main.tscn");
		gVar.CurrentRound = GlobalVariables.ChessPieceColors.Light;
	}
	private void OnInspectChessPieceUI(ChessPiece chessPiece, bool OpenState){
		//_isPieceMenuOpen = OpenState;
		GD.Print("menu open state: "+OpenState);
		if(chessPiece.PieceTextureName != null)
			GetNode<TextureRect>("PieceMenu/Icon").Texture = GD.Load<Texture2D>($"res://graphics/sprites/pieces/{chessPiece.PieceTextureName}.png");
		GetNode<Label>("PieceMenu/TypeLabel").Text = "Piece Type:\n"+chessPiece.PieceType;
		GetNode<Label>("PieceMenu/ColorLabel").Text = "Piece Color:\n"+chessPiece.PieceColor;

		if(OpenState)
			_animationPlayer.Play("chessPieceMenu", -1, OpenState ? 1 : -1, !OpenState);
	}
	private void OnAnimationFinished(StringName animName){
		if(animName == "chessPieceMenu")
			_isPieceMenuOpen = true;
	}
}
