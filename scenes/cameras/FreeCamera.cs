using Godot;
using System;

public partial class FreeCamera : CharacterBody3D
{
    public enum CoordinateMode
    {
        GLOBAL,
        LOCAL
    }
	[Export] public float moveSpeed { get; private set; } = 7.0f;
    [Export] private float _maxVerticalAngleDegrees = 75f;
    public float MaxVerticalAngle
    {
        get => Mathf.DegToRad(_maxVerticalAngleDegrees);
        private set => _maxVerticalAngleDegrees = Mathf.RadToDeg(value);
    }
    [Export] public float LookSensitivity { get; private set; } = 0.005f;
    [Export] public CoordinateMode horizontalInputMode { get; private set; } = CoordinateMode.GLOBAL;
    [Export] public CoordinateMode verticalInputMode { get; private set; } = CoordinateMode.GLOBAL;

    public bool isActive = false; // TEMP

    private float _rotationX;
    private float _rotationY;

    private Camera3D _camera;

    public override void _Ready()
	{
        _camera = (Camera3D)GetNode("Camera3D");
	}

	
	public override void _Process(double delta)
	{
        ProcessMovement((float)delta);

        // TEMP
        if (Input.IsActionJustPressed("switch_camera_mode"))
        {
            isActive = !isActive;
            _camera.Current =  isActive;
        }
	}

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion)
        {
            Vector2 _mouseDelta = ((InputEventMouseMotion)@event).Relative;
            _rotationY -= _mouseDelta.X * LookSensitivity;
            _rotationX -= _mouseDelta.Y * LookSensitivity;
            _rotationX = Mathf.Clamp(_rotationX, -MaxVerticalAngle, MaxVerticalAngle);
        }
    }

    private void ProcessMovement(float delta)
    {
        if (isActive) // TEMP check if freecam is selected
        {
            GlobalRotation = new(_rotationX, _rotationY, 0);

            Vector3 inputVector = new();

            float inputAxisX = Input.GetAxis("move_left", "move_right");
            inputVector += Transform.Basis.X * inputAxisX;

            float inputAxisZ = Input.GetAxis("move_up", "move_down");
            if (horizontalInputMode == CoordinateMode.LOCAL)
            {
                inputVector += Transform.Basis.Z * inputAxisZ;
            }
            else
            {
                inputVector += Vector3.Forward.Rotated(Vector3.Up, Rotation.Y).Normalized() * -inputAxisZ;
            }

            float inputAxisY = Input.GetAxis("free_camera_down", "free_camera_up");
            if (verticalInputMode == CoordinateMode.LOCAL)
            {
                inputVector += Transform.Basis.Y * inputAxisY;
            }
            else
            {
                inputVector += Vector3.Up * inputAxisY;
            }

            inputVector = inputVector.Normalized();
            Velocity = inputVector * moveSpeed;
            MoveAndSlide();
        }
    }
        
}
