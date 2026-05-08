using Godot;

public partial class BugSquashMinigame : Control
{
	private const float TotalTimeSeconds = 30.0f;

	private Label _timerLabel;
	private Label _scoreLabel;
	private Label _resultLabel;
	private Button _bugButton;
	private Button _continueButton;

	private float _timeRemaining = TotalTimeSeconds;
	private int _score = 0;
	private bool _isFinished = false;
	private bool _rewardApplied = false;

	private readonly RandomNumberGenerator _rng = new RandomNumberGenerator();

	public override void _Ready()
	{
		StatusOverlay.AttachTo(this, false);
		_timerLabel = GetNode<Label>("Panel/VBox/TimerLabel");
		_scoreLabel = GetNode<Label>("Panel/VBox/ScoreLabel");
		_resultLabel = GetNode<Label>("Panel/VBox/ResultLabel");
		var instructionLabel = GetNodeOrNull<Label>("Panel/VBox/InstructionLabel");
		_bugButton = GetNode<Button>("PlayArea/BugButton");
		_continueButton = GetNode<Button>("Panel/VBox/ContinueButton");

		_rng.Randomize();

		_bugButton.Pressed += OnBugClicked;
		_continueButton.Pressed += OnContinuePressed;

		_resultLabel.Text = "";
		if (instructionLabel != null)
			instructionLabel.Text = "Click BUG as fast as possible before the timer ends.";
		_continueButton.Visible = false;

		SpawnBugAtRandomPosition();
		UpdateHud();
	}

	public override void _Process(double delta)
	{
		if (_isFinished)
			return;

		_timeRemaining -= (float)delta;
		if (_timeRemaining <= 0.0f)
		{
			_timeRemaining = 0.0f;
			EndMinigame();
		}

		UpdateHud();
	}

	private void OnBugClicked()
	{
		if (_isFinished)
			return;

		_score += 1;
		UpdateHud();
		SpawnBugAtRandomPosition();
	}

	private void SpawnBugAtRandomPosition()
	{
		var playArea = GetNode<Control>("PlayArea");
		var areaSize = playArea.Size;
		var bugSize = _bugButton.Size;

		float maxX = Mathf.Max(0, areaSize.X - bugSize.X);
		float maxY = Mathf.Max(0, areaSize.Y - bugSize.Y);

		_bugButton.Position = new Vector2(
			_rng.RandfRange(0, maxX),
			_rng.RandfRange(0, maxY)
		);
	}

	private void UpdateHud()
	{
		_timerLabel.Text = $"Timer: {Mathf.CeilToInt(_timeRemaining)}";
		_scoreLabel.Text = $"Score: {_score}";
	}

	private void EndMinigame()
	{
		_isFinished = true;
		if (_rewardApplied)
			return;
		_rewardApplied = true;
		_bugButton.Visible = false;

		var profile = GlobalVars.Instance.Profile;
		var attributes = GlobalVars.Instance.Attributes;

		if (!profile.TrySpendAction(1))
		{
			_resultLabel.Text = "No actions left. No rewards granted.";
			_continueButton.Visible = true;
			return;
		}

		if (_score >= 15)
		{
			attributes.IncreaseKnowledge(10);
			attributes.IncreasePortfolio(8);
			profile.IncreaseCareerReadiness(15);
			attributes.DecreaseEnergy(10);
			_resultLabel.Text =
				"Clean Debugging!\n" +
				"Final Score: " + _score + "\n" +
				"You squashed issues quickly and kept your workflow sharp.\n" +
				"Rewards:\n" +
				"+10 Knowledge\n" +
				"+8 Portfolio\n" +
				"+15 Career Readiness\n" +
				"-10 Energy";
		}
		else if (_score >= 8)
		{
			attributes.IncreaseKnowledge(6);
			attributes.IncreasePortfolio(5);
			profile.IncreaseCareerReadiness(8);
			attributes.DecreaseEnergy(10);
			_resultLabel.Text =
				"Good Progress\n" +
				"Final Score: " + _score + "\n" +
				"You made solid debugging progress with room to optimize.\n" +
				"Rewards:\n" +
				"+6 Knowledge\n" +
				"+5 Portfolio\n" +
				"+8 Career Readiness\n" +
				"-10 Energy";
		}
		else
		{
			attributes.IncreaseKnowledge(2);
			attributes.IncreasePortfolio(2);
			profile.IncreaseCareerReadiness(3);
			attributes.DecreaseEnergy(15);
			attributes.DecreaseFocus(5);
			_resultLabel.Text =
				"Buggy Session\n" +
				"Final Score: " + _score + "\n" +
				"You hit a few blockers, but each pass still built experience.\n" +
				"Rewards:\n" +
				"+2 Knowledge\n" +
				"+2 Portfolio\n" +
				"+3 Career Readiness\n" +
				"-15 Energy\n" +
				"-5 Focus";
		}

		_continueButton.Visible = true;
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
