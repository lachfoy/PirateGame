using Godot;
using System;

public partial class Enemy : Node3D
{
	private int _health = 120;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _Die()
	{
		//Maybe do some more stuff here like change sprite instead of just deleting
		QueueFree();
	}
	
	private void _on_player_gun_shot(long damage, ulong id)
	{
		if (GetNode<RigidBody3D>("RigidBody3D").GetInstanceId() == id)
		{
			_health -= (int)damage;
			if (_health <= 0)
				_Die();
		}
	}
}


