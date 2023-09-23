using Godot;
using System;

public partial class ScoreKeeper : Node2D
{
	[Export] int Score { get; set; } = 0;
	private Label ScoreLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ScoreLabel = GetNode<Label>("ScoreLabel");
        ResetScore();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void UpdateScore(int scoreToAdd)
	{
		Score += scoreToAdd;
		SetScoreText(Score);
	}

	public void ResetScore()
	{
		Score = 0;
		SetScoreText(Score);
    }

	public void SetScoreText(int score)
	{
        ScoreLabel.Text = "Doubloons: " + score.ToString();
    }

	public int ReturnScore()
	{
		return Score;
	}
}
