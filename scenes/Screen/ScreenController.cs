using Godot;

public partial class ScreenController : Node2D
{
	private CharacterAttributes attributes;

	public override void _Ready()
	{
		attributes = GlobalVars.Instance.PlayerAttributes;

		ConnectAction("Button/ThisPCButton", attributes.StudyCs, "Study CS");
		ConnectAction("Button/FileButton", attributes.CodingPractice, "Coding Practice");
		ConnectAction("Button/BrowserButton", attributes.Sleep, "Sleep");
		ConnectAction("Button/EmailButton", attributes.Procrastinate, "Procrastinate");

		GD.Print("Screen actions ready.");
		attributes.PrintStats("Initial Stats");
	}

	private void ConnectAction(string nodePath, System.Action action, string actionName)
	{
		var button = GetNodeOrNull<TextureButton>(nodePath);
		if (button == null)
		{
			GD.PrintErr($"Missing action button: {nodePath}");
			return;
		}

		button.Pressed += () =>
		{
			action();
			GD.Print($"Triggered from UI: {actionName}");
		};
	}
}
