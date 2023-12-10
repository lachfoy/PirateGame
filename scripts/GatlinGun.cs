using Godot;
using System;

public partial class GatlinGun : Node3D
{
	private bool _canMount;
	private bool _isMount;
	private Label _label;
	private Sprite3D _sprite;
	private Sprite2D _uiSprite;
	private Node3D _player;
	// Called when the node enters the scene tree for the first time.

	[Signal]
	public delegate void MountingEventHandler(string notificationString, Vector3 gunPos);

	[Signal]
	public delegate void UnmountingEventHandler(string notificationString, Vector3 gunPos);
	public override void _Ready()
	{
		_label = GetNode<Label>("CanvasLayer/Control/Label");
		_sprite = GetNode<Sprite3D>("StaticBody3D/Sprite3D");
		_uiSprite = GetNode<Sprite2D>("CanvasLayer/Control2/Sprite2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("interact"))
		{
			if (_isMount)
			{
				//Unmount Player
				_label.Visible = true;
				_sprite.Visible = true;
				_uiSprite.Visible = false;
				_isMount = false;
				EmitSignal(SignalName.Unmounting, _player.Name, Transform.Origin);
			}
			else if (_canMount)
			{
				//Mount Player
				_label.Visible = false;
				_sprite.Visible = false;
				_isMount = true;
				_uiSprite.Visible = true;
				EmitSignal(SignalName.Mounting, _player.Name, Transform.Origin);
			}
		}
	}
	private void _on_area_3d_body_entered(Node3D body)
	{
		if (body.Name == "Player")
		{
			_label.Visible = true;
			_canMount = true;
			_player = body;
		}
	}
	
	private void _on_area_3d_body_exited(Node3D body)
	{
		if (body.Name == "Player")
		{
			_label.Visible = false;
			_canMount = false;
			_player = null;
		}
	}
}

