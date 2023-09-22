using Godot;
using System;

public partial class EnemySpawner : Node2D
{
    [Export]
    private PackedScene enemyShipScene;

    [Export]
    private float margin = 0;

    [Export]
    private float MinSpawnTime = 15.0f;

    [Export]
    private float MaxSpawnTime = 30.0f;

    private Random random = new Random();
    private Timer spawnTimer;

    public override void _Ready()
    {
        SpawnEnemyShip();
        spawnTimer = GetNode<Timer>("SpawnTimer");
        var randomWaitTime = MinSpawnTime + (float)random.NextDouble() * (MaxSpawnTime - MinSpawnTime);
        spawnTimer.WaitTime = randomWaitTime;
        spawnTimer.Start();
    }

    private void OnSpawnTimerTimeout()
    {
        SpawnEnemyShip();
    }

    private void SpawnEnemyShip()
    {
        var enemy = enemyShipScene.Instantiate<EnemyShip>();
        AddChild(enemy);

        // Random position along the viewport edge, considering the margin.
        var randomX = (float)random.NextDouble() * GetViewportRect().Size.X;
        var randomY = (float)random.NextDouble() * GetViewportRect().Size.Y;

        var edgeChoice = random.Next(4); // Randomly choose an edge: 0 = top, 1 = right, 2 = bottom, 3 = left

        switch (edgeChoice)
        {
            case 0: // Top
                randomY = -margin;
                break;
            case 1: // Right
                randomX = GetViewportRect().Size.X + margin;
                break;
            case 2: // Bottom
                randomY = GetViewportRect().Size.Y + margin;
                break;
            case 3: // Left
                randomX = -margin;
                break;
        }

        enemy.Position = new Vector2(randomX, randomY);

        // Determine target position (a random point on the opposite edge).
        Vector2 targetPosition;

        if (randomX <= 0)
        {
            targetPosition = new Vector2(GetViewportRect().Size.X + margin, (float)random.NextDouble() * GetViewportRect().Size.Y);
        }
        else if (randomX >= GetViewportRect().Size.X)
        {
            targetPosition = new Vector2(-margin, (float)random.NextDouble() * GetViewportRect().Size.Y);
        }
        else if (randomY <= 0)
        {
            targetPosition = new Vector2((float)random.NextDouble() * GetViewportRect().Size.X, GetViewportRect().Size.Y + margin);
        }
        else
        {
            targetPosition = new Vector2((float)random.NextDouble() * GetViewportRect().Size.X, -margin);
        }

        // Calculate direction to the target position.
        var directionToTarget = targetPosition - enemy.Position;

        // Adjust for sprite's orientation.
        var angle = Mathf.Atan2(directionToTarget.Y, directionToTarget.X) + Mathf.Pi / 2; // Add 90 degrees (in radians)
        enemy.Rotation = angle;
    }

}
