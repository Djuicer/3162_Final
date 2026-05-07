using Godot;
using System;

public partial class ComputerScreen : Node2D
{
    private Label _profileLabel;
    private Label _statsLabel;
    private Label _feedbackLabel;
    private bool _isTransitioning;

    public override void _Ready()
    {
        BuildUi();
        RefreshUi();
        ShowFeedback("Welcome back. Choose an action.");
    }

    private void BuildUi()
    {
        var panel = new Panel
        {
            Name = "ProfilePanel",
            Position = new Vector2(250, 45),
            Size = new Vector2(710, 530)
        };
        AddChild(panel);

        var layout = new VBoxContainer
        {
            Position = new Vector2(12, 12),
            Size = new Vector2(686, 506)
        };
        panel.AddChild(layout);

        var title = new Label { Text = "Student Profile", ThemeTypeVariation = "HeaderSmall" };
        layout.AddChild(title);

        layout.AddChild(new Label
        {
            Text = "Choose one activity. Each activity uses 1 action. When actions reach 0, return to dorm and sleep.",
            AutowrapMode = TextServer.AutowrapMode.WordSmart
        });

        _profileLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart };
        layout.AddChild(_profileLabel);

        _statsLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart };
        layout.AddChild(_statsLabel);

        var actionsTitle = new Label { Text = "Temporary Test Actions" };
        layout.AddChild(actionsTitle);

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
        var button = new Button { Text = text, CustomMinimumSize = new Vector2(0, 40) };
        button.Pressed += action;
        return button;
    }

    private void OnStudyCsPressed()
    {
        if (_isTransitioning) return;
        var state = GlobalVars.Instance;
        if (state.Profile.ActionsLeft <= 0)
        {
            ShowFeedback("You are too tired to do more today. Go to sleep.");
            RefreshUi();
            return;
        }

        _isTransitioning = true;
        GetTree().ChangeSceneToFile("res://scenes/Minigames/FocusCatch/focus_catch.tscn");
    }

    private void OnCodingPracticePressed()
    {
        if (_isTransitioning) return;
        var state = GlobalVars.Instance;
        if (state.Profile.ActionsLeft <= 0)
        {
            ShowFeedback("You are too tired to do more today. Go to sleep.");
            RefreshUi();
            return;
        }

        _isTransitioning = true;
        GetTree().ChangeSceneToFile("res://scenes/Minigames/BugSquash/bug_squash.tscn");
    }


    private void OnApplyForInternshipPressed()
    {
        if (_isTransitioning) return;
        var state = GlobalVars.Instance;
        if (state.Profile.ActionsLeft <= 0)
        {
            ShowFeedback("You are too tired to do more today. Go to sleep.");
            RefreshUi();
            return;
        }

        _isTransitioning = true;
        GetTree().ChangeSceneToFile("res://scenes/Minigames/InterviewRhythm/interview_rhythm.tscn");
    }

    private void OnBackPressed()
    {
        if (_isTransitioning) return;
        _isTransitioning = true;
        GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn");
    }

    private void OnProcrastinatePressed()
    {
        var state = GlobalVars.Instance;
        if (!state.Profile.TrySpendAction(1))
        {
            ShowFeedback("You are too tired to do more today. Go to sleep.");
            RefreshUi();
            return;
        }

        state.Attributes.IncreaseEnergy(10);
        state.Attributes.DecreaseConfidence(5);
        state.Attributes.DecreaseFocus(5);

        RefreshUi();
        PrintState("Procrastinate");
        ShowFeedback("You rested, but your motivation slipped.");
    }

    private void RefreshUi()
    {
        var state = GlobalVars.Instance;
        var profile = state.Profile;
        var attributes = state.Attributes;

        _profileLabel.Text =
            $"Name: {profile.PlayerName}\n" +
            $"Year Level: {profile.YearLevel}\n" +
            $"Semester: {profile.Semester}\n" +
            $"Day: {profile.CurrentDay} / {profile.FinalDay}\n" +
            $"Actions Left: {profile.ActionsLeft} / {profile.MaxActionsPerDay}\n" +
            $"Goal: {profile.Goal}\n" +
            $"Career Readiness: {profile.CareerReadiness}/100";

        _statsLabel.Text =
            $"Focus: {attributes.Focus}/100\n" +
            $"Energy: {attributes.Energy}/100\n" +
            $"Knowledge: {attributes.Knowledge}/100\n" +
            $"Confidence: {attributes.Confidence}/100";
    }

    private void ShowFeedback(string message)
    {
        _feedbackLabel.Text = $"Feedback: {message}";
    }

    private void PrintState(string actionName)
    {
        var profile = GlobalVars.Instance.Profile;
        var attributes = GlobalVars.Instance.Attributes;

        GD.Print($"[{actionName}] Name={profile.PlayerName}, Day={profile.CurrentDay}, CareerReadiness={profile.CareerReadiness}/100, Focus={attributes.Focus}/100, Energy={attributes.Energy}/100, Knowledge={attributes.Knowledge}/100, Confidence={attributes.Confidence}/100");
    }
}
