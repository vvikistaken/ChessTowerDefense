using Godot;
using System;

public partial class UI : CanvasLayer
{
	private GlobalVariables gVar;
	private Board _board;
	private ChessPiece _chessPiece;
	private bool IsPieceInfoOpen=false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gVar = GetNode<GlobalVariables>("/root/GlobalVariables");
		_board = GetNode<Board>("../Board");

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
		if(Input.IsActionJustPressed("left_click") || Input.IsActionJustPressed("right_click"))
			if(IsPieceInfoOpen){
				IsPieceInfoOpen = false;
				GetNode<AnimationPlayer>("AnimationPlayer").Play("chessPieceInfoSlide", -1,-1, true);
			}
    }
    private async void OnGameEnd(GlobalVariables.ChessPieceColors winners){
		GetNode<Label>("EndScreen/EndLabel").Text = winners + " won";
		GetNode<Control>("EndScreen").Visible = true;
		
		await ToSignal(GetTree().CreateTimer(1), Timer.SignalName.Timeout);
		GetTree().Paused = true;
	}
	private void OnInspectChessPieceUI(ChessPiece chessPiece, bool IsOpening){
		IsPieceInfoOpen = true;
		// dont kno why it's reversed, no clue
		GetNode<AnimationPlayer>("AnimationPlayer").Play("chessPieceInfoSlide", -1, !IsOpening ? 1 : -1, IsOpening);
		GetNode<Label>("PieceInfo/TypeLabel").Text = "Piece Type:\n"+chessPiece.PieceType;
		GetNode<Label>("PieceInfo/ColorLabel").Text = "Piece Color:\n"+chessPiece.PieceColor;
	}
	private void OnRetryPressed(){
		GetTree().Paused = false;
		GetTree().ChangeSceneToFile("res://scenes/main.tscn");
		gVar.CurrentRound = GlobalVariables.ChessPieceColors.Light;
	}
}
