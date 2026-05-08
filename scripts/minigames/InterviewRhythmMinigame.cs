using Godot;

public partial class InterviewRhythmMinigame : Control
{
	private const int TotalAttempts = 10;
	private const float MarkerSpeed = 420.0f;

	private Label _attemptLabel;
	private Label _scoreLabel;
	private Label _instructionLabel;
	private Label _resultLabel;
	private Button _continueButton;
	private Control _timingBar;
	private Control _targetZone;
	private Control _marker;

	private int _attemptsUsed = 0;
	private int _score = 0;
	private float _markerDirection = 1.0f;
	private bool _isFinished = false;
	private bool _rewardApplied = false;
	private bool _hasStarted = false;

	private Control _introOverlay;
	private Button _startButton;

	public override void _Ready()
	{
		StatusOverlay.AttachTo(this, false);
		_attemptLabel = GetNode<Label>("Panel/VBox/AttemptLabel");
		_scoreLabel = GetNode<Label>("Panel/VBox/ScoreLabel");
		_instructionLabel = GetNode<Label>("Panel/VBox/InstructionLabel");
		_resultLabel = GetNode<Label>("Panel/VBox/ResultLabel");
		_continueButton = GetNode<Button>("Panel/VBox/ContinueButton");
		_timingBar = GetNode<Control>("TimingBar");
		_targetZone = GetNode<Control>("TimingBar/TargetZone");
		_marker = GetNode<Control>("TimingBar/Marker");

		_continueButton.Pressed += OnContinuePressed;
		_continueButton.Visible = false;
		_resultLabel.Text = "";

		BuildIntroOverlay(
			"Interview Rhythm",
			"Press Space when the marker is inside the target zone.\nGood timing improves your interview performance."
		);

		UpdateHud();
	}

	public override void _Process(double delta)
	{
		if (_isFinished || !_hasStarted)
			return;

		MoveMarker((float)delta);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (_isFinished || !_hasStarted)
			return;

		if (@event.IsActionPressed("ui_accept") || (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo && keyEvent.Keycode == Key.Space))
		{
			ResolveAttempt();
			GetViewport().SetInputAsHandled();
		}
	}

	private void MoveMarker(float delta)
	{
		float markerWidth = _marker.Size.X;
		float maxX = Mathf.Max(0.0f, _timingBar.Size.X - markerWidth);

		Vector2 pos = _marker.Position;
		pos.X += MarkerSpeed * _markerDirection * delta;

		if (pos.X <= 0.0f)
		{
			pos.X = 0.0f;
			_markerDirection = 1.0f;
		}
		else if (pos.X >= maxX)
		{
			pos.X = maxX;
			_markerDirection = -1.0f;
		}

		_marker.Position = pos;
	}

	private void ResolveAttempt()
	{
		_attemptsUsed += 1;

		if (IsMarkerInsideTargetZone())
			_score += 1;

		if (_attemptsUsed >= TotalAttempts)
			EndMinigame();

		UpdateHud();
	}

	private bool IsMarkerInsideTargetZone()
	{
		float markerCenter = _marker.Position.X + (_marker.Size.X * 0.5f);
		float targetLeft = _targetZone.Position.X;
		float targetRight = _targetZone.Position.X + _targetZone.Size.X;
		return markerCenter >= targetLeft && markerCenter <= targetRight;
	}

	private void UpdateHud()
	{
		int currentAttempt = Mathf.Min(_attemptsUsed + 1, TotalAttempts);
		_attemptLabel.Text = _isFinished
			? $"Attempt: {TotalAttempts} / {TotalAttempts}"
			: $"Attempt: {currentAttempt} / {TotalAttempts}";
		_scoreLabel.Text = $"Score: {_score}";

		if (!_isFinished)
			_instructionLabel.Text = "Press Space when the marker is in the target zone";
	}

	private void EndMinigame()
	{
		_isFinished = true;
		if (_rewardApplied)
			return;
		_rewardApplied = true;

		var profile = GlobalVars.Instance.Profile;
		var attributes = GlobalVars.Instance.Attributes;

		if (!profile.TrySpendAction(1))
		{
			_resultLabel.Text = "No actions left. No rewards granted.";
			_instructionLabel.Text = "Interview finished.";
			_continueButton.Visible = true;
			return;
		}

		if (_score >= 7)
		{
			profile.IncreaseCareerReadiness(20);
			attributes.IncreaseConfidence(10);
			attributes.DecreaseEnergy(10);
			_resultLabel.Text = "Excellent interview rhythm! Score 7-10\nCareer Readiness +20, Confidence +10, Energy -10";
		}
		else if (_score >= 4)
		{
			profile.IncreaseCareerReadiness(10);
			attributes.IncreaseConfidence(3);
			attributes.DecreaseEnergy(10);
			_resultLabel.Text = "Good interview timing! Score 4-6\nCareer Readiness +10, Confidence +3, Energy -10";
		}
		else
		{
			profile.IncreaseCareerReadiness(5);
			attributes.DecreaseConfidence(10);
			attributes.DecreaseEnergy(10);
			_resultLabel.Text = "Rough interview attempt. Score 0-3\nCareer Readiness +5, Confidence -10, Energy -10";
		}

		_instructionLabel.Text = "Interview finished.";
		_continueButton.Visible = true;
	}

	private void BuildIntroOverlay(string title, string instructions)
	{
		_introOverlay = new ColorRect
		{
			Name = "IntroOverlay",
			AnchorRight = 1.0f,
			AnchorBottom = 1.0f,
			Color = new Color(0f, 0f, 0f, 0.8f),
			MouseFilter = MouseFilterEnum.Stop
		};

		var panel = new Panel
		{
			AnchorLeft = 0.5f,
			AnchorTop = 0.5f,
			AnchorRight = 0.5f,
			AnchorBottom = 0.5f,
			OffsetLeft = -300f,
			OffsetTop = -170f,
			OffsetRight = 300f,
			OffsetBottom = 170f
		};

		var vbox = new VBoxContainer
		{
			AnchorRight = 1.0f,
			AnchorBottom = 1.0f,
			OffsetLeft = 20f,
			OffsetTop = 20f,
			OffsetRight = -20f,
			OffsetBottom = -20f
		};

		vbox.AddChild(new Label { Text = title, HorizontalAlignment = HorizontalAlignment.Center });
		vbox.AddChild(new Label { Text = instructions, AutowrapMode = TextServer.AutowrapMode.WordSmart });
		_startButton = new Button { Text = "Start", SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter };
		_startButton.Pressed += OnStartPressed;
		vbox.AddChild(_startButton);
		panel.AddChild(vbox);
		_introOverlay.AddChild(panel);
		AddChild(_introOverlay);
	}

	private void OnStartPressed()
	{
		_hasStarted = true;
		_startButton.Disabled = true;
		_introOverlay.QueueFree();
	}

	private void OnContinuePressed()
	{
		_continueButton.Disabled = true;
		var state = GlobalVars.Instance;
		string returnScene = state.ReturnScenePath;
		if (string.IsNullOrEmpty(returnScene))
			returnScene = "res://scenes/Screen/screen.tscn";
		if (!string.IsNullOrEmpty(state.ReturnSpawnId))
			state.QueuePendingSpawn(state.ReturnSpawnId);

		TransitionManager.Instance?.ChangeSceneToFileWithFade(returnScene);
	}
}
