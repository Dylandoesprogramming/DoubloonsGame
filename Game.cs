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

    private int curHighScore = 0;

    private Menu menu;
    private Main main;

    public override void _Ready()
    {
        menu = GetNode<Menu>("Menu");
        GetOrCreateHighScoreFile();
    }

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

    public void CreateHighScoreFile()
    {
        using var file = FileAccess.Open(HighScoreFilePath, FileAccess.ModeFlags.WriteRead);

        file.StoreString("0");
        file.Close();
    }

    public void GetOrCreateHighScoreFile()
    {
        using var fileToRead = FileAccess.Open(HighScoreFilePath, FileAccess.ModeFlags.ReadWrite);
        if (String.IsNullOrEmpty(fileToRead.GetAsText()))
        {
            CreateHighScoreFile();
        }

        fileToRead.Close();
        using var file = FileAccess.Open(HighScoreFilePath, FileAccess.ModeFlags.ReadWrite);

        var contents = file.GetAsText();

        file.Close();

        SetHighScore(contents);
    }

    public void ChecknSaveHighScore(int score)
    {
        using var file = FileAccess.Open(HighScoreFilePath, FileAccess.ModeFlags.ReadWrite);
        var contents = file.GetAsText();

        if (Int32.Parse(contents) < score)
        {
            file.StoreString(score.ToString());
            contents = file.GetAsText();
            file.Close();
        }
        SetHighScore(contents);
    }

    private void SetHighScore(string highScore)
    {
        curHighScore = Int32.Parse(highScore);
        menu.SetHighScore(curHighScore);
    }
}
