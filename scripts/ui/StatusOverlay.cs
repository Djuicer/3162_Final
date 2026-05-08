using Godot;

public partial class StatusOverlay : CanvasLayer
{
	private static readonly PackedScene OverlayScene = ResourceLoader.Load<PackedScene>("res://scenes/UI/StatusOverlay.tscn");
	private static StatusOverlay _instance;

	[Export] public Vector2 PanelTopLeftOffset { get; set; } = new(-290, -230);
	[Export] public Vector2 PanelBottomRightOffset { get; set; } = new(-16, -16);
	[Export] public Vector2 FloatingLayerTopLeftOffset { get; set; } = new(-300, -260);
	[Export] public Vector2 FloatingLayerBottomRightOffset { get; set; } = new(-20, -20);
	[Export] public Vector2 FloatingMoveOffset { get; set; } = new(80, -60);
	[Export] public Vector2 FloatingSpawnOffset { get; set; } = Vector2.Zero;
	[Export] public float FloatingDurationSeconds { get; set; } = 1.0f;
	[Export] public float FloatingLineSpacing { get; set; } = 18.0f;

	private PanelContainer _statusPanel;
	private NinePatchRect _panelBackground;
	private Label _headerLabel;
	private Label _dayActionsLabel;
	private Label _wellbeingRow;
	private Label _growthRow;
	private Label _careerRow;
	private Control _floatingTextLayer;
	private int _floatingIndex;
	private bool _showStatusPanel = true;

	public override void _Ready()
	{
		_instance = this;
		BindNodes();
		ApplyAnchors();
		ApplyBackgroundPlaceholderHelp();
	}

	public override void _ExitTree()
	{
		if (_instance == this)
			_instance = null;
	}

	public override void _Process(double delta)
	{
		if (GlobalVars.Instance == null)
			return;

		var s = GlobalVars.Instance;
		_headerLabel.Text = "Student Status";
		_dayActionsLabel.Text = $"Day {s.Profile.CurrentDay} / {s.Profile.FinalDay} | Actions {s.Profile.ActionsLeft} / {s.Profile.MaxActionsPerDay}";
		_wellbeingRow.Text = $"Wellbeing: Energy {s.Attributes.Energy} | Focus {s.Attributes.Focus}";
		_growthRow.Text = $"Growth: Knowledge {s.Attributes.Knowledge} | Confidence {s.Attributes.Confidence}";
		_careerRow.Text = $"Career: Readiness {s.Profile.CareerReadiness} | Network {s.Attributes.Networking} | Portfolio {s.Attributes.Portfolio}";
	}

	public static void AttachTo(Node parent, bool showStatusPanel = false)
	{
		if (parent.GetNodeOrNull<StatusOverlay>("StatusOverlay") != null)
			return;
		if (OverlayScene == null)
		{
			GD.PrintErr("StatusOverlay scene missing: res://scenes/UI/StatusOverlay.tscn");
			return;
		}

		var overlay = OverlayScene.Instantiate<StatusOverlay>();
		overlay.Name = "StatusOverlay";
		overlay._showStatusPanel = showStatusPanel;
		parent.AddChild(overlay);
	}

	public static void NotifyStatChange(string statName, int delta)
	{
		_instance?.SpawnFloatingText(statName, delta);
	}

	private void SpawnFloatingText(string statName, int delta)
	{
		if (_floatingTextLayer == null || delta == 0)
			return;

		var pop = new Label();
		pop.MouseFilter = Control.MouseFilterEnum.Ignore;
		pop.Text = $"{(delta > 0 ? "+" : "")}{delta} {statName}";
		pop.Modulate = delta > 0 ? new Color(0.6f, 1f, 0.6f, 1f) : new Color(1f, 0.6f, 0.6f, 1f);
		pop.Position = FloatingSpawnOffset + new Vector2(0, -(_floatingIndex % 6) * FloatingLineSpacing);
		_floatingIndex++;
		_floatingTextLayer.AddChild(pop);

		var tween = CreateTween();
		tween.SetParallel(true);
		tween.TweenProperty(pop, "position", pop.Position + FloatingMoveOffset, FloatingDurationSeconds);
		tween.TweenProperty(pop, "modulate:a", 0.0f, FloatingDurationSeconds);
		tween.Finished += () => pop.QueueFree();
	}

	private void BindNodes()
	{
		_statusPanel = GetNode<PanelContainer>("StatusPanel");
		_statusPanel.Visible = _showStatusPanel;
		_panelBackground = GetNode<NinePatchRect>("StatusPanel/PanelBackground");
		_headerLabel = GetNode<Label>("StatusPanel/Margin/StatusContent/HeaderLabel");
		_dayActionsLabel = GetNode<Label>("StatusPanel/Margin/StatusContent/DayActionsLabel");
		_wellbeingRow = GetNode<Label>("StatusPanel/Margin/StatusContent/WellbeingRow");
		_growthRow = GetNode<Label>("StatusPanel/Margin/StatusContent/GrowthRow");
		_careerRow = GetNode<Label>("StatusPanel/Margin/StatusContent/CareerRow");
		_floatingTextLayer = GetNode<Control>("FloatingTextLayer");

		// Overlay visuals should never consume clicks meant for scene UI beneath them.
		_statusPanel.MouseFilter = Control.MouseFilterEnum.Ignore;
		_floatingTextLayer.MouseFilter = Control.MouseFilterEnum.Ignore;
		SetMouseFilterRecursive(_statusPanel, Control.MouseFilterEnum.Ignore);
		SetMouseFilterRecursive(_floatingTextLayer, Control.MouseFilterEnum.Ignore);
	}

	private static void SetMouseFilterRecursive(Node node, Control.MouseFilterEnum mouseFilter)
	{
		if (node is Control control)
			control.MouseFilter = mouseFilter;

		foreach (Node child in node.GetChildren())
			SetMouseFilterRecursive(child, mouseFilter);
	}

	private void ApplyAnchors()
	{
		_statusPanel.OffsetLeft = PanelTopLeftOffset.X;
		_statusPanel.OffsetTop = PanelTopLeftOffset.Y;
		_statusPanel.OffsetRight = PanelBottomRightOffset.X;
		_statusPanel.OffsetBottom = PanelBottomRightOffset.Y;

		_floatingTextLayer.OffsetLeft = FloatingLayerTopLeftOffset.X;
		_floatingTextLayer.OffsetTop = FloatingLayerTopLeftOffset.Y;
		_floatingTextLayer.OffsetRight = FloatingLayerBottomRightOffset.X;
		_floatingTextLayer.OffsetBottom = FloatingLayerBottomRightOffset.Y;
	}

	private void ApplyBackgroundPlaceholderHelp()
	{
		_panelBackground.TooltipText = "Assign a NinePatchRect texture here for custom panel art (UI frame).";
	}
}
