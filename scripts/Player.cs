using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float WalkSpeed = 5.0f;
    [Export] public float SprintSpeed = 8.0f;
    [Export] public float Acceleration = 60.0f;
	[Export] public float JumpForce = 4.5f;

    [Export] public float JetpackForce = 3.0f;
    [Export] public float JetpackMaxFuel = 1.5f;
    [Export] public float JetpackFuelRegenSpeed = 4.0f;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
    public bool sprinting = false;
    public bool jetpackOn = false;

    public float jetpackFuel = 0f;

    private ShoulderCamera shoulderCamera;
    private float currentSpeed;

    public override void _Ready()
    {
        shoulderCamera = (ShoulderCamera)GetNode("ShoulderCamera");
    }

    public override void _Process(double delta)
    {
        // TEMP
        if (Input.IsActionJustPressed("quit_temp"))
        {
            GetTree().Quit();
        }
    }

    public override void _PhysicsProcess(double delta)
	{
		ProcessMovement(delta);

	}

	private void ProcessMovement(double delta)
	{
        Vector3 newVelocity = Velocity;

        // Horizontal movement
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        Vector2 localInputDir = inputDir.Rotated(-shoulderCamera.Rotation.Y);
        Vector3 directionH = (Transform.Basis * new Vector3(localInputDir.X, 0, localInputDir.Y)).Normalized();

        if (Input.IsActionPressed("sprint"))
        {
            sprinting = true;
            currentSpeed = SprintSpeed;
        }
        else
        {
            sprinting = false;
            currentSpeed = WalkSpeed;
        }

        Vector3 targetVelocityH = directionH * currentSpeed;
        newVelocity.X = (float)Mathf.MoveToward(Velocity.X, targetVelocityH.X, Acceleration * delta);
        newVelocity.Z = (float)Mathf.MoveToward(Velocity.Z, targetVelocityH.Z, Acceleration * delta);

		// Vertical movement
		newVelocity.Y -= gravity * (float)delta;

		if (Input.IsActionJustPressed("jump"))
		{
            // Jump
            if (IsOnFloor())
            {
                newVelocity.Y = JumpForce;
            }
            else
            {
                jetpackOn = true;
            }
			
		}
        // Process jetpack
        if (jetpackOn)
        {
            newVelocity.Y = Mathf.Max(newVelocity.Y, JetpackForce);
            jetpackFuel -= (float)delta;
            if (Input.IsActionJustReleased("jump") || jetpackFuel <= 0f)
            {
                jetpackOn = false;
            }
        }
        // Jetpack fuel regen
        if (IsOnFloor() && jetpackOn == false)
        {
            jetpackFuel += JetpackFuelRegenSpeed * (float)delta;
            jetpackFuel = Mathf.Clamp(jetpackFuel, 0, JetpackMaxFuel);
        }

        Velocity = newVelocity;
        MoveAndSlide();
    }
}
