using Godot;
using System;

public partial class NotificationList : VBoxContainer
{
	private Tween _tween;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void _on_player_pickup_notification(String weaponName)
	{
		NotificationLabel label = new NotificationLabel();
		label.Text = String.Format("Picked up {0}!", weaponName);
		AddChild(label);
	}
	
	private void _on_player_gun_shot(long damage, ulong id)
	{
		NotificationLabel label = new NotificationLabel();
		label.Text = String.Format("Fire a shot for {0} Damage! Hit {1}", damage, id);
		AddChild(label);
	}
}

