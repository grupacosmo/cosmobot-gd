using Godot;
using System;
using Cosmobot;

namespace Cosmobot
{

    public partial class Player : CharacterBody3D
    {
        [ExportCategory("Movement")]
        [Export] public float WalkSpeed = 5.0f;
        [Export] public float SprintSpeed = 8.0f;
        [Export] public float Acceleration = 60.0f;
        [Export] public float AirAcceleration = 15.0f;
        [Export] public float JumpForce = 4.5f;
        [Export] public float GravityMultiplier = 1.0f;

        [ExportCategory("Jetpack")]
        [Export] public float JetpackForce = 3.0f;
        [Export] public float JetpackMaxFuel = 1.5f;
        [Export] public float JetpackFuelRegenSpeed = 4.0f;

        private float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
        private ShoulderCamera shoulderCamera;

        private bool sprinting = false;
        private bool jetpackOn = false;
        private float jetpackFuel = 0f;
        private float currentSpeed;
        private float currentAcceleration;
        
        public override void _Ready()
        {
            shoulderCamera = (ShoulderCamera)GetNode("ShoulderCamera");
            gravity *= GravityMultiplier;
        }

        public override void _Process(double delta)
        {
            // TEMP
            // feature for quickly closing the game when testing
            if (Input.IsActionJustPressed("quit_temp"))
            {
                GetTree().Quit();
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            ProcessMovement(delta);
        }

        #region ProcessMovement
        private void ProcessMovement(double delta)
        {
            Vector3 newVelocity = Velocity;

            // ### Horizontal movement

            // Get input
            Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_up", "move_down");
            Vector2 localInputDir = inputDir.Rotated(-shoulderCamera.Rotation.Y);
            Vector3 directionH = (Transform.Basis * new Vector3(localInputDir.X, 0, localInputDir.Y)).Normalized();

            // Calculate max speed
            sprinting = Input.IsActionPressed("sprint");
            currentSpeed = sprinting ? SprintSpeed : WalkSpeed;
            currentAcceleration = IsOnFloor() ? Acceleration : AirAcceleration;

            // Calculate velocity
            Vector3 targetVelocityH = directionH * currentSpeed;
            newVelocity.X = (float)Mathf.MoveToward(Velocity.X, targetVelocityH.X, currentAcceleration * delta);
            newVelocity.Z = (float)Mathf.MoveToward(Velocity.Z, targetVelocityH.Z, currentAcceleration * delta);

            // ### Vertical movement
            newVelocity.Y -= gravity * (float)delta;

            // Check for jump/jetpack input
            if (Input.IsActionJustPressed("jump"))
            {
                if (IsOnFloor()) // Jump
                {
                    newVelocity.Y = JumpForce;
                }
                else // Enable jetpack
                {
                    jetpackOn = true;
                }

            }
            // Process jetpack
            if (jetpackOn)
            {
                newVelocity.Y = Mathf.Max(newVelocity.Y, JetpackForce);
                jetpackFuel -= (float)delta;

                // Turn off jetpack if conditions met
                if (!Input.IsActionPressed("jump") || jetpackFuel <= 0f)
                {
                    jetpackOn = false;
                }
            }
            // Jetpack fuel regen when on floor
            if (IsOnFloor() && jetpackOn == false)
            {
                jetpackFuel += JetpackFuelRegenSpeed * (float)delta;
                jetpackFuel = Mathf.Clamp(jetpackFuel, 0, JetpackMaxFuel);
            }

            // ### Finalize
            Velocity = newVelocity;
            MoveAndSlide();
        }
    }
    #endregion

}
