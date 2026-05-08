using Godot;
using System;

public partial class EndingScreen : Control
{
    private const string DefaultRestartScenePath = "res://scenes/Domitory/dormitory.tscn";

    private Label _endingTitleLabel;
    private Label _endingDescriptionLabel;
    private Label _statsLabel;

    public override void _Ready()
    {
        _endingTitleLabel = GetNodeOrNull<Label>("CenterContainer/EndingPanel/MarginContainer/ContentContainer/EndingTitle");
        _endingDescriptionLabel = GetNodeOrNull<Label>("CenterContainer/EndingPanel/MarginContainer/ContentContainer/EndingDescription");
        _statsLabel = GetNodeOrNull<Label>("CenterContainer/EndingPanel/MarginContainer/ContentContainer/FinalStats");

        var restartButton = GetNodeOrNull<Button>("CenterContainer/EndingPanel/MarginContainer/ContentContainer/ButtonRow/PlayAgainButton");
        var quitButton = GetNodeOrNull<Button>("CenterContainer/EndingPanel/MarginContainer/ContentContainer/ButtonRow/QuitButton");

        if (_endingTitleLabel == null || _endingDescriptionLabel == null || _statsLabel == null)
            GD.PrintErr("[EndingScreen] One or more ending labels are missing after UI refactor.");

        if (restartButton == null)
            GD.PrintErr("[EndingScreen] Play Again button is missing.");
        else
            restartButton.Pressed += OnPlayAgainPressed;

        if (quitButton == null)
            GD.PrintErr("[EndingScreen] Quit button is missing.");
        else
            quitButton.Pressed += OnQuitPressed;

        PopulateEnding();
    }

    private void PopulateEnding()
    {
        var state = GlobalVars.Instance;
        if (state == null)
        {
            GD.PrintErr("[EndingScreen] GlobalVars.Instance is null.");
            return;
        }

        var profile = state.Profile;
        var attributes = state.Attributes;
        var ending = GetEnding(state);

        GD.Print($"[EndingScreen] Ending selected: {ending.title} ({ending.reason})");

        if (_endingTitleLabel != null)
            _endingTitleLabel.Text = ending.title;

        if (_endingDescriptionLabel != null)
            _endingDescriptionLabel.Text = ending.description;

        if (_statsLabel != null)
        {
            _statsLabel.Text =
                $"Final Day: {profile.CurrentDay}\n" +
                $"Career Readiness: {profile.CareerReadiness}/100\n" +
                $"Knowledge: {attributes.Knowledge}/100\n" +
                $"Confidence: {attributes.Confidence}/100\n" +
                $"Networking: {attributes.Networking}/100\n" +
                $"Portfolio: {attributes.Portfolio}/100\n" +
                $"Focus: {attributes.Focus}/100\n" +
                $"Energy: {attributes.Energy}/100\n" +
                $"Party Networking Unlocked: {(state.PartyNetworkingUnlocked ? "Yes" : "No")}\n" +
                $"Party Attended: {(state.PartyAttended ? "Yes" : "No")}\n" +
                $"IT Ball Attended: {(state.ITBallAttended ? "Yes" : "No")}\n" +
                $"Joined Developer Group: {(state.JoinedDeveloperGroup ? "Yes" : "No")}";
        }
    }

    private static (string title, string description, string reason) GetEnding(GlobalVars state)
    {
        int careerReadiness = state.Profile.CareerReadiness;
        int knowledgeOrCoding = state.Attributes.Knowledge;
        int confidence = state.Attributes.Confidence;
        int networking = state.Attributes.Networking;
        int portfolio = state.Attributes.Portfolio;

        bool financialFreedom = state.JoinedDeveloperGroup
            && portfolio >= 70
            && networking >= 60
            && confidence >= 60
            && knowledgeOrCoding >= 70;

        if (financialFreedom)
        {
            return (
                "Financial Freedom Ending",
                "You built strong skills and connections, joined a developer group, and turned your final semester into the start of financial independence.",
                "Matched Financial Freedom thresholds");
        }

        bool goodJobBase = careerReadiness >= 70
            && knowledgeOrCoding >= 60
            && confidence >= 50
            && networking >= 30;

        bool goodJobReferral = state.PartyNetworkingUnlocked
            && careerReadiness >= 60
            && knowledgeOrCoding >= 55
            && confidence >= 45
            && networking >= 20;

        if (goodJobBase || goodJobReferral)
        {
            return (
                "Good Job Ending",
                "You rebuilt your confidence, improved your skills, and used your network to secure a graduate IT role.",
                goodJobBase ? "Matched base Good Job thresholds" : "Matched referral Good Job thresholds");
        }

        return (
            "Financial Hardship Ending",
            "Graduation arrived before you were fully ready. You still have a path forward, but it will take rebuilding step by step.",
            "Did not match Financial Freedom or Good Job thresholds");
    }

    private void OnPlayAgainPressed()
    {
        GD.Print("[EndingScreen] Play Again pressed.");

        var state = GlobalVars.Instance;
        if (state == null)
        {
            GD.PrintErr("[EndingScreen] Cannot restart because GlobalVars.Instance is null.");
            return;
        }

        state.ResetGame();
        var restartScene = string.IsNullOrWhiteSpace(state.IntroScenePath) ? DefaultRestartScenePath : state.IntroScenePath;
        TransitionManager.Instance?.ChangeSceneToFileWithFade(restartScene);
    }

    private void OnQuitPressed()
    {
        GD.Print("[EndingScreen] Quit pressed.");
        GetTree().Quit();
    }
}
