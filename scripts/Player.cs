using Godot;
using System;
using System.Collections.Generic;

public partial class Player : CharacterBody3D
{
	public float _speed = 8.0f;
	public float _mouseSens = 0.3f;
	private Camera3D _camera;
	private AnimationPlayer _animationPlayer;

	private Weapon[] _weapons; // List of weapons
	private int _currentWeaponIndex = -1;
	
	Label _debugLabel; // REMOVE THIS
	
	[Signal]
	public delegate void PickupNotificationEventHandler(String weaponName);
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/Control/Sprite2D/AnimationPlayer");

		_weapons = new Weapon[(int)EWeaponType.WeaponTypeCount];
		_weapons[(int)EWeaponType.Slingshot] = new Slingshot();
		_weapons[(int)EWeaponType.Pistol] = new Weapon();
		_weapons[(int)EWeaponType.Blunderbuss] = new Weapon();
		_weapons[(int)EWeaponType.Rifle] = new Weapon();


		_debugLabel = GetNode<Label>("CanvasLayer/Control/DebugLabel");
	}

	public override void _Process(double delta)
	{
		// Easy quick way to quit the game
		if (Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
		if (Input.IsActionPressed("primary_fire"))
		{
			_animationPlayer.Play("fire");
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDir = Input.GetVector("strafe_left", "strafe_right", "walk_forward", "walk_backward");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, -1, inputDir.Y)).Normalized();

		Velocity = direction * _speed;

		MoveAndSlide();
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			//GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateY(Mathf.DegToRad(-eventMouseMotion.Relative.X * _mouseSens));

			// Calculate the new rotation angle
			float newAngle = _camera.RotationDegrees.X + (-eventMouseMotion.Relative.Y * _mouseSens);

			// Clamp the new angle to the range [-70, 70]
			newAngle = Mathf.Clamp(newAngle, -70, 70);

			// Apply the clamped angle
			_camera.RotationDegrees = new Vector3(newAngle, _camera.RotationDegrees.Y, _camera.RotationDegrees.Z);
		}
	}

	private void _on_area_3d_area_entered(Area3D area)
	{
		// Replace with function body.
		GD.Print("Weapon pickup");
		
		Pickup pickup = area as Pickup;
		if (pickup != null)
		{
			// If we don't already have that weapon then we add it
			int weaponPickupIndex = (int)pickup.WeaponType;
			if (!_weapons[weaponPickupIndex].HasThisWeapon)
			{
				_weapons[weaponPickupIndex].HasThisWeapon = true;
				_weapons[weaponPickupIndex].Ammo = pickup.Ammo;

				// DEBUG STRING!
				String debugString = String.Format("Current weapon: {0}, Ammo: {1}", _weapons[weaponPickupIndex].WeaponName, _weapons[weaponPickupIndex].Ammo);
				_debugLabel.Text = debugString;
				
				EmitSignal(SignalName.PickupNotification, _weapons[weaponPickupIndex].WeaponName); // Emit a pickup notifaction signal
			}
			else // Otherwise add ammo to the weapon
			{
				_weapons[weaponPickupIndex].Ammo += pickup.Ammo;

				// DEBUG STRING!
				String debugString = String.Format("Current weapon: {0}, Ammo: {1}", _weapons[weaponPickupIndex].WeaponName, _weapons[weaponPickupIndex].Ammo);
				_debugLabel.Text = debugString;

				String notificationString = String.Format("{0} Ammo for {1}", pickup.Ammo, _weapons[weaponPickupIndex].WeaponName);
				EmitSignal(SignalName.PickupNotification, notificationString); // Emit a pickup notifaction signal
			}

			pickup.QueueFree(); // Delete the pickup
		}
	}
	
}


