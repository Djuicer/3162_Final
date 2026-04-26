using Godot;
using System;
using System.Collections.Generic;

public partial class ScreenController : Node2D
{
	private const int GraduationDay = 7;
	private const int FocusBlocksPerDay = 3;

	private TextureButton thisPcButton;
	private TextureButton fileButton;
	private TextureButton browserButton;
	private TextureButton emailButton;

	private Label dayLabel;
	private Label knowledgeLabel;
	private Label codingSkillLabel;
	private Label energyLabel;
	private Label confidenceLabel;
	private Label portfolioLabel;
	private Label logLabel;
	private Label minigameQuestionLabel;

	private Panel minigamePanel;
	private Button[] answerButtons;

	private int currentSlot = -1;
	private string playerName = "User";
	private int currentDay = 1;
	private int focusBlocks = 0;
	private int knowledge = 1;
	private int codingSkill = 1;
	private int energy = 5;
	private int confidence = 2;
	private int portfolioProgress = 0;

	private bool isGameOver = false;
	private QuestionData currentQuestion;
	private readonly Random random = new Random();

	private readonly List<QuestionData> questions = new List<QuestionData>
	{
		new QuestionData(
			"Bug Ticket #113: Why does this print 6?\\nint x = 1;\\nfor (int i = 0; i < 3; i++) x += i;",
			new [] { "Because loop adds 0 + 1 + 2", "Because i starts from 1", "Because x is reset each loop" },
			0),
		new QuestionData(
			"Bug Ticket #207: Which fix prevents null crash when reading saveData[\"email\"]?",
			new [] { "Use ContainsKey check first", "Cast to string directly", "Wrap all logic in while(true)" },
			0),
		new QuestionData(
			"Bug Ticket #341: Which data structure is best for quick key lookup by student id?",
			new [] { "Dictionary", "List", "Queue" },
			0)
	};

	public override void _Ready()
	{
		GetNodes();
		BuildComputerUi();
		LoadData();
		RefreshUi();
		AddLog("Laptop booted. You have limited days before graduation—make every session count.");
	}

	private void GetNodes()
	{
		thisPcButton = GetNode<TextureButton>("Button/ThisPCButton");
		fileButton = GetNode<TextureButton>("Button/FileButton");
		browserButton = GetNode<TextureButton>("Button/BrowserButton");
		emailButton = GetNode<TextureButton>("Button/EmailButton");

		thisPcButton.Pressed += OnStudyPressed;
		fileButton.Pressed += OnPortfolioPressed;
		browserButton.Pressed += OnNetworkPressed;
		emailButton.Pressed += OnMinigamePressed;
	}

	private void BuildComputerUi()
	{
		var uiLayer = new CanvasLayer();
		uiLayer.Name = "ComputerGameplayUI";
		AddChild(uiLayer);

		var statsPanel = new Panel();
		statsPanel.Position = new Vector2(860, 40);
		statsPanel.Size = new Vector2(270, 240);
		uiLayer.AddChild(statsPanel);

		var statsBox = new VBoxContainer();
		statsBox.Position = new Vector2(10, 10);
		statsBox.Size = new Vector2(250, 220);
		statsPanel.AddChild(statsBox);

		dayLabel = new Label();
		knowledgeLabel = new Label();
		codingSkillLabel = new Label();
		energyLabel = new Label();
		confidenceLabel = new Label();
		portfolioLabel = new Label();

		statsBox.AddChild(dayLabel);
		statsBox.AddChild(knowledgeLabel);
		statsBox.AddChild(codingSkillLabel);
		statsBox.AddChild(energyLabel);
		statsBox.AddChild(confidenceLabel);
		statsBox.AddChild(portfolioLabel);

		var logPanel = new Panel();
		logPanel.Position = new Vector2(130, 430);
		logPanel.Size = new Vector2(900, 190);
		uiLayer.AddChild(logPanel);

		logLabel = new Label();
		logLabel.Position = new Vector2(12, 12);
		logLabel.Size = new Vector2(875, 166);
		logLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		logLabel.VerticalAlignment = VerticalAlignment.Top;
		logPanel.AddChild(logLabel);

		minigamePanel = new Panel();
		minigamePanel.Position = new Vector2(270, 120);
		minigamePanel.Size = new Vector2(620, 260);
		minigamePanel.Visible = false;
		uiLayer.AddChild(minigamePanel);

		var questionTitle = new Label();
		questionTitle.Text = "Interview Prep Minigame: Bug Fix Sprint";
		questionTitle.Position = new Vector2(15, 10);
		questionTitle.Size = new Vector2(590, 24);
		minigamePanel.AddChild(questionTitle);

		minigameQuestionLabel = new Label();
		minigameQuestionLabel.Position = new Vector2(15, 42);
		minigameQuestionLabel.Size = new Vector2(590, 80);
		minigameQuestionLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
		minigamePanel.AddChild(minigameQuestionLabel);

		answerButtons = new Button[3];
		for (int i = 0; i < answerButtons.Length; i++)
		{
			var button = new Button();
			button.Position = new Vector2(15, 130 + i * 40);
			button.Size = new Vector2(590, 34);
			int answerIndex = i;
			button.Pressed += () => OnAnswerSelected(answerIndex);
			answerButtons[i] = button;
			minigamePanel.AddChild(button);
		}
	}

