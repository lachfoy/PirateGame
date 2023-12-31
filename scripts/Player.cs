using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class Player : CharacterBody3D
{
	private bool _active = true;

	private float _speed = 6.0f;
	private float _strafeSpeed = 5.0f;
    private float _mouseSens = 0.3f;
	private Camera3D _camera;
	private Vector3 _cameraOrigin;

	private AnimationPlayer _animationPlayer;
	private Sprite2D _weaponSprite;
	private Vector2 _weaponSpriteOrigin;

	private Weapon[] _weapons; // List of weapons
	private int _currentWeaponIndex = -1;
	private bool _canShoot = true;

	private float _gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();
	private float _jumpSpeed = 3.0f;

    private PackedScene _bulletDecal = GD.Load<PackedScene>("res://Entities/BulletDecal.tscn");

	private RandomNumberGenerator _rng;

    [Export]
	public Label DebugLabel;

	// Camera bob
	private float _bobAmount = 0.8f;
	private float _maxSpeed = 6.0f;
	private float _bobSpeed = 4.0f;

    [Signal]
	public delegate void NotificationEventHandler(string notificationString);

	[Signal]
	public delegate void WeaponChangedEventHandler(int newIndex, int ammo); // Maybe this should be weapon type instead of index

	[Signal]
	public delegate void GunShotEventHandler(int damage, int id);

	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		_cameraOrigin = _camera.Transform.Origin; // Save the initial camera LOCAL origin as a reference point.

        Input.MouseMode = Input.MouseModeEnum.Captured;
		_animationPlayer = GetNode<AnimationPlayer>("CanvasLayer/Control/WeaponSpriteControl/Sprite2D/AnimationPlayer");
		_weaponSprite = GetNode<Sprite2D>("CanvasLayer/Control/WeaponSpriteControl/Sprite2D");

        _weaponSpriteOrigin = _weaponSprite.Transform.Origin; // save the initial origin

		// Initialize the weapons list
		_weapons = new Weapon[(int)Weapon.EWeaponType.WeaponTypeCount];

		_rng = new RandomNumberGenerator();
	}

	public override void _Process(double delta)
	{		
		// Easy quick way to quit the game
		if (Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}

		if (!_active) return;

		if (Input.IsActionJustPressed("primary_fire"))
		{
			// Shoot current weapon
			if (_currentWeaponIndex != -1)
			{
				if (_weapons[_currentWeaponIndex].Ammo > 0 && _canShoot)
				{
					// TODO : refactor into some bite sized functions
					// Get the physics state
                    PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;

					float randomOffsetAmount = 0.1f; // TODO : get this from the weapon
					int numberOfShots = 10; // TODO : get this from the weapon
					for (int i = 0; i < numberOfShots; i++)
					{
						// Calculate a random offset
                        Vector3 randomOffset = new Vector3(
                            _rng.Randf() * 2.0f - 1.0f,
                            _rng.Randf() * 2.0f - 1.0f,
                            _rng.Randf() * 2.0f - 1.0f
						) * randomOffsetAmount;

                        // Make ray query to the physics server
                        Vector3 rayOrigin = _camera.GlobalTransform.Origin;
						Vector3 rayDirection = -_camera.GlobalBasis.Z;
						rayDirection += randomOffset;
						rayDirection = rayDirection.Normalized() * 1000.0f;

                        PhysicsRayQueryParameters3D rayQuery = PhysicsRayQueryParameters3D.Create(
                            rayOrigin,
                            rayOrigin + rayDirection,
                            0b00000000_00000000_00000000_00000001 // Collision mask 1
                        );

						Dictionary rayResult = spaceState.IntersectRay(rayQuery);

						// If the result is not empty, then we hit something
						if (rayResult.Count > 0) // For some reason result.IsEmpty doesn't exist, when according the documentation it should...
						{
							GodotObject target = rayResult["collider"].AsGodotObject();
							EmitSignal(SignalName.GunShot, _weapons[_currentWeaponIndex].Damage, target.GetInstanceId());

							string notificationString = string.Format("Fire a shot for {0} Damage! Hit {1}", _weapons[_currentWeaponIndex].Damage, target.GetInstanceId());
							EmitSignal(SignalName.Notification, notificationString);

							Vector3 collisionPoint = rayResult["position"].AsVector3();
							Vector3 collisionNormal = rayResult["normal"].AsVector3();
                        
							// Bullet decal
							var bulletDecal = _bulletDecal.Instantiate() as BulletDecal;
							GetTree().Root.AddChild(bulletDecal);

							bulletDecal.GlobalPosition = collisionPoint;

							Vector3 up = new Vector3(0, 1, 0);
							if (Mathf.Abs(collisionNormal.Dot(up)) > 0.99f)
							{
								up = new Vector3(1, 0, 0);
							}

							bulletDecal.LookAt(collisionPoint + collisionNormal, up, true);
						}
					}

					_animationPlayer.Play("fire");
					_canShoot = false;

					_weapons[_currentWeaponIndex].Ammo--;
					EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
				}
			}
		}

		// REPLACE WITH INPUT ACTIONS
		if (Input.IsPhysicalKeyPressed(Key.Key1) && _weapons[(int)Weapon.EWeaponType.Slingshot] != null)
		{
			_currentWeaponIndex = (int)Weapon.EWeaponType.Slingshot;
			EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
		}

		if (Input.IsPhysicalKeyPressed(Key.Key2) && _weapons[(int)Weapon.EWeaponType.Pistol] != null)
		{
			_currentWeaponIndex = (int)Weapon.EWeaponType.Pistol;
			EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
		}

		if (Input.IsPhysicalKeyPressed(Key.Key3) && _weapons[(int)Weapon.EWeaponType.Blunderbuss] != null)
		{
			_currentWeaponIndex = (int)Weapon.EWeaponType.Blunderbuss;
			EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
		}

		if (Input.IsPhysicalKeyPressed(Key.Key4) && _weapons[(int)Weapon.EWeaponType.Rifle] != null)
		{
			_currentWeaponIndex = (int)Weapon.EWeaponType.Rifle;
			EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!_active) return;

		Vector3 velocity = Velocity;

        velocity.Y -= _gravity * (float)delta;
		
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = _jumpSpeed;
        }

		Vector2 inputDir = Input.GetVector("strafe_left", "strafe_right", "walk_forward", "walk_backward");
		inputDir.X *= _strafeSpeed;
		inputDir.Y *= _speed;

		Vector3 direction = Transform.Basis * new Vector3(inputDir.X, 0.0f, inputDir.Y);

		velocity.X = direction.X;
		velocity.Z = direction.Z;

        float normalizedSpeed = Mathf.Remap(Velocity.Length(), 0.0f, _maxSpeed, 0.0f, 1.0f);

        float bobTime = (Time.GetTicksMsec() / 1000.0f) * _bobSpeed;

        float bobX = Mathf.Sin(bobTime * 2.0f) * _bobAmount;
        float bobY = Mathf.Cos(bobTime) * _bobAmount;

        if (normalizedSpeed <= 0.0f || !IsOnFloor())
        {
            bobX = 0.0f;
            bobY = 0.0f;
        }

		// There are some magic numbers here . Please ignore.
        Vector3 cameraPos = _camera.Transform.Origin;
        cameraPos.X = Mathf.Lerp(cameraPos.X, _cameraOrigin.X + bobY, (float)delta);
        cameraPos.Y = Mathf.Lerp(cameraPos.Y, _cameraOrigin.Y + bobX, (float)delta);
        //_camera.Transform = new Transform3D(_camera.Basis, cameraPos); // commented out camera bob for now

        Vector2 weaponSpriteOrigin = _weaponSprite.Transform.Origin;
        weaponSpriteOrigin.X = Mathf.Lerp(weaponSpriteOrigin.X, _weaponSpriteOrigin.X + bobY * 100.0f, (float)delta);
        weaponSpriteOrigin.Y = Mathf.Lerp(weaponSpriteOrigin.Y, _weaponSpriteOrigin.Y + bobX * 100.0f, (float)delta);
        _weaponSprite.Transform = new Transform2D(0.0f, weaponSpriteOrigin);

        Velocity = velocity;
		MoveAndSlide();

        DebugLabel.Text = string.Format("velocity = {0}\nnormalized speed = {1}\nbob target = {2}, {3}", velocity.ToString(), normalizedSpeed, bobX, bobY);
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
			_camera.RotationDegrees = new Vector3(newAngle, _camera.RotationDegrees.Y, _camera.RotationDegrees.Z);
		}
	}

	private void _on_area_3d_area_entered(Area3D area)
	{
		// Consider reversing this dependency?
		Pickup pickup = area as Pickup;
		if (pickup.Weapon != null)
		{
			// If we don't already have that weapon then we add it
			int weaponPickupIndex = (int)pickup.Weapon.WeaponType;
			if (_weapons[weaponPickupIndex] == null)
			{
				_weapons[weaponPickupIndex] = pickup.Weapon;

				_currentWeaponIndex = weaponPickupIndex;
				EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal

				string notificationString = string.Format("Picked up {0}", _weapons[weaponPickupIndex].WeaponName);
				EmitSignal(SignalName.Notification, notificationString); // Emit a notifaction signal

                GD.Print(notificationString);
            }
            else // Otherwise add ammo to the weapon
			{
				_weapons[weaponPickupIndex].Ammo += pickup.Weapon.Ammo;
				EmitSignal(SignalName.WeaponChanged, _currentWeaponIndex, _weapons[_currentWeaponIndex].Ammo); // Emit a weapon changed signal

				string notificationString = string.Format("Picked up {0} Ammo for {1}", pickup.Weapon.Ammo, _weapons[weaponPickupIndex].WeaponName);
				EmitSignal(SignalName.Notification, notificationString); // Emit a notifaction signal

				GD.Print(notificationString);
			}

			pickup.QueueFree(); // Delete the pickup
		}
	}
	
	private void _on_animation_player_animation_finished(StringName anim_name)
	{
		if (anim_name == "fire")
		{
			_canShoot = true;
			GD.Print("Can Shoot");
		}
	}

	private void _on_gatlin_gun_mounting(string notificationString, Vector3 gunPos)
	{
		_weaponSprite.Visible = false;
		GlobalPosition = gunPos;
		if (notificationString == "Player")
		{
			_active = false;
		}
	}
	
	private void _on_gatlin_gun_unmounting(string notificationString, Vector3 gunPos)
	{
		_weaponSprite.Visible = true;
		Translate(Vector3.Back);
		if (notificationString == "Player")
		{
			_active = true;
		}
	}
}



