using Godot;
using System;

public partial class Spawner : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PackedScene scene = GD.Load<PackedScene>("res://Entities//Enemy.tscn");
		Enemy e = scene.Instantiate<Enemy>();
		e.Scale = new Vector3(3, 3, 3);
		CallDeferred("add_child", e);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
