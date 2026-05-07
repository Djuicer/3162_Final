using Godot;

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

        var ending = GetEnding(profile.CareerReadiness, attributes.Knowledge, attributes.Confidence);

        _endingTitleLabel.Text = ending.title;
        _endingDescriptionLabel.Text = ending.description;
        _statsLabel.Text =
            $"Final CareerReadiness: {profile.CareerReadiness}/100\n" +
            $"Final Knowledge: {attributes.Knowledge}/100\n" +
            $"Final Confidence: {attributes.Confidence}/100\n" +
            $"Final Focus: {attributes.Focus}/100\n" +
            $"Final Energy: {attributes.Energy}/100";
    }

    private static (string title, string description) GetEnding(int careerReadiness, int knowledge, int confidence)
    {
        if (careerReadiness >= 70 && knowledge >= 60 && confidence >= 40)
        {
            return (
                "You Made a Comeback",
                "You rebuilt your confidence, improved your skills, and became ready to face life after graduation.");
        }

        if (careerReadiness >= 40)
        {
            return (
                "Progress, But Not Finished",
                "You made real progress this semester, but the comeback is still incomplete. You are better than where you started.");
        }

        return (
            "The Semester Slipped Away",
            "The final semester ended before you could fully turn things around. There is still a long road ahead.");
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
