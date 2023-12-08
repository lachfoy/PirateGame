using Godot;
using System;

public partial class FPSController : CharacterBody3D
{
	[Export]
	public float Speed = 10f;
	
	private Camera3D _camera;
	
	public override void _Ready()
	{
		_camera = GetNode<Camera3D>("Camera3D");
		//Input.SetMouseMode(Input.MouseMode.Captured);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = Vector3.Zero;
		//if (Input.IsActionPressed("move_forward"))
			//direction.z -= 1;
		//if (Input.IsActionPressed("move_backward"))
			//direction.z += 1;
		//if (Input.IsActionPressed("move_left"))
			//direction.x -= 1;
		//if (Input.IsActionPressed("move_right"))
			//direction.x += 1;

		direction = direction.Normalized();
		MoveAndSlide();
	}
	
	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseMotion eventMouseMotion)
		{
			GD.Print("Your message here");
		}
	}
}
