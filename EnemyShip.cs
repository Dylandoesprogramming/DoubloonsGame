using Godot;
using System;

public partial class EnemyShip : Area2D
{
    [Export]
    public PackedScene CannonballScene;

    [Export]
    public int MaxSpeed { get; set; } = 50;

    [Export]
    public int MinSpeed { get; set; } = 20;

    [Export]
    public string Faction { get; set; } = "Enemy";

    [Export]
    public int MaxHealth { get; set; } = 30;

    [Signal]
    public delegate void EnemySankEventHandler();

    private Random random = new Random();
    private int curHealth = 30;
    private float curSpeed = 0f;
    private AnimatedSprite2D animatedSprite;
    private bool firedLeft = false;
    private bool firedRight = false;
    private bool leftCannonAnimTimerRunning = false;
    private bool rightCannonAnimTimerRunning = false;
    private Timer leftCannonTimer;
    private Timer rightCannonTimer;
    private Timer leftCannonAnimTimer;
    private Timer rightCannonAnimTimer;

    private Marker2D topLeftCannonMarker;
    private Marker2D centerUpperLeftCannonMarker;
    private Marker2D centerLowerLeftCannonMarker;
    private Marker2D bottomLeftCannonMarker;

    private Marker2D topRightCannonMarker;
    private Marker2D centerUpperRightCannonMarker;
    private Marker2D centerLowerRightCannonMarker;
    private Marker2D bottomRightCannonMarker;

    private RayCast2D topLeftCannonRay;
    private RayCast2D bottomLeftCannonRay;
    private RayCast2D topRightCannonRay;
    private RayCast2D bottomRightCannonRay;

    private ProgressBar healthBar;

    private AudioStreamPlayer2D cannonAudioStreamPlayer;
    private AudioStreamPlayer2D damageAudioStreamPlayer;

    public override void _Ready()
    {
        MaxSpeed = (int)random.NextInt64(MinSpeed, MaxSpeed);
        curHealth = MaxHealth;
        healthBar = GetNode<ProgressBar>("HealthBar");
        healthBar.MaxValue = MaxHealth;
        healthBar.Value = curHealth;
        cannonAudioStreamPlayer = GetNode<AudioStreamPlayer2D>("CannonAudioStreamPlayer2D");
        damageAudioStreamPlayer = GetNode<AudioStreamPlayer2D>("DamageAudioStreamPlayer2D");
        animatedSprite = GetNode<AnimatedSprite2D>("EnemyAnimSprite2D");
        leftCannonTimer = GetNode<Timer>("LeftCannonTimer");
        rightCannonTimer = GetNode<Timer>("RightCannonTimer");
        leftCannonAnimTimer = GetNode<Timer>("LeftCannonAnimTimer");
        rightCannonAnimTimer = GetNode<Timer>("RightCannonAnimTimer");

        topLeftCannonMarker = GetNode<Marker2D>("TopLeftCannonMarker");
        centerUpperLeftCannonMarker = GetNode<Marker2D>("CenterUpperLeftCannonMarker");
        centerLowerLeftCannonMarker = GetNode<Marker2D>("CenterLowerLeftCannonMarker");
        bottomLeftCannonMarker = GetNode<Marker2D>("BottomLeftCannonMarker");

        topRightCannonMarker = GetNode<Marker2D>("TopRightCannonMarker");
        centerUpperRightCannonMarker = GetNode<Marker2D>("CenterUpperRightCannonMarker");
        centerLowerRightCannonMarker = GetNode<Marker2D>("CenterLowerRightCannonMarker");
        bottomRightCannonMarker = GetNode<Marker2D>("BottomRightCannonMarker");

        topLeftCannonRay = GetNode<RayCast2D>("TopLeftCannonRay");
        bottomLeftCannonRay = GetNode<RayCast2D>("BottomLeftCannonRay");
        topRightCannonRay = GetNode<RayCast2D>("TopRightCannonRay");
        bottomRightCannonRay = GetNode<RayCast2D>("BottomRightCannonRay");
    }

    public override void _Process(double delta)
    {
        healthBar.Value = curHealth;
        SetEnemyAnim();
        MoveForward(delta);
        CheckForPlayerInRange();
    }

