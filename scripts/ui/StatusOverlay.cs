using Godot;

public partial class StatusOverlay : CanvasLayer
{
	private static readonly PackedScene OverlayScene = ResourceLoader.Load<PackedScene>("res://scenes/UI/StatusOverlay.tscn");
	private static readonly PackedScene UserProfileScene = ResourceLoader.Load<PackedScene>("res://scenes/UI/UserProfileOverlay/user_profile_overlay.tscn");
	private static readonly PackedScene ProfileHintScene = ResourceLoader.Load<PackedScene>("res://scenes/UI/ProfileHint/profile_hint.tscn");
	private static StatusOverlay _instance;

	[Export] public Vector2 FloatingLayerTopLeftOffset { get; set; } = new(-300, -260);
	[Export] public Vector2 FloatingLayerBottomRightOffset { get; set; } = new(-20, -20);
	[Export] public Vector2 FloatingMoveOffset { get; set; } = new(80, -60);
	[Export] public Vector2 FloatingSpawnOffset { get; set; } = Vector2.Zero;
	[Export] public float FloatingDurationSeconds { get; set; } = 1.0f;
	[Export] public float FloatingLineSpacing { get; set; } = 18.0f;

	private Control _floatingTextLayer;
	private PanelContainer _reactionPanel;
	private Label _reactionLabel;
	private int _floatingIndex;
	private Label _dayLabel;
	private Label _actionsLabel;

	public override void _Ready()
	{
		_instance = this;
		_floatingTextLayer = GetNode<Control>("FloatingTextLayer");
		_reactionPanel = GetNode<PanelContainer>("ReactionPanel");
		_reactionLabel = GetNode<Label>("ReactionPanel/Margin/ReactionLabel");
		ApplyAnchors();
		SetMouseFilterRecursive(_floatingTextLayer, Control.MouseFilterEnum.Ignore);
		SetMouseFilterRecursive(_reactionPanel, Control.MouseFilterEnum.Ignore);
		_reactionPanel.Visible = false;
		AttachUserProfileAndHint();
		_dayLabel = GetNodeOrNull<Label>("DayActionsHud/HudFrame/MarginContainer/HudContent/DayLabel");
		_actionsLabel = GetNodeOrNull<Label>("DayActionsHud/HudFrame/MarginContainer/HudContent/ActionsLabel");
		RefreshDayActionsHud();
	}

	private void AttachUserProfileAndHint()
	{
		if (GetNodeOrNull<UserProfileOverlay>("UserProfileOverlay") == null && UserProfileScene != null)
		{
			var profileOverlay = UserProfileScene.Instantiate<UserProfileOverlay>();
			profileOverlay.Name = "UserProfileOverlay";
			AddChild(profileOverlay);
		}

		if (GetNodeOrNull<Control>("ProfileHint") == null && ProfileHintScene != null)
		{
			var hint = ProfileHintScene.Instantiate<Control>();
			hint.Name = "ProfileHint";
			AddChild(hint);
		}
	}

	public override void _Process(double delta)
	{
		RefreshDayActionsHud();
	}

	public override void _ExitTree()
	{
		if (_instance == this)
			_instance = null;
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
		parent.AddChild(overlay);
	}

	public static void NotifyStatChange(string statName, int delta) => _instance?.SpawnFloatingText(statName, delta);
	public static void ShowReaction(string message) => _instance?.ShowReactionMessage(message);

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

	private void ShowReactionMessage(string message)
	{
		if (_reactionPanel == null || _reactionLabel == null || string.IsNullOrWhiteSpace(message)) return;
		_reactionLabel.Text = message;
		_reactionPanel.Modulate = new Color(1, 1, 1, 1);
		_reactionPanel.Visible = true;
		var timer = GetTree().CreateTimer(2.8f);
		timer.Timeout += () =>
		{
			if (!IsInstanceValid(_reactionPanel)) return;
			var tween = CreateTween();
			tween.TweenProperty(_reactionPanel, "modulate:a", 0.0f, 0.3f);
			tween.Finished += () => { if (IsInstanceValid(_reactionPanel)) { _reactionPanel.Visible = false; _reactionPanel.Modulate = new Color(1, 1, 1, 1); } };
		};
	}

	private static void SetMouseFilterRecursive(Node node, Control.MouseFilterEnum mouseFilter)
	{
		if (node is Control c) c.MouseFilter = mouseFilter;
		foreach (Node child in node.GetChildren()) SetMouseFilterRecursive(child, mouseFilter);
	}

	
	private void RefreshDayActionsHud()
	{
		if (GlobalVars.Instance == null || _dayLabel == null || _actionsLabel == null)
			return;

		var profile = GlobalVars.Instance.Profile;
		_dayLabel.Text = $"Day {profile.CurrentDay} / {profile.FinalDay}";
		_actionsLabel.Text = $"Actions {profile.ActionsLeft} / {profile.MaxActionsPerDay}";
	}
private void ApplyAnchors()
	{
		_floatingTextLayer.OffsetLeft = FloatingLayerTopLeftOffset.X;
		_floatingTextLayer.OffsetTop = FloatingLayerTopLeftOffset.Y;
		_floatingTextLayer.OffsetRight = FloatingLayerBottomRightOffset.X;
		_floatingTextLayer.OffsetBottom = FloatingLayerBottomRightOffset.Y;
	}
}
