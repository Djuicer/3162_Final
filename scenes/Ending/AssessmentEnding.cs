using Godot;

public partial class AssessmentEnding : Control
{
    public override void _Ready()
    {
        var titleLabel = GetNode<Label>("MarginContainer/VBox/TitleLabel");
        var endingLabel = GetNode<Label>("MarginContainer/VBox/EndingLabel");
        var scoreLabel = GetNode<Label>("MarginContainer/VBox/ScoreSummaryLabel");
        var backButton = GetNode<Button>("MarginContainer/VBox/BackButton");

        scoreLabel.Text = $"Assessment Score: {AssessmentSession.LastAssessmentScore}/{AssessmentSession.TotalQuestions}";

        if (AssessmentSession.CareerProgress >= 3 && AssessmentSession.Confidence >= 2)
        {
            titleLabel.Text = "The Callback";
            endingLabel.Text = "A few days later, an email from BrightPath Tech arrives.\nYou passed the assessment and have been invited to an interview.\nIt is not the final victory yet, but for the first time in a long while, your comeback feels real.";
        }
        else if (AssessmentSession.Programming >= 2)
        {
            titleLabel.Text = "The Study Plan";
            endingLabel.Text = "The result arrives. You did not make it to the next stage.\nBut something is different this time.\nYou understand your mistakes, open your notes, and start building a study plan.\nThe comeback has not happened yet, but it has finally begun.";
        }
        else
        {
            titleLabel.Text = "Back to Avoiding";
            endingLabel.Text = "The assessment ends badly.\nYou stare at the screen, then close the laptop.\nThe room feels quiet again.\nMaybe tomorrow you will try. Maybe not.";
        }

        backButton.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn");
    }
}
