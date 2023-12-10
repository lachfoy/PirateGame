using Godot;

public partial class NotificationList : VBoxContainer
{	
	private void _on_player_notification(string notificationString)
	{
		NotificationLabel label = new NotificationLabel();
		label.Text = notificationString;
		AddChild(label);
	}
}
