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
	private Label _dayLabel;
	private Label _actionsLabel;
	private Label _energyLabel;
	private Label _focusLabel;
	private Label _knowledgeLabel;
	private Label _confidenceLabel;
	private Label _careerReadinessLabel;
	private Label _networkingLabel;
	private Label _portfolioLabel;
	private Control _floatingTextLayer;
	private int _floatingIndex;

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
		_headerLabel.Text = "Status";
		_dayLabel.Text = $"Day {s.Profile.CurrentDay} / {s.Profile.FinalDay}";
		_actionsLabel.Text = $"Actions {s.Profile.ActionsLeft} / {s.Profile.MaxActionsPerDay}";
		_energyLabel.Text = $"Energy {s.Attributes.Energy}";
		_focusLabel.Text = $"Focus {s.Attributes.Focus}";
		_knowledgeLabel.Text = $"Knowledge {s.Attributes.Knowledge}";
		_confidenceLabel.Text = $"Confidence {s.Attributes.Confidence}";
		_careerReadinessLabel.Text = $"Career Readiness {s.Profile.CareerReadiness}";
		_networkingLabel.Text = $"Networking {s.Attributes.Networking}";
		_portfolioLabel.Text = $"Portfolio {s.Attributes.Portfolio}";
	}

	public static void AttachTo(Node parent)
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
		_panelBackground = GetNode<NinePatchRect>("StatusPanel/PanelBackground");
		_headerLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/HeaderLabel");
		_dayLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/DayLabel");
		_actionsLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/ActionsLabel");
		_energyLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/EnergyLabel");
		_focusLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/FocusLabel");
		_knowledgeLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/KnowledgeLabel");
		_confidenceLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/ConfidenceLabel");
		_careerReadinessLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/CareerReadinessLabel");
		_networkingLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/NetworkingLabel");
		_portfolioLabel = GetNode<Label>("StatusPanel/Margin/StatsContainer/PortfolioLabel");
		_floatingTextLayer = GetNode<Control>("FloatingTextLayer");
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
