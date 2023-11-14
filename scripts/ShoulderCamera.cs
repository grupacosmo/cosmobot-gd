using Godot;
using System;
using Cosmobot;

public partial class ShoulderCamera : Camera3D
{
	[Export] public Vector3 Offset = new Vector3(1, 1, 2);
	[Export] public float CameraSensitivity = 1.0f;
	[Export] public float MaxVerticalAngleDegrees = 75f;

	private float rotation_x;
	private float rotation_y;
	private Vector2 mouseDelta = Vector2.Zero;
	
	private float MaxVerticalAngle;
	private Player player;
	private RayCast3D clipPreventionRay;


	public override void _Ready()
	{
		player = (Player)GetParent();
		clipPreventionRay = (RayCast3D)GetNode("ClipPreventionRay");

        clipPreventionRay.TargetPosition = Offset;
        MaxVerticalAngle = Mathf.DegToRad(MaxVerticalAngleDegrees);

		// Lock and hide the cursor
		Input.MouseMode = Input.MouseModeEnum.Captured;
    }

	public override void _Process(double delta)
	{
        // Set camera rotation
        GlobalRotation = new Vector3(rotation_x, rotation_y, 0);

        // Prevents camera from clipping into surroundings
        clipPreventionRay.GlobalPosition = player.GlobalPosition;

        clipPreventionRay.ForceRaycastUpdate();

        // Calculate & set camera position
        if (clipPreventionRay.IsColliding())  // There's an object in the way
        {
            // (add a little extra safe distance from wall so the camera view won't partially clip inside)
            GlobalPosition = clipPreventionRay.GetCollisionPoint() - Position.Normalized() * 0.1f;
        }
        else // Can put camera at normal position
        {
            Vector3 effectiveOffset = Offset;
            effectiveOffset = effectiveOffset.Rotated(Vector3.Right, rotation_x);
            effectiveOffset = effectiveOffset.Rotated(Vector3.Up, rotation_y);
            GlobalPosition = player.GlobalPosition + effectiveOffset;
        }

        
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
		{
			// Get mouse input and calculate new rotation
			mouseDelta = ((InputEventMouseMotion)@event).Relative;
            rotation_y -= mouseDelta.X * CameraSensitivity;
            rotation_x -= mouseDelta.Y * CameraSensitivity;
			rotation_x = Mathf.Clamp(rotation_x, -MaxVerticalAngle, MaxVerticalAngle);

            // Reset mouse delta to zero because with no mouse movement _Input won't be called
            mouseDelta = Vector2.Zero;
        }
    }
}
