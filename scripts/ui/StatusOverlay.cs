using Godot;

public partial class StatusOverlay : CanvasLayer
{
	private static StatusOverlay _instance;
	private Label _label;
	private Control _floatingRoot;
	private int _floatingIndex;

	public override void _Ready()
	{
		_instance = this;
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

		_floatingRoot = new Control();
		_floatingRoot.AnchorLeft = 1;
		_floatingRoot.AnchorTop = 1;
		_floatingRoot.AnchorRight = 1;
		_floatingRoot.AnchorBottom = 1;
		_floatingRoot.OffsetLeft = -300;
		_floatingRoot.OffsetTop = -260;
		_floatingRoot.OffsetRight = -20;
		_floatingRoot.OffsetBottom = -20;
		AddChild(_floatingRoot);
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
				$"Career Readiness {s.Profile.CareerReadiness}\n" +
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

	public override void _ExitTree()
	{
		if (_instance == this)
			_instance = null;
	}

	public static void NotifyStatChange(string statName, int delta)
	{
		_instance?.SpawnFloatingText(statName, delta);
	}

	private void SpawnFloatingText(string statName, int delta)
	{
		if (_floatingRoot == null || delta == 0)
			return;

		var pop = new Label();
		pop.Text = $"{(delta > 0 ? "+" : "")}{delta} {statName}";
		pop.Modulate = delta > 0 ? new Color(0.6f, 1f, 0.6f, 1f) : new Color(1f, 0.6f, 0.6f, 1f);
		pop.Position = new Vector2(0, -(_floatingIndex % 6) * 18);
		_floatingIndex++;
		_floatingRoot.AddChild(pop);

		var tween = CreateTween();
		tween.SetParallel(true);
		tween.TweenProperty(pop, "position", pop.Position + new Vector2(80, -60), 1.0f);
		tween.TweenProperty(pop, "modulate:a", 0.0f, 1.0f);
		tween.Finished += () => pop.QueueFree();
	}
}
