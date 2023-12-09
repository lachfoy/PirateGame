using Godot;

public partial class AmmoCounter : Label
{	
	private void _on_player_weapon_changed(long newIndex, long ammo)
	{
		Text = "Ammo: " + ammo.ToString();
	}
}
