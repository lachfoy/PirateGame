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
		anim = GetNode<AnimationPlayer>("AnimationPlayer");
		anim.Play("fire");
	}

	public override void _Process(double delta)
	{
		// Easy quick way to quit the game
		if (Input.IsPhysicalKeyPressed(Key.Escape))
		{
			GetTree().Quit();
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
}
