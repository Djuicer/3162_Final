using Godot;

public partial class AssessmentResult : Control
{
    public override void _Ready()
    {
        var scoreLabel = GetNode<Label>("MarginContainer/VBox/ScoreLabel");
        var programmingGainLabel = GetNode<Label>("MarginContainer/VBox/ProgrammingGainLabel");
        var confidenceGainLabel = GetNode<Label>("MarginContainer/VBox/ConfidenceGainLabel");
        var careerProgressGainLabel = GetNode<Label>("MarginContainer/VBox/CareerProgressGainLabel");
        var reflectionLabel = GetNode<Label>("MarginContainer/VBox/ReflectionLabel");
        var continueButton = GetNode<Button>("MarginContainer/VBox/ContinueButton");

        scoreLabel.Text = $"Score: {AssessmentSession.LastAssessmentScore} / {AssessmentSession.TotalQuestions}";
        programmingGainLabel.Text = $"Programming {FormatGain(AssessmentSession.LastProgrammingGain)}";
        confidenceGainLabel.Text = $"Confidence {FormatGain(AssessmentSession.LastConfidenceGain)}";
        careerProgressGainLabel.Text = $"Career Progress {FormatGain(AssessmentSession.LastCareerProgressGain)}";

        reflectionLabel.Text = GetReflectionText(AssessmentSession.LastAssessmentScore);

        continueButton.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/Ending/AssessmentEnding.tscn");
    }

    private static string FormatGain(int value)
    {
        return value >= 0 ? $"+{value}" : value.ToString();
    }

    private static string GetReflectionText(int score)
    {
        if (score == 5)
        {
            return "You caught every bug. For once, the screen does not feel like an enemy.";
        }

        if (score >= 3)
        {
            return "You fixed most of the critical bugs. Maybe you are not as far behind as you thought.";
        }

        if (score >= 1)
        {
            return "That was rough, but you can see what you need to work on now.";
        }

        return "The test ends badly. You feel the pressure, but the choice to improve is still yours.";
    }
}