    public void HandleCannonballHit()
    {
        damageAudioStreamPlayer.Play();
        curHealth -= 10;
    }

    public void OnEnemyLeftScreen()
    {
        QueueFree();
    }

    public void CheckForDeath()
    {
        if (curHealth <= 0)
        {
            EmitSignal(SignalName.EnemySank);
            QueueFree();
        }
    }

    private void SetEnemyAnim()
    {
        if (firedLeft && firedRight && leftCannonAnimTimerRunning && rightCannonAnimTimerRunning)
        {
            animatedSprite.Animation = "fire_both";
        }
        else if (firedLeft && leftCannonAnimTimerRunning)
        {
            animatedSprite.Animation = "fire_left";
        }
        else if (firedRight && rightCannonAnimTimerRunning)
        {
            animatedSprite.Animation = "fire_right";
        }
        else
        {
            animatedSprite.Animation = "forward";
        }
    }

    private void MoveForward(double delta)
    {
        if (curSpeed < MaxSpeed)
        {
            curSpeed = MaxSpeed;  // Enemy ships will always move at max speed for simplicity
        }

        var forwardDirection = new Vector2(Mathf.Cos(Rotation - Mathf.Pi / 2), Mathf.Sin(Rotation - Mathf.Pi / 2));
        Position += forwardDirection * curSpeed * (float)delta;
    }

    private void CheckForPlayerInRange()
    {
        if (topLeftCannonRay.IsColliding() && topLeftCannonRay.GetCollider() is PlayerShip || bottomLeftCannonRay.IsColliding() && bottomLeftCannonRay.GetCollider() is PlayerShip)
        {
            if (!firedLeft)
            {
                FireCannons("left");
            }
        }
        if (topRightCannonRay.IsColliding() && topRightCannonRay.GetCollider() is PlayerShip || bottomRightCannonRay.IsColliding() && bottomRightCannonRay.GetCollider() is PlayerShip)
        {
            if (!firedRight)
            {
                FireCannons("right");
            }
        }
    }

    private void FireCannons(string direction)
    {
        float shipRotation = Rotation;
        cannonAudioStreamPlayer.Play();
        if (direction == "left" && !firedLeft)
        {
            firedLeft = true;
            leftCannonTimer.Start();
            leftCannonAnimTimer.Start();
            leftCannonAnimTimerRunning = true;
            float fireAngle = shipRotation + Mathf.Pi;

            SpawnCannonball(topLeftCannonMarker.GlobalPosition, fireAngle);
            SpawnCannonball(centerUpperLeftCannonMarker.GlobalPosition, fireAngle);
            SpawnCannonball(centerLowerLeftCannonMarker.GlobalPosition, fireAngle);
            SpawnCannonball(bottomLeftCannonMarker.GlobalPosition, fireAngle);
        }
        else if (direction == "right" && !firedRight)
        {
            firedRight = true;
            rightCannonTimer.Start();
            rightCannonAnimTimer.Start();
            rightCannonAnimTimerRunning = true;
            float fireAngle = shipRotation;

            SpawnCannonball(topRightCannonMarker.GlobalPosition, fireAngle);
            SpawnCannonball(centerUpperRightCannonMarker.GlobalPosition, fireAngle);
            SpawnCannonball(centerLowerRightCannonMarker.GlobalPosition, fireAngle);
            SpawnCannonball(bottomRightCannonMarker.GlobalPosition, fireAngle);
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

    private void OnRightCannonAnimTimerTimeout()
    {
        rightCannonAnimTimerRunning = false;
    }

    private void OnLeftCannonAnimTimerTimeout()
    {
        leftCannonAnimTimerRunning = false;
    }

    private void SpawnCannonball(Vector2 spawnPosition, float angle)
    {
        Cannonball cannonballInstance = (Cannonball)CannonballScene.Instantiate<Cannonball>();
        cannonballInstance.Faction = Faction;
        cannonballInstance.Position = spawnPosition;
        cannonballInstance.Rotation = angle;
        GetTree().CurrentScene.AddChild(cannonballInstance);
    }
}
