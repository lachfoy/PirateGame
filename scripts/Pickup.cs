using Godot;

public partial class Pickup : Area3D
{
	[Export]
	public Weapon Weapon;

    private Sprite3D _pickupSprite;

    public override void _Ready()
	{
        if (Weapon == null)
        {
            GD.PushWarning("A pickup must have a weapon resource set!!");
        }

        _pickupSprite = GetNode<Sprite3D>("Sprite3D");
        _pickupSprite.Texture = Weapon.PickupSprite;
    }
}
