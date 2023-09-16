using Godot;
using System;

public partial class PlayerShip : Area2D
{
	[Export]
	public int Speed { get; set; } = 400;

	private AnimatedSprite2D animatedSprite;
	private bool firedLeft = false;
	private bool firedRight = false;
	private Timer leftCannonTimer;
	private Timer rightCannonTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("PlayerAnimSprite2D");
		leftCannonTimer = GetNode<Timer>("LeftCannonTimer");
		rightCannonTimer = GetNode<Timer>("RightCannonTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		SetPlayerAnim();

		if(!firedLeft && Input.IsActionPressed("fire_left"))
		{
			FireCannons("left");
		}

		if(!firedRight && Input.IsActionPressed("fire_right"))
		{
			FireCannons("right");
		}
	}

	private void SetPlayerAnim()
	{
        if (firedLeft && firedRight)
        {
            animatedSprite.Animation = "fire_both";
        }
        else if (firedLeft)
        {
            animatedSprite.Animation = "fire_left";
        }
        else if (firedRight)
        {
            animatedSprite.Animation = "fire_right";
        }
        else
        {
            animatedSprite.Animation = "forward";
        }
    }

	private void FireCannons(string direction)
	{
		if(direction == "left")
		{
			firedLeft = true;
			leftCannonTimer.Start();
		}
		else if(direction == "right") 
		{
			firedRight = true;
			rightCannonTimer.Start();
		}
	}

	private void OnLeftCannonTimerTimeout()
	{
		firedLeft = false;
	}

	private void OnRightCannonTimerTimeout()
	{
		firedRight = false;
	}

}
