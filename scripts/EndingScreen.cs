using Godot;
using System;

public partial class EndingScreen : Control
{
    private Label _endingTitleLabel;
    private Label _endingDescriptionLabel;
    private Label _statsLabel;

    public override void _Ready()
    {
        _endingTitleLabel = GetNode<Label>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/EndingTitle");
        _endingDescriptionLabel = GetNode<Label>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/EndingDescription");
        _statsLabel = GetNode<Label>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/FinalStats");

        var restartButton = GetNode<Button>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/ButtonRow/PlayAgainButton");
        restartButton.Pressed += OnPlayAgainPressed;

        var quitButton = GetNode<Button>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/ButtonRow/QuitButton");
        quitButton.Pressed += OnQuitPressed;

        PopulateEnding();
    }

    private void PopulateEnding()
    {
        var state = GlobalVars.Instance;
        var profile = state.Profile;
        var attributes = state.Attributes;

        var ending = GetEnding(state);

        GD.Print(
            $"[EndingCalc] Day={profile.CurrentDay}, CareerReadiness={profile.CareerReadiness}, " +
            $"Knowledge={attributes.Knowledge}, Confidence={attributes.Confidence}, Networking={attributes.Networking}, " +
            $"Portfolio={attributes.Portfolio}, Focus={attributes.Focus}, Energy={attributes.Energy}, " +
            $"PartyNetworkingUnlocked={state.PartyNetworkingUnlocked}, ITBallAttended={state.ITBallAttended}, " +
            $"JoinedDeveloperGroup={state.JoinedDeveloperGroup}, SelectedEnding={ending.title}, Reason={ending.reason}");

        _endingTitleLabel.Text = ending.title;
        _endingDescriptionLabel.Text = ending.description;
        _statsLabel.Text =
            $"Final Day: {profile.CurrentDay}\n" +
            $"Final CareerReadiness: {profile.CareerReadiness}/100\n" +
            $"Final Knowledge: {attributes.Knowledge}/100\n" +
            $"Final Confidence: {attributes.Confidence}/100\n" +
            $"Final Networking: {attributes.Networking}/100\n" +
            $"Final Portfolio: {attributes.Portfolio}/100\n" +
            $"Final Focus: {attributes.Focus}/100\n" +
            $"Final Energy: {attributes.Energy}/100\n" +
            $"Party Networking: {(state.PartyNetworkingUnlocked ? "Yes" : "No")}\n" +
            $"IT Ball Attended: {(state.ITBallAttended ? "Yes" : "No")}\n" +
            $"Developer Group Joined: {(state.JoinedDeveloperGroup ? "Yes" : "No")}";
    }

    private static (string title, string description, string reason) GetEnding(GlobalVars state)
    {
        int careerReadiness = state.Profile.CareerReadiness;
        int knowledge = state.Attributes.Knowledge;
        int confidence = state.Attributes.Confidence;
        int networking = state.Attributes.Networking;
        int portfolio = state.Attributes.Portfolio;

        bool financialFreedom = state.JoinedDeveloperGroup
            && portfolio >= 70
            && networking >= 60
            && confidence >= 60
            && knowledge >= 70;

        if (financialFreedom)
        {
            return (
                "Financial Freedom Ending",
                "You joined a group of student developers and helped build an app that gained real users. Instead of only waiting for an opportunity, you created one. Your final semester became the start of your financial independence.",
                "JoinedDeveloperGroup + high Portfolio/Networking/Confidence/Knowledge");
        }

        bool goodJobBase = careerReadiness >= 70
            && knowledge >= 60
            && confidence >= 50
            && networking >= 30;

        bool goodJobReferral = state.PartyNetworkingUnlocked
            && careerReadiness >= 60
            && knowledge >= 55
            && confidence >= 45
            && networking >= 20;

        if (goodJobBase || goodJobReferral)
        {
            return (
                "Good Job Ending",
                "You rebuilt your confidence, improved your skills, and used your connections wisely. By graduation, you secured a graduate IT role and took your first step into the industry.",
                goodJobBase ? "Met base Good Job thresholds" : "Met referral-assisted Good Job thresholds");
        }

        return (
            "Financial Hardship Ending",
            "Graduation arrived before you were ready. Without enough preparation, confidence, or support, finding stable work became difficult. The future is uncertain, but this is not the end of the story. Rebuilding step by step is still possible.",
            "Did not meet Financial Freedom or Good Job conditions");
    }

    private void OnPlayAgainPressed()
    {
        GlobalVars.Instance.ResetGame();
        GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn");
    }

    private void OnQuitPressed()
    {
        GetTree().Quit();
    }
}
