using Godot;

public partial class StatusOverlay : CanvasLayer
{
	private Label _label;

	public override void _Ready()
	{
		var panel = new Panel();
		panel.AnchorLeft = 1;
		panel.AnchorTop = 1;
		panel.AnchorRight = 1;
		panel.AnchorBottom = 1;
		panel.OffsetLeft = -290;
		panel.OffsetTop = -230;
		panel.OffsetRight = -16;
		panel.OffsetBottom = -16;
		AddChild(panel);

		_label = new Label();
		_label.OffsetLeft = 10;
		_label.OffsetTop = 10;
		_label.OffsetRight = 260;
		_label.OffsetBottom = 200;
		_label.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		panel.AddChild(_label);
	}

	public override void _Process(double delta)
	{
		if (_label == null || GlobalVars.Instance == null)
			return;

		var s = GlobalVars.Instance;
		_label.Text =
			$"Day {s.Profile.CurrentDay} / {s.Profile.FinalDay}\n" +
			$"Actions {s.Profile.ActionsLeft} / {s.Profile.MaxActionsPerDay}\n" +
			$"Energy {s.Attributes.Energy}\n" +
			$"Focus {s.Attributes.Focus}\n" +
			$"Knowledge {s.Attributes.Knowledge}\n" +
			$"Confidence {s.Attributes.Confidence}\n" +
			$"Career {s.Profile.CareerReadiness}\n" +
			$"Networking {s.Attributes.Networking}\n" +
			$"Portfolio {s.Attributes.Portfolio}";
	}

	public static void AttachTo(Node parent)
	{
		if (parent.GetNodeOrNull<StatusOverlay>("StatusOverlay") != null)
			return;
		var overlay = new StatusOverlay { Name = "StatusOverlay" };
		parent.AddChild(overlay);
	}
}
