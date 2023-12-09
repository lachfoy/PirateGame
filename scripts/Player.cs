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
	private bool _canShoot = true;
	
	[Signal]
	public delegate void PickupNotificationEventHandler(string weaponName);

	[Signal]
	public delegate void WeaponChangedEventHandler(int newIndex, int ammo); // Maybe this should be weapon type instead of index

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/Control/Sprite2D/AnimationPlayer");

		// Initialize all the weapons
		_weapons = new Weapon[(int)EWeaponType.WeaponTypeCount];
		_weapons[(int)EWeaponType.Slingshot] = new Slingshot();
		_weapons[(int)EWeaponType.Pistol] = new Pistol();
		_weapons[(int)EWeaponType.Blunderbuss] = new Weapon();
		_weapons[(int)EWeaponType.Rifle] = new Weapon();
	}

	public override void _Process(double delta)
	{
		// Easy quick way to quit the game
		if (Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}

		if (Input.IsActionJustPressed("primary_fire"))
		{
			// Shoot current weapon
			if (_currentWeaponIndex != -1)
			{
				if (_weapons[_currentWeaponIndex].Ammo > 0 && _canShoot)
				{
					_weapons[_currentWeaponIndex].Shoot();
					
					_animationPlayer.Play("fire");
					_canShoot = false;
					
					_weapons[_currentWeaponIndex].Ammo--;
					EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
				}
			}
		}

		// REPLACE WITH INPUT ACTIONS
		if (Input.IsPhysicalKeyPressed(Key.Key1) && _weapons[(int)EWeaponType.Slingshot].HasThisWeapon)
		{
			_currentWeaponIndex = (int)EWeaponType.Slingshot;
			EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
		}

		if (Input.IsPhysicalKeyPressed(Key.Key2) && _weapons[(int)EWeaponType.Pistol].HasThisWeapon)
		{
			_currentWeaponIndex = (int)EWeaponType.Pistol;
			EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
		}

		if (Input.IsPhysicalKeyPressed(Key.Key3) && _weapons[(int)EWeaponType.Blunderbuss].HasThisWeapon)
		{
			_currentWeaponIndex = (int)EWeaponType.Slingshot;
			EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
		}

		if (Input.IsPhysicalKeyPressed(Key.Key4) && _weapons[(int)EWeaponType.Rifle].HasThisWeapon)
		{
			_currentWeaponIndex = (int)EWeaponType.Slingshot;
			EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
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

				_currentWeaponIndex = weaponPickupIndex;
				EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal

				EmitSignal(SignalName.PickupNotification, _weapons[weaponPickupIndex].WeaponName); // Emit a pickup notifaction signal
			}
			else // Otherwise add ammo to the weapon
			{
				_weapons[weaponPickupIndex].Ammo += pickup.Ammo;
				EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal

				string notificationString = string.Format("{0} Ammo for {1}", pickup.Ammo, _weapons[weaponPickupIndex].WeaponName);
				EmitSignal(SignalName.PickupNotification, notificationString); // Emit a pickup notifaction signal
			}

			pickup.QueueFree(); // Delete the pickup
		}
	}
	
	private void _on_animation_player_animation_finished(StringName anim_name)
	{
		if (anim_name == "fire")
		{
			_canShoot = true;
		}
	}
}




