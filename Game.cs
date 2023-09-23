using Godot;
using Microsoft.VisualBasic;
using System;
using System.Text.Json;

public partial class Game : Node2D
{
	[Export]
	public PackedScene MainPackedScene { get; set; }
	
	[Export]
	public PackedScene MenuPackedScene { get; set; }
	
	

	private const string HighScoreFilePath = "user://HighScores.dat";

    private int curHighScore;

	private Menu menu;
	private Main main;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		menu = GetNode<Menu>("Menu");
		GetOrCreateHighScoreFile();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	public void ReturnToMenu()
	{
		var instantiatedMenu = MenuPackedScene.Instantiate<Menu>();
		menu = instantiatedMenu;
        AddChild(menu);
		main.QueueFree();
        ChecknSaveHighScore(main.DeathScore);

		var callablePlay = new Callable(this, "OnPlayPressed");
		var callableQuit = new Callable(this, "OnQuitPressed");

		menu.Connect("PressPlay", callablePlay);
		menu.Connect("PressQuit", callableQuit);
    }

	public void OnPlayPressed()
	{
		GD.Print("Play Game");
		var instantiatedMain = MainPackedScene.Instantiate<Main>();
		main = instantiatedMain;
		AddChild(main);
		var callable = new Callable(this, "ReturnToMenu");
		main.Connect("GotoMenu", callable);
		menu.QueueFree();
	}

	public void OnQuitPressed()
	{
		GD.Print("Quitting");
		GetTree().Quit();
	}

    public void GetOrCreateHighScoreFile()
    {
        using var file = FileAccess.Open(HighScoreFilePath, FileAccess.ModeFlags.WriteRead);

		var contents = file.GetAsText();

		if(String.IsNullOrEmpty(contents))
		{
			file.StoreString("0");
            file.Flush();
            contents = file.GetAsText();
		}
		SetHighScore(contents);
    }

	public void ChecknSaveHighScore(int score)
	{
		GD.Print("Check Score: " + score);
        using var file = FileAccess.Open(HighScoreFilePath, FileAccess.ModeFlags.ReadWrite);
		var  contents = file.GetAsText();
		GD.Print("Pre save contents: "+ contents);
		if(Int32.Parse(contents) < score)
		{
			file.StoreString(score.ToString());
			file.Flush();
			contents = file.GetAsText();
			GD.Print("Post save contents: " + contents);
		}
        SetHighScore(contents);
    }

	private void SetHighScore(string highScore)
	{
        curHighScore = Int32.Parse(highScore);
        menu.SetHighScore(curHighScore);
    }
}
