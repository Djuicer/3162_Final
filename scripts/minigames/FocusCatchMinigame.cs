using Godot;
using System.Collections.Generic;

public partial class FocusCatchMinigame : Control
{
    private const float TotalTimeSeconds = 30.0f;
    private const float CatcherSpeed = 450.0f;
    private const float SpawnIntervalSeconds = 0.65f;

    private Label _timerLabel;
    private Label _scoreLabel;
    private Label _resultLabel;
    private Button _continueButton;
    private ColorRect _catcher;
    private Control _playArea;

    private float _timeRemaining = TotalTimeSeconds;
    private float _spawnTimer = 0.0f;
    private int _score = 0;
    private bool _isFinished = false;
    private bool _rewardApplied = false;
    private bool _hasStarted = false;

    private Control _introOverlay;
    private Button _startButton;

    private readonly RandomNumberGenerator _rng = new RandomNumberGenerator();
    private readonly List<FallingItem> _items = new List<FallingItem>();

    private class FallingItem
    {
        public Label Node;
        public bool IsStudyItem;
        public float Speed;
    }

    public override void _Ready()
    {
        StatusOverlay.AttachTo(this, false);
        _timerLabel = GetNode<Label>("Panel/VBox/TimerLabel");
        _scoreLabel = GetNode<Label>("Panel/VBox/ScoreLabel");
        _resultLabel = GetNode<Label>("Panel/VBox/ResultLabel");
        var instructionLabel = GetNodeOrNull<Label>("Panel/VBox/InstructionLabel");
        _continueButton = GetNode<Button>("Panel/VBox/ContinueButton");
        _catcher = GetNode<ColorRect>("PlayArea/Catcher");
        _playArea = GetNode<Control>("PlayArea");

        _continueButton.Pressed += OnContinuePressed;
        _continueButton.Visible = false;
        _resultLabel.Text = "";
        if (instructionLabel != null)
            instructionLabel.Text = "Move left/right to catch NOTE and avoid PHONE.";

        _rng.Randomize();

        BuildIntroOverlay(
            "Focus Catch",
            "Catch study items.\nAvoid distractions.\nMove with A / D or Left / Right."
        );

        UpdateHud();
    }

    public override void _Process(double delta)
    {
        if (_isFinished || !_hasStarted)
            return;

        float dt = (float)delta;
        MoveCatcher(dt);

        _spawnTimer += dt;
        if (_spawnTimer >= SpawnIntervalSeconds)
        {
            _spawnTimer = 0.0f;
            SpawnItem();
        }

        UpdateItems(dt);

        _timeRemaining -= dt;
        if (_timeRemaining <= 0.0f)
        {
            _timeRemaining = 0.0f;
            EndMinigame();
        }

        UpdateHud();
    }

    private void MoveCatcher(float delta)
    {
        float input = Input.GetActionStrength("ui_right") - Input.GetActionStrength("ui_left");
        if (Input.IsKeyPressed(Key.A))
            input -= 1.0f;
        if (Input.IsKeyPressed(Key.D))
            input += 1.0f;

        Vector2 pos = _catcher.Position;
        pos.X += input * CatcherSpeed * delta;

        float minX = 0.0f;
        float maxX = _playArea.Size.X - _catcher.Size.X;
        pos.X = Mathf.Clamp(pos.X, minX, maxX);
        _catcher.Position = pos;
    }

    private void SpawnItem()
    {
        bool isStudyItem = _rng.Randf() > 0.4f;
        var item = new Label();

        item.Text = isStudyItem ? "NOTE" : "PHONE";
        item.HorizontalAlignment = HorizontalAlignment.Center;
        item.VerticalAlignment = VerticalAlignment.Center;
        item.Size = new Vector2(80, 30);
        item.Modulate = isStudyItem ? new Color(0.65f, 1f, 0.7f) : new Color(1f, 0.65f, 0.65f);

        float maxX = _playArea.Size.X - item.Size.X;
        item.Position = new Vector2(_rng.RandfRange(0, Mathf.Max(0, maxX)), 0);

        _playArea.AddChild(item);

        _items.Add(new FallingItem
        {
            Node = item,
            IsStudyItem = isStudyItem,
            Speed = _rng.RandfRange(170f, 260f)
        });
    }

    private void UpdateItems(float delta)
    {
        Rect2 catcherRect = new Rect2(_catcher.Position, _catcher.Size);

        for (int i = _items.Count - 1; i >= 0; i--)
        {
            FallingItem item = _items[i];
            if (!IsInstanceValid(item.Node))
            {
                _items.RemoveAt(i);
                continue;
            }

            Vector2 pos = item.Node.Position;
            pos.Y += item.Speed * delta;
            item.Node.Position = pos;

            Rect2 itemRect = new Rect2(item.Node.Position, item.Node.Size);
            bool caught = catcherRect.Intersects(itemRect);
            bool outOfBounds = item.Node.Position.Y > _playArea.Size.Y;

            if (caught)
            {
                _score += item.IsStudyItem ? 1 : -1;
                if (_score < 0)
                    _score = 0;
                RemoveItemAt(i);
            }
            else if (outOfBounds)
            {
                RemoveItemAt(i);
            }
        }
    }

    private void RemoveItemAt(int index)
    {
        FallingItem item = _items[index];
        if (IsInstanceValid(item.Node))
            item.Node.QueueFree();
        _items.RemoveAt(index);
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

        foreach (FallingItem item in _items)
        {
            if (IsInstanceValid(item.Node))
                item.Node.QueueFree();
        }
        _items.Clear();

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
            attributes.IncreaseKnowledge(15);
            attributes.IncreaseFocus(5);
            attributes.DecreaseEnergy(10);
            SetResultText(
                "Excellent Focus!",
                "You avoided distractions and made strong study progress.",
                "+15 Knowledge\n+5 Focus\n-10 Energy"
            );
        }
        else if (_score >= 8)
        {
            attributes.IncreaseKnowledge(10);
            attributes.DecreaseEnergy(10);
            SetResultText(
                "Good Study Session",
                "You stayed on track and made steady study progress.",
                "+10 Knowledge\n-10 Energy"
            );
        }
        else
        {
            attributes.IncreaseKnowledge(3);
            attributes.DecreaseEnergy(15);
            attributes.DecreaseConfidence(5);
            SetResultText(
                "Rough Session",
                "Distractions got in the way, but you still learned a little.",
                "+3 Knowledge\n-15 Energy\n-5 Confidence"
            );
        }

        _continueButton.Visible = true;
    }

    private void SetResultText(string title, string feedback, string rewardSummary)
    {
        _resultLabel.Text = $"{title}\nFinal Score: {_score}\n{feedback}\nRewards:\n{rewardSummary}";
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
            OffsetLeft = -280f,
            OffsetTop = -170f,
            OffsetRight = 280f,
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

        var titleLabel = new Label { Text = title, HorizontalAlignment = HorizontalAlignment.Center };
        var instructionLabel = new Label { Text = instructions, AutowrapMode = TextServer.AutowrapMode.WordSmart };

        _startButton = new Button { Text = "Start", SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter };
        _startButton.Pressed += OnStartPressed;

        vbox.AddChild(titleLabel);
        vbox.AddChild(instructionLabel);
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
