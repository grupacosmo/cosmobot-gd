using Godot;
using System;
using Cosmobot;

namespace Cosmobot
{

    public partial class Player : CharacterBody3D
    {
        [ExportCategory("Movement")]
        [Export] public float WalkSpeed { get; private set; } = 5.0f;
        [Export] public float SprintSpeed { get; private set; } = 8.0f;
        [Export] public float Acceleration { get; private set; } = 60.0f;
        [Export] public float AirAcceleration { get; private set; } = 15.0f;
        [Export] public float JumpForce { get; private set; } = 4.5f;
        [Export] public float GravityMultiplier { get; private set; } = 1.0f;

        [ExportCategory("Jetpack")]
        [Export] public float JetpackForce { get; private set; } = 3.0f;
        [Export] public float JetpackMaxFuel { get; private set; } = 1.5f;
        [Export] public float JetpackFuelRegenSpeed { get; private set; } = 4.0f;

        public float JetpackFuel { get; private set; } = 0f;

        public bool isActive = true; // TEMP

        private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
        private ShoulderCamera _shoulderCamera;

        private bool _jetpackActive = false;
        private bool _isSprinting = false;

        private bool CanRefuelJetpack => IsOnFloor() && _jetpackActive == false && isActive;
        private bool JetpackDisableCondition => !Input.IsActionPressed("jump_jetpack") || JetpackFuel <= 0f;

        public override void _Ready()
        {
            _shoulderCamera = (ShoulderCamera)GetNode("ShoulderCamera");
            _gravity *= GravityMultiplier;
        }

        public override void _Process(double delta)
        {
            // TEMP
            // feature for quickly closing the game when testing
            if (Input.IsActionJustPressed("quit_temp"))
            {
                GetTree().Quit();
            }

            // TEMP
            if (Input.IsActionJustPressed("switch_camera_mode"))
            {
                isActive = !isActive;
                _shoulderCamera.Current = isActive;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            ProcessMovement((float)delta);
        }

        #region ProcessMovement
        private void ProcessMovement(float delta)
        {
            ProcessHorizontalMovement((float)delta);
            ProcessVerticalMovement((float)delta);

            MoveAndSlide();
        }

        private void ProcessHorizontalMovement(float delta) // Walking and sprinting
        {
            Vector2 newVelocityH = new(Velocity.X, Velocity.Z);

            Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            inputDir = isActive ? inputDir : Vector2.Zero; // TEMP
            Vector2 localInputDir = inputDir.Rotated(-_shoulderCamera.Rotation.Y);
            _isSprinting = Input.IsActionPressed("sprint");

            float _currentSpeed = _isSprinting ? SprintSpeed : WalkSpeed;
            float _currentAcceleration = IsOnFloor() ? Acceleration : AirAcceleration;

            Vector2 targetVelocityH = localInputDir * _currentSpeed;
            newVelocityH.X = Mathf.MoveToward(newVelocityH.X, targetVelocityH.X, _currentAcceleration * delta);
            newVelocityH.Y = Mathf.MoveToward(newVelocityH.Y, targetVelocityH.Y, _currentAcceleration * delta);

            Velocity = new(newVelocityH.X, Velocity.Y, newVelocityH.Y);
        }

        private void ProcessVerticalMovement(float delta) // Jumping and jetpack
        {
            float newVelocityY = Velocity.Y;

            newVelocityY -= _gravity * delta;

            if (Input.IsActionJustPressed("jump_jetpack") && isActive)
            {
                if (IsOnFloor()) // Jump
                {
                    newVelocityY = JumpForce;
                }
                else // Enable jetpack
                {
                    _jetpackActive = true;
                }

            }

            if (_jetpackActive)
            {
                newVelocityY = Mathf.Max(newVelocityY, JetpackForce);
                JetpackFuel -= delta;

                if (JetpackDisableCondition)
                {
                    _jetpackActive = false;
                }
            }

            if (CanRefuelJetpack)
            {
                JetpackFuel += JetpackFuelRegenSpeed * delta;
                JetpackFuel = Mathf.Clamp(JetpackFuel, 0, JetpackMaxFuel);
            }

            Velocity = new(Velocity.X, newVelocityY, Velocity.Z);
        }

        #endregion
    }


}
