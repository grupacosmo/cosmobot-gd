using Godot;
using System;

public partial class ShoulderCamera : Camera3D
{

	[Export] public Vector3 Offset = new Vector3(1, 1, 2);
	[Export] public float CameraSensitivity = 1.0f;
	[Export] public float MaxVerticalAngleDegrees = 90f;

	private float rotation_x;
	private float rotation_y;

	private float MaxVerticalAngle;

	private Player player;
	private Vector2 mouseDelta = Vector2.Zero;

	public override void _Ready()
	{
		player = (Player)GetParent();
		MaxVerticalAngle = Mathf.DegToRad(MaxVerticalAngleDegrees);

		Input.MouseMode = Input.MouseModeEnum.Captured;
    }

	public override void _Process(double delta)
	{

		GlobalRotation = new Vector3(rotation_x, rotation_y, 0);
		GlobalPosition = player.GlobalPosition + Offset.Rotated(Vector3.Right, rotation_x).Rotated(Vector3.Up, rotation_y);

		// Reset mouse delta to zero because it won't change without an input signal
        mouseDelta = Vector2.Zero;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
		{
			mouseDelta = ((InputEventMouseMotion)@event).Relative;
            rotation_x -= mouseDelta.Y * CameraSensitivity;
			rotation_x = Mathf.Clamp(rotation_x, -MaxVerticalAngle, MaxVerticalAngle);
            rotation_y -= mouseDelta.X * CameraSensitivity;
        }
    }
}
