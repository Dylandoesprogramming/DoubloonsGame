using Godot;
using System;
using System.Security;

public partial class Cannonball : Area2D
{
    [Export]
    public float Speed { get; set; } = 400f;  // You can adjust the speed value in the inspector

    [Export]
    public string Faction { get; set; } = "Player";

    private AnimatedSprite2D animatedSprite;
    private Timer cannonballSplashTimer;
    private Timer cannonballDeleteTimer;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("CannonballAnimatedSprite2D");
        cannonballSplashTimer = GetNode<Timer>("CannonballSplashTimer");
        cannonballDeleteTimer = GetNode<Timer>("CannonballDeleteTimer");

        // Start the splash timer to handle cannonball "miss" logic
        cannonballSplashTimer.Start();
    }

    public override void _Process(double delta)
    {
        // Make the cannonball move straight based on its rotation
        var forwardDirection = new Vector2(Mathf.Cos(Rotation), Mathf.Sin(Rotation));
        Position += forwardDirection * Speed * (float)delta;
    }

    public void OnCannonballSplashTimerTimeout()
    {
        // Play the miss animation for a miss
        animatedSprite.Play("miss");
        // Start the delete timer to remove the cannonball after playing the sink animation
        cannonballDeleteTimer.Start();
    }

    public void OnCannonballDeleteTimerTimeout()
    {
        QueueFree();
    }

    public void OnCannonballCollide(Area2D area)
    {
        if(area is PlayerShip pShip)
        {
            if(pShip.Faction != Faction)
            {
                pShip.HandleCannonballHit();
                QueueFree();
            }
        }

        if(area is EnemyShip eShip)
        {
            if(eShip.Faction != Faction)
            {
                eShip.HandleCannonballHit();
                QueueFree();
            }
        }
    }
}
