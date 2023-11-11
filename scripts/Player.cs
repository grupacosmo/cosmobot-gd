using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] public float WalkSpeed = 5.0f;
	[Export] public float JumpForce = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

    public override void _Process(double delta)
    {
        
    }

    public override void _PhysicsProcess(double delta)
	{
		ProcessMovement(delta);

		/*Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();*/
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
