using Godot;
using System;

public partial class UI : CanvasLayer
{
	private GlobalVariables gVar;
	private Board _board;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gVar = GetNode<GlobalVariables>("/root/GlobalVariables");
		_board = GetNode<Board>("../Board");

		GetNode<Button>("EndScreen/RetryButton").Pressed += OnRetryPressed;

		_board.GameEnd += OnGameEnd;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Label>("TurnLabel").Text = "Current Turn:\n"+gVar.CurrentRound;
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
}
