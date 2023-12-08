using Godot;
using System;

public partial class FPSController : CharacterBody3D
{
	[Export]
	public float Speed = 10.0f;

	[Export]
	public float MouseSensitivity = 0.3f;

	private Camera3D _camera;
	
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		Input.MouseMode = Input.MouseModeEnum.Captured;
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
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

		Velocity = direction * Speed;

		MoveAndSlide();
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			GD.Print("I collided with ", ((Node)collision.GetCollider()).Name);
		}
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			RotateY(Mathf.DegToRad(-eventMouseMotion.Relative.X * MouseSensitivity));
			_camera.RotateX(Mathf.DegToRad(-eventMouseMotion.Relative.Y * MouseSensitivity));
        }
	}
}
