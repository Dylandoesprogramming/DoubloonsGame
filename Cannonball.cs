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

    public void OnCannonballAreaEntered(Area2D area)
    {
        // Check if the cannonball hit an enemy ship or player ship
        if (area is EnemyShip || area is PlayerShip)
        {

            // Signal the ship that it was hit
            area.EmitSignal("HitByCannonball");

            // Delete
            QueueFree();
        }
    }

    public void OnCannonballSplashTimerTimeout()
    {
        // This function is called when the CannonballSplashTimer times out
        // Play the miss animation for a miss
        animatedSprite.Play("miss");
        // Start the delete timer to remove the cannonball after playing the sink animation
        cannonballDeleteTimer.Start();
    }

    public void OnCannonballDeleteTimerTimeout()
    {
        // Delete the cannonball
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
