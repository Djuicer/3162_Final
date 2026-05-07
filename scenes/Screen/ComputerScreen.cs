using Godot;
using System;

public partial class ComputerScreen : Node2D
{
    private Label _profileLabel;
    private Label _statsLabel;
    private Label _hintLabel;
    private Label _feedbackLabel;
    private bool _isTransitioning;

    public override void _Ready()
    {
        BuildUi();
        RefreshUi();
        ShowFeedback("Welcome back. Plan your day.");
    }

    private void BuildUi()
    {
        var panel = new Panel { Name = "ProfilePanel", Position = new Vector2(180, 35), Size = new Vector2(820, 570) };
        AddChild(panel);

        var layout = new VBoxContainer { Position = new Vector2(12, 12), Size = new Vector2(796, 546) };
        panel.AddChild(layout);

        layout.AddChild(new Label { Text = "Plan Your Day", ThemeTypeVariation = "HeaderSmall" });

        _profileLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart };
        layout.AddChild(_profileLabel);

        _statsLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart };
        layout.AddChild(_statsLabel);

        layout.AddChild(new Label { Text = "Available Actions", ThemeTypeVariation = "HeaderSmall" });
        layout.AddChild(new Label { Text = "Study CS: Play Focus Catch to gain Knowledge. Costs 1 action and Energy.", AutowrapMode = TextServer.AutowrapMode.WordSmart });
        layout.AddChild(new Label { Text = "Coding Practice: Play Bug Squash to gain Knowledge and Career Readiness. Costs 1 action and Energy.", AutowrapMode = TextServer.AutowrapMode.WordSmart });
        layout.AddChild(new Label { Text = "Apply for Internship: Play Interview Rhythm to gain Career Readiness and Confidence. Costs 1 action and Energy.", AutowrapMode = TextServer.AutowrapMode.WordSmart });
        layout.AddChild(new Label { Text = "Procrastinate: Recover a little Energy, but lose Focus or Confidence. Costs 1 action.", AutowrapMode = TextServer.AutowrapMode.WordSmart });
        layout.AddChild(new Label { Text = "Sleep: Return to your bed when you are out of actions or Energy.", AutowrapMode = TextServer.AutowrapMode.WordSmart });

        _hintLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, Modulate = new Color(0.85f, 0.95f, 1f) };
        layout.AddChild(_hintLabel);

        layout.AddChild(CreateActionButton("Study CS", OnStudyCsPressed));
        layout.AddChild(CreateActionButton("Coding Practice", OnCodingPracticePressed));
        layout.AddChild(CreateActionButton("Apply for Internship", OnApplyForInternshipPressed));
        layout.AddChild(CreateActionButton("Procrastinate", OnProcrastinatePressed));
        layout.AddChild(CreateActionButton("Back to Dormitory", OnBackPressed));

        _feedbackLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart, Modulate = new Color(0.95f, 0.95f, 0.4f) };
        layout.AddChild(_feedbackLabel);
    }

    private Button CreateActionButton(string text, Action action)
    {
        var button = new Button { Text = text, CustomMinimumSize = new Vector2(0, 36) };
        button.Pressed += action;
        return button;
    }

    private bool CanDoAction()
    {
        var state = GlobalVars.Instance;
        if (state.Profile.ActionsLeft <= 0)
        {
            ShowFeedback("You have no actions left. Go to bed and sleep.");
            RefreshUi();
            return false;
        }

        if (state.Attributes.Energy <= 0)
        {
            ShowFeedback("You are out of energy. Go to bed and sleep.");
            RefreshUi();
            return false;
        }

        return true;
    }

    private void OnStudyCsPressed() { if (!_isTransitioning && CanDoAction()) { _isTransitioning = true; GetTree().ChangeSceneToFile("res://scenes/Minigames/FocusCatch/focus_catch.tscn"); } }
    private void OnCodingPracticePressed() { if (!_isTransitioning && CanDoAction()) { _isTransitioning = true; GetTree().ChangeSceneToFile("res://scenes/Minigames/BugSquash/bug_squash.tscn"); } }
    private void OnApplyForInternshipPressed() { if (!_isTransitioning && CanDoAction()) { _isTransitioning = true; GetTree().ChangeSceneToFile("res://scenes/Minigames/InterviewRhythm/interview_rhythm.tscn"); } }
    private void OnBackPressed() { if (!_isTransitioning) { _isTransitioning = true; GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn"); } }

    private void OnProcrastinatePressed()
    {
        var state = GlobalVars.Instance;
        if (!state.Profile.TrySpendAction(1))
        {
            ShowFeedback("You have no actions left. Go to bed and sleep.");
            RefreshUi();
            return;
        }

        state.Attributes.IncreaseEnergy(10);
        state.Attributes.DecreaseConfidence(5);
        state.Attributes.DecreaseFocus(5);
        RefreshUi();
        ShowFeedback("You rested, but your motivation slipped.");
    }

    private void RefreshUi()
    {
        var state = GlobalVars.Instance;
        var profile = state.Profile;
        var attributes = state.Attributes;

        _profileLabel.Text = $"Name: {profile.PlayerName}\nDay: {profile.CurrentDay} / {profile.FinalDay}\nActions Left: {profile.ActionsLeft} / {profile.MaxActionsPerDay}\nGoal: {profile.Goal}\nCareer Readiness: {profile.CareerReadiness}/100";
        _statsLabel.Text = $"Focus: {attributes.Focus}/100\nEnergy: {attributes.Energy}/100\nKnowledge: {attributes.Knowledge}/100\nConfidence: {attributes.Confidence}/100";
        _hintLabel.Text = $"Hint: {state.GetCurrentHintMessage()}";
    }

    private void ShowFeedback(string message) => _feedbackLabel.Text = $"Feedback: {message}";
}
