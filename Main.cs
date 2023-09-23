using Godot;
using System;

public partial class Main : Node
{
	[Export]
	public int ScorePerKill { get; set; } = 500;

    [Signal]
    public delegate void GotoMenuEventHandler();

	public int DeathScore = 0;

    private ScoreKeeper scoreKeeper;
	public override void _Ready()
	{
		scoreKeeper = GetNode<ScoreKeeper>("ScoreKeeper");
	}

	public override void _Process(double delta)
	{
	}

	public void OnEnemyDeath()
	{
		scoreKeeper.UpdateScore(ScorePerKill);
	}

	public void OnPlayerDeath()
	{
		DeathScore = scoreKeeper.ReturnScore();
		GD.Print("Death Score: " + DeathScore);
		EmitSignal("GotoMenu");
	}
}