	private void LoadData()
	{
		currentSlot = GlobalVars.Instance.CurrentSlot;
		if (currentSlot < 0)
		{
			return;
		}

		var saveData = SaveManager.Instance.LoadGame(currentSlot);
		if (saveData.Count == 0)
		{
			return;
		}

		if (saveData.ContainsKey("player_name")) playerName = saveData["player_name"].AsString();
		if (saveData.ContainsKey("current_day")) currentDay = saveData["current_day"].AsInt32();
		if (saveData.ContainsKey("focus_blocks")) focusBlocks = saveData["focus_blocks"].AsInt32();
		if (saveData.ContainsKey("knowledge")) knowledge = saveData["knowledge"].AsInt32();
		if (saveData.ContainsKey("coding_skill")) codingSkill = saveData["coding_skill"].AsInt32();
		if (saveData.ContainsKey("energy")) energy = saveData["energy"].AsInt32();
		if (saveData.ContainsKey("confidence")) confidence = saveData["confidence"].AsInt32();
		if (saveData.ContainsKey("portfolio_progress")) portfolioProgress = saveData["portfolio_progress"].AsInt32();
	}

	private void SaveData()
	{
		if (currentSlot < 0)
		{
			return;
		}

		var saveData = SaveManager.Instance.LoadGame(currentSlot);
		saveData["player_name"] = playerName;
		saveData["current_day"] = currentDay;
		saveData["focus_blocks"] = focusBlocks;
		saveData["knowledge"] = knowledge;
		saveData["coding_skill"] = codingSkill;
		saveData["energy"] = energy;
		saveData["confidence"] = confidence;
		saveData["portfolio_progress"] = portfolioProgress;
		saveData["scene_path"] = "res://scenes/Screen/screen.tscn";
		SaveManager.Instance.SaveGame(currentSlot, saveData);
	}

	private void OnStudyPressed()
	{
		if (!TrySpendEnergy(1, "No energy left for coding drills.")) return;

		knowledge += 1;
		codingSkill += 2;
		confidence += 1;
		AdvanceTime();
		AddLog("You solved algorithm drills. Coding skill and confidence improved.");
		RefreshUi();
	}

	private void OnPortfolioPressed()
	{
		if (!TrySpendEnergy(2, "You are too exhausted to build portfolio projects.")) return;

		int gain = 8 + codingSkill / 2;
		portfolioProgress = Mathf.Clamp(portfolioProgress + gain, 0, 100);
		confidence += 1;
		AdvanceTime();
		AddLog($"You shipped a portfolio feature (+{gain}% portfolio progress).");
		RefreshUi();
	}

