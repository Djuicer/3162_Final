using Godot;

public partial class ScreenEvent : Node2D
{
    private const string MinigameScenePath = "res://scenes/Minigame/DebugQuiz.tscn";

    public override void _Ready()
    {
        var startButton = GetNode<Button>("AssessmentPanel/MarginContainer/VBox/StartAssessmentButton");
        startButton.Pressed += OnStartAssessmentPressed;
    }

    private void OnStartAssessmentPressed()
    {
        GetTree().ChangeSceneToFile(MinigameScenePath);
    }
}
