using Godot;
using System;

public partial class UI : CanvasLayer
{
	private GlobalVariables gVar;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		gVar = GetNode<GlobalVariables>("/root/GlobalVariables");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetNode<Label>("TurnLabel").Text = "Current Turn:\n"+gVar.CurrentRound;
	}
}
