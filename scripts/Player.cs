using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public float _speed = 8.0f;
	public float _mouseSens = 0.3f;
	private Camera3D _camera;
	public AnimationPlayer anim;
	
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
		anim = GetNode<AnimationPlayer>("CanvasLayer/Control/Sprite2D/AnimationPlayer");
	}

	public override void _Process(double delta)
	{
		// Easy quick way to quit the game
		if (Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().Quit();
		}
		if (Input.IsActionPressed("Primary_Fire"))
		{
			anim.Play("fire");
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
			_camera.RotateX(Mathf.DegToRad(-eventMouseMotion.Relative.Y * _mouseSens));
		}
	}
	
	private void _on_area_3d_area_entered(Area3D area)
	{
		// Replace with function body.
		GD.Print("Weapon pickup");
		
		Pickup pickup = area as Pickup;
		if (pickup != null)
		{
			GD.Print(String.Format("Picked up weapon of type {0}", pickup.WeaponType.ToString()));
			
			// If we don't already have that weapon then we add it 
			
			
			pickup.QueueFree(); // Delete the pickup
		}
	}
}


