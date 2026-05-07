using Godot;

public partial class TAOfficeView : CanvasLayer
{
	[Signal]
	public delegate void SceneClosedEventHandler();

	public override void _Ready()
	{
		var backButton = GetNodeOrNull<Button>("Panel/BackButton");
		if (backButton != null)
			backButton.Pressed += OnBackPressed;
	}

	private void OnBackPressed()
	{
		EmitSignal(SignalName.SceneClosed);
		QueueFree();
	}
}
