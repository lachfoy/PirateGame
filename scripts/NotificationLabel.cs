using Godot;

public partial class NotificationLabel : Label
{
	public override void _Ready()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 0.0f, 3.0f);
		tween.TweenCallback(Callable.From(QueueFree));
		tween.Play();
	}
}
