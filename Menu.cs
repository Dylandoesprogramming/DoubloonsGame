using Godot;
using System;

public partial class Menu : Control
{
    [Signal]
    public delegate void PressPlayEventHandler();
    [Signal]
    public delegate void PressQuitEventHandler();
	[Export]
	public int HighScore { get; set; } = 0;
	private Label highScoreLabel;
    public override void _Ready()
	{
		highScoreLabel = GetNode<Label>("HighScoreLabel");	
	}

	public override void _Process(double delta)
	{
	}

	public void OnStartButtonPressed()
	{
		GD.Print("Emit PressPlay");
		EmitSignal("PressPlay");
	}

	public void OnQuitButtonPressed()
	{
		GD.Print("Emit PressQuit");
		EmitSignal("PressQuit");
	}

	public void SetHighScore(int highScore)
	{
		HighScore = highScore;
        highScoreLabel.Text = "Largest Haul: " + HighScore;
    }
}
