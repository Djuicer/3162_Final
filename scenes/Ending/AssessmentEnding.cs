using Godot;

public partial class AssessmentEnding : Control
{
    public override void _Ready()
    {
        var endingLabel = GetNode<Label>("MarginContainer/VBox/EndingLabel");
        var scoreLabel = GetNode<Label>("MarginContainer/VBox/ScoreSummaryLabel");
        var backButton = GetNode<Button>("MarginContainer/VBox/BackButton");

        var score = AssessmentSession.LastScore;
        var total = AssessmentSession.TotalQuestions;

        scoreLabel.Text = $"Assessment Score: {score}/{total}";

        if (score >= 3)
        {
            endingLabel.Text = "A few days later, an email arrives. You passed the assessment. It is not a job offer yet, but for the first time in a long while, your comeback feels real.";
        }
        else
        {
            endingLabel.Text = "The result arrives. You did not pass the assessment. But this time, you do not close the laptop and give up. You open your notes and start again.";
        }

        backButton.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn");
    }
}
