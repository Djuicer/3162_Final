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

    private readonly RandomNumberGenerator _rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        _timerLabel = GetNode<Label>("Panel/VBox/TimerLabel");
        _scoreLabel = GetNode<Label>("Panel/VBox/ScoreLabel");
        _resultLabel = GetNode<Label>("Panel/VBox/ResultLabel");
        _bugButton = GetNode<Button>("PlayArea/BugButton");
        _continueButton = GetNode<Button>("Panel/VBox/ContinueButton");

        _rng.Randomize();

        _bugButton.Pressed += OnBugClicked;
        _continueButton.Pressed += OnContinuePressed;

        _resultLabel.Text = "";
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
        _bugButton.Visible = false;

        var profile = GlobalVars.Instance.Profile;
        var attributes = GlobalVars.Instance.Attributes;

        profile.TrySpendAction(1);

        if (_score >= 15)
        {
            attributes.IncreaseKnowledge(10);
            profile.IncreaseCareerReadiness(15);
            attributes.DecreaseEnergy(10);
            _resultLabel.Text = "Great run! Score 15+\nKnowledge +10, Career Readiness +15, Energy -10";
        }
        else if (_score >= 8)
        {
            attributes.IncreaseKnowledge(6);
            profile.IncreaseCareerReadiness(8);
            attributes.DecreaseEnergy(10);
            _resultLabel.Text = "Solid run! Score 8-14\nKnowledge +6, Career Readiness +8, Energy -10";
        }
        else
        {
            attributes.IncreaseKnowledge(2);
            profile.IncreaseCareerReadiness(3);
            attributes.DecreaseEnergy(15);
            attributes.DecreaseFocus(5);
            _resultLabel.Text = "Keep practicing! Score below 8\nKnowledge +2, Career Readiness +3, Energy -15, Focus -5";
        }

        _continueButton.Visible = true;
    }

    private void OnContinuePressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/Screen/screen.tscn");
    }
}
