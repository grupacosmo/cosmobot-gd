using Godot;
using System;
using Cosmobot;

public partial class ShoulderCamera : Camera3D
{
    [Export] public Vector3 Offset { get; private set; } = new(1, 1, 2);
    [Export] public float LookSensitivity { get; private set; } = 0.005f;

    [Export] private float _maxVerticalAngleDegrees = 75f;
    public float MaxVerticalAngle
    {
        get => Mathf.DegToRad(_maxVerticalAngleDegrees);
        private set => _maxVerticalAngleDegrees = Mathf.RadToDeg(value);
    }

    private float _rotationX;
    private float _rotationY;

    private Player _player;
    private RayCast3D _clipPreventionRay;

    public override void _Ready()
    {
        _player = (Player)GetParent();
        _clipPreventionRay = (RayCast3D)GetNode("ClipPreventionRay");

        _clipPreventionRay.TargetPosition = Offset;

        // Lock and hide the cursor
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _Process(double delta)
    {
        GlobalRotation = new(_rotationX, _rotationY, 0);

        _clipPreventionRay.GlobalPosition = _player.GlobalPosition;
        _clipPreventionRay.ForceRaycastUpdate();
        if (_clipPreventionRay.IsColliding()) // There's an object in the way
        {
            // (add a little extra safe distance from wall so the camera view won't partially clip inside)
            GlobalPosition = _clipPreventionRay.GetCollisionPoint() - Position.Normalized() * 0.1f;
        }
        else // Can put camera at normal position
        {
            Vector3 effectiveOffset = Offset;
            effectiveOffset = effectiveOffset.Rotated(Vector3.Right, _rotationX);
            effectiveOffset = effectiveOffset.Rotated(Vector3.Up, _rotationY);
            GlobalPosition = _player.GlobalPosition + effectiveOffset;
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
}