	private void OnNetworkPressed()
	{
		int roll = random.Next(0, 3);
		if (roll == 0)
		{
			confidence += 2;
			knowledge += 1;
			AddLog("You found a senior's internship roadmap. Motivation boosted.");
		}
		else if (roll == 1)
		{
			confidence = Mathf.Max(confidence - 1, 0);
			AddLog("You doomscrolled classmates' offers. Confidence dropped slightly.");
		}
		else
		{
			codingSkill += 1;
			knowledge += 1;
			AddLog("You watched a system design walkthrough and took notes.");
		}

		AdvanceTime();
		RefreshUi();
	}

	private void OnMinigamePressed()
	{
		if (!TrySpendEnergy(1, "You need at least 1 energy to attempt the coding ticket.")) return;

		currentQuestion = questions[random.Next(questions.Count)];
		minigameQuestionLabel.Text = currentQuestion.Question;

		for (int i = 0; i < answerButtons.Length; i++)
		{
			answerButtons[i].Text = currentQuestion.Options[i];
		}

		SetMainButtonsDisabled(true);
		minigamePanel.Visible = true;
	}

	private void OnAnswerSelected(int answerIndex)
	{
		bool correct = answerIndex == currentQuestion.CorrectIndex;
		if (correct)
		{
			codingSkill += 2;
			confidence += 2;
			portfolioProgress = Mathf.Clamp(portfolioProgress + 6, 0, 100);
			AddLog("Correct fix! Recruiter challenge cleared. Your portfolio reputation improved.");
		}
		else
		{
			confidence = Mathf.Max(confidence - 1, 0);
			knowledge += 1;
			AddLog("Not the best fix, but you learned from the failed patch review.");
		}

		AdvanceTime();
		minigamePanel.Visible = false;
		SetMainButtonsDisabled(false);
		RefreshUi();
	}

	private bool TrySpendEnergy(int cost, string failText)
	{
		if (isGameOver)
		{
			return false;
		}

		if (energy < cost)
		{
			AddLog(failText);
			RefreshUi();
			return false;
		}

		energy -= cost;
		return true;
	}

	private void AdvanceTime()
	{
		focusBlocks++;
		if (focusBlocks >= FocusBlocksPerDay)
		{
			focusBlocks = 0;
			currentDay++;
			energy = Mathf.Min(energy + 3, 8);
			AddLog("A new day starts. You rested a little and recovered energy.");
		}

		if (currentDay > GraduationDay)
		{
			FinishRun();
		}
	}

	private void FinishRun()
	{
		isGameOver = true;
		SetMainButtonsDisabled(true);

		string ending;
		if (portfolioProgress >= 80 && codingSkill >= 10 && confidence >= 8)
		{
			ending = "COMEBACK COMPLETE: You graduate with a strong portfolio and land interviews.";
		}
		else
		{
			ending = "Semester over: You improved, but need one more grind cycle for your target job.";
		}

		AddLog(ending);
	}

	private void SetMainButtonsDisabled(bool disabled)
	{
		thisPcButton.Disabled = disabled || isGameOver;
		fileButton.Disabled = disabled || isGameOver;
		browserButton.Disabled = disabled || isGameOver;
		emailButton.Disabled = disabled || isGameOver;
	}

	private void RefreshUi()
	{
		dayLabel.Text = $"{playerName}  Day {Mathf.Min(currentDay, GraduationDay)} / {GraduationDay}";
		knowledgeLabel.Text = $"Knowledge: {knowledge}";
		codingSkillLabel.Text = $"Coding Skill: {codingSkill}";
		energyLabel.Text = $"Energy: {energy} (sessions used: {focusBlocks}/{FocusBlocksPerDay})";
		confidenceLabel.Text = $"Confidence: {confidence}";
		portfolioLabel.Text = $"Portfolio: {portfolioProgress}%";
		SaveData();
	}

	private void AddLog(string message)
	{
		logLabel.Text = $"{message}\\n\\n{logLabel.Text}";
	}

	private readonly struct QuestionData
	{
		public QuestionData(string question, string[] options, int correctIndex)
		{
			Question = question;
			Options = options;
			CorrectIndex = correctIndex;
		}

		public string Question { get; }
		public string[] Options { get; }
		public int CorrectIndex { get; }
	}
}
