using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float WalkSpeed = 5.0f;
	[Export] public float JumpForce = 4.5f;

	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    private ShoulderCamera shoulderCamera;

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

        // Ruch w poziomie
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction != Vector3.Zero)
        {
            newVelocity.X = direction.X * WalkSpeed;
            newVelocity.Z = direction.Z * WalkSpeed;
        }
        else
        {
            newVelocity.X = Mathf.MoveToward(Velocity.X, 0, WalkSpeed);
            newVelocity.Z = Mathf.MoveToward(Velocity.Z, 0, WalkSpeed);
        }
        newVelocity = newVelocity.Rotated(Vector3.Up, shoulderCamera.Rotation.Y);

		// Ruch w pionie
		newVelocity.Y -= gravity * (float)delta;

		if (Input.IsActionJustPressed("jump"))
		{
			newVelocity.Y = JumpForce;
		}

        Velocity = newVelocity;
        MoveAndSlide();
    }
}
