using Godot;
using System;

public partial class PlayerShip : Area2D
{

    [Export]
    public PackedScene CannonballScene;

    [Export]
    public float MaxSpeed { get; set; } = 400f;

    [Export]
    public float MaxRotationSpeed { get; set; } = .5f;

    [Export]
    public float Deceleration { get; set; } = 5f;

    [Export]
    public float Accelleration = 5;

    [Export]
    public int MaxHealth = 100;

    [Export]
    public string Faction { get; set; } = "Player";

    [Signal]
    public delegate void PlayerSankEventHandler();

    [Signal]
    public delegate void PlayerLeftScreenEventHandler();

    private int curHealth = 100;
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

    private ProgressBar healthBar;

    private AudioStreamPlayer2D cannonAudioStreamPlayer;
    private AudioStreamPlayer2D damageAudioStreamPlayer;

    public override void _Ready()
    {
        curHealth = MaxHealth;
        healthBar = GetNode<ProgressBar>("HealthBar");
        healthBar.MaxValue = MaxHealth;
        healthBar.Value = curHealth;
        cannonAudioStreamPlayer = GetNode<AudioStreamPlayer2D>("CannonAudioStreamPlayer2D");
        damageAudioStreamPlayer = GetNode<AudioStreamPlayer2D>("DamageAudioStreamPlayer2D");
        animatedSprite = GetNode<AnimatedSprite2D>("PlayerAnimSprite2D");
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
    }

    public override void _Process(double delta)
    {
        SetPlayerAnim();
        healthBar.Value = curHealth;

        if (!firedLeft && Input.IsActionJustReleased("fire_left"))
        {
            FireCannons("left");
        }

        if (!firedRight && Input.IsActionJustReleased("fire_right"))
        {
            FireCannons("right");
        }

        if (Input.IsActionPressed("move_forward") && curSpeed < MaxSpeed)
        {
            curSpeed += Accelleration * (float)delta;
        }

        if (Input.IsActionPressed("slow_down"))
        {
            curSpeed -= Deceleration * (float)delta;
            if (curSpeed < 0)
            {
                curSpeed = 0;
            }
        }

        if (curSpeed > MaxSpeed)
        {
            curSpeed = MaxSpeed;
        }

        float currentRotationSpeed = CalculateRotationSpeed();

        // Only allow rotation if the ship is moving
        if (curSpeed > 0)
        {
            if (Input.IsActionPressed("turn_left"))
            {
                Rotation -= currentRotationSpeed * (float)delta;
            }

            if (Input.IsActionPressed("turn_right"))
            {
                Rotation += currentRotationSpeed * (float)delta;
            }
        }

        var forwardDirection = new Vector2(Mathf.Cos(Rotation - Mathf.Pi / 2), Mathf.Sin(Rotation - Mathf.Pi / 2));
        Position += forwardDirection * curSpeed * (float)delta;
    }

    public void HandleCannonballHit()
    {
        damageAudioStreamPlayer.Play();
        curHealth -= 10;
        if (curHealth <= 0)
        {
            EmitSignal(SignalName.PlayerSank);
            QueueFree();
        }
    }

    private void SetPlayerAnim()
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

    private void FireCannons(string direction)
    {
        cannonAudioStreamPlayer.Play();
        float shipRotation = Rotation;

        if (direction == "left")
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
        else if (direction == "right")
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

    private float CalculateRotationSpeed()
    {
        // Derived from a downward facing parabola based on curSpeed
        float factor = -4 * (curSpeed / MaxSpeed - 0.5f) * (curSpeed / MaxSpeed - 0.5f) + 1;
        // Clamp the lower value to 1/10th of MaxRotationSpeed
        var curRotationSpeed = Mathf.Clamp(MaxRotationSpeed * factor, MaxRotationSpeed / 10, MaxRotationSpeed);
        return curRotationSpeed;

    }

    private void SpawnCannonball(Vector2 spawnPosition, float angle)
    {
        Cannonball cannonballInstance = CannonballScene.Instantiate<Cannonball>();
        GetParent().AddChild(cannonballInstance);

        cannonballInstance.Faction = Faction;
        cannonballInstance.Position = spawnPosition;
        cannonballInstance.Rotation = angle;
    }

    private void OnPlayerCollide(Area2D area)
    {
        if(area is EnemyShip eShip)
        {
            eShip.QueueFree(); //placeholder for kill it
            curHealth -= 50;
        }
    }

    private void PlayerLeft()
    {
        EmitSignal("PlayerLeftScreen");
    }
}
