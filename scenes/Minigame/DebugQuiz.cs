using Godot;
using System.Collections.Generic;

public partial class DebugQuiz : Control
{
    private class QuizQuestion
    {
        public string Prompt { get; set; }
        public string[] Choices { get; set; }
        public int CorrectIndex { get; set; }
    }

    private readonly List<QuizQuestion> _questions = new()
    {
        new QuizQuestion
        {
            Prompt = "What is wrong with this code?\\n\\nif (score = 100)\\n{\\n    GD.Print(\"Passed\");\\n}",
            Choices = new[] { "Use score == 100", "Use score != 100", "Use score <= 100" },
            CorrectIndex = 0
        },
        new QuizQuestion
        {
            Prompt = "Why will this crash?\\n\\nstring name = null;\\nGD.Print(name.Length);",
            Choices = new[] { "name is null before Length is read", "Length only works on arrays", "GD.Print cannot print integers" },
            CorrectIndex = 0
        },
        new QuizQuestion
        {
            Prompt = "What fixes this loop condition?\\n\\nfor (int i = 0; i > 5; i++)",
            Choices = new[] { "Change i > 5 to i < 5", "Change i++ to i--", "Start i at 10" },
            CorrectIndex = 0
        },
        new QuizQuestion
        {
            Prompt = "Which line correctly adds 1 to level?",
            Choices = new[] { "level += 1;", "level =+ 1;", "level == level + 1;" },
            CorrectIndex = 0
        },
        new QuizQuestion
        {
            Prompt = "How do you compare two strings in C# for equality?",
            Choices = new[] { "if (a == b)", "if (a = b)", "if (a.Equals = b)" },
            CorrectIndex = 0
        }
    };

    private int _currentQuestionIndex = 0;
    private int _score = 0;

    private Label _questionLabel;
    private Label _progressLabel;
    private Label _scoreLabel;
    private Button _choiceAButton;
    private Button _choiceBButton;
    private Button _choiceCButton;

    public override void _Ready()
    {
        _questionLabel = GetNode<Label>("MarginContainer/VBox/QuestionLabel");
        _progressLabel = GetNode<Label>("MarginContainer/VBox/ProgressLabel");
        _scoreLabel = GetNode<Label>("MarginContainer/VBox/ScoreLabel");

        _choiceAButton = GetNode<Button>("MarginContainer/VBox/Choices/ChoiceA");
        _choiceBButton = GetNode<Button>("MarginContainer/VBox/Choices/ChoiceB");
        _choiceCButton = GetNode<Button>("MarginContainer/VBox/Choices/ChoiceC");

        _choiceAButton.Pressed += () => OnAnswerSelected(0);
        _choiceBButton.Pressed += () => OnAnswerSelected(1);
        _choiceCButton.Pressed += () => OnAnswerSelected(2);

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        var question = _questions[_currentQuestionIndex];
        _questionLabel.Text = question.Prompt;
        _choiceAButton.Text = $"A. {question.Choices[0]}";
        _choiceBButton.Text = $"B. {question.Choices[1]}";
        _choiceCButton.Text = $"C. {question.Choices[2]}";

        _progressLabel.Text = $"Question {_currentQuestionIndex + 1}/{_questions.Count}";
        _scoreLabel.Text = $"Score: {_score}";
    }

    private void OnAnswerSelected(int selectedIndex)
    {
        if (_questions[_currentQuestionIndex].CorrectIndex == selectedIndex)
        {
            _score++;
        }

        _currentQuestionIndex++;

        if (_currentQuestionIndex >= _questions.Count)
        {
            AssessmentSession.LastScore = _score;
            AssessmentSession.TotalQuestions = _questions.Count;
            GetTree().ChangeSceneToFile("res://scenes/Ending/AssessmentEnding.tscn");
            return;
        }

        ShowQuestion();
    }
}
