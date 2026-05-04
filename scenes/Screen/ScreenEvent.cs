using Godot;

public partial class ScreenEvent : Node2D
{
    private const string MinigameScenePath = "res://scenes/Minigame/DebugQuiz.tscn";

    private Panel _assessmentPanel;

    public override void _Ready()
    {
        _assessmentPanel = GetNode<Panel>("AssessmentPanel");

        var startButton = GetNode<Button>("AssessmentPanel/MarginContainer/VBox/ButtonRow/StartAssessmentButton");
        var notNowButton = GetNode<Button>("AssessmentPanel/MarginContainer/VBox/ButtonRow/NotNowButton");
        var openEmailButton = GetNode<Button>("OpenEmailButton");

        startButton.Pressed += OnStartAssessmentPressed;
        notNowButton.Pressed += OnNotNowPressed;
        openEmailButton.Pressed += OnOpenEmailPressed;

        _assessmentPanel.Visible = true;
    }

    private void OnStartAssessmentPressed()
    {
        GetTree().ChangeSceneToFile(MinigameScenePath);
    }

    private void OnNotNowPressed()
    {
        _assessmentPanel.Visible = false;
    }

    private void OnOpenEmailPressed()
    {
        _assessmentPanel.Visible = true;
    }
}
