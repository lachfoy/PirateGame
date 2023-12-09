using Godot;

public partial class Pickup : Area3D
{
	[Export]
	public EWeaponType WeaponType = EWeaponType.Slingshot;
	
	[Export]
	public int Ammo = 10;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	//// Called when the player collides with this Area3D
	//private void _on_body_entered(Node3D body)
	//{
	//	GD.Print("Heasdasdllo");
	//}
}


