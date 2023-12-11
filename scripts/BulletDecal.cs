using Godot;


public partial class BulletDecal : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        GD.Print("Create bullet decal");

        GetTree().CreateTimer(1.0f).Timeout += OnTimeout;
		
    }

	private void OnTimeout()
	{
		GD.Print("Destroy bullet decal");
		QueueFree();
	}
}
