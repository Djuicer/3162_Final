using Godot;
using System;
using System.Collections.Generic;

public partial class ScreenController : Node2D
{
	private const int GraduationDay = 7;
	private const int FocusBlocksPerDay = 3;
	private const int GoodEndingPortfolio = 80;
	private const int GoodEndingCoding = 12;
	private const int GoodEndingConfidence = 9;

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

	private Panel dialoguePanel;
	private Label dialogueSpeakerLabel;
	private Label dialogueTextLabel;
	private Button dialogueContinueButton;

	private Panel minigamePanel;
	private Label minigameQuestionLabel;
	private Button[] answerButtons;

	private Panel endingPanel;
	private Label endingTitleLabel;
	private Label endingBodyLabel;

	private int currentSlot = -1;
	private string playerName = "User";
	private int currentDay = 1;
	private int focusBlocks = 0;
	private int knowledge = 1;
	private int codingSkill = 1;
	private int energy = 5;
	private int confidence = 2;
	private int portfolioProgress = 0;
	private bool hasStartedLaptopStory = false;

	private bool isGameOver = false;
	private bool minigameUsedToday = false;

	private QuestionData currentQuestion;
	private readonly Random random = new Random();
	private Queue<DialogueLine> dialogueQueue = new Queue<DialogueLine>();

	private readonly List<QuestionData> questions = new List<QuestionData>
	{
		new QuestionData(
			"Ticket #113: Why does this print 6?\nint x = 1;\nfor (int i = 0; i < 3; i++) x += i;",
			new [] { "Loop adds 0 + 1 + 2", "i starts from 1", "x resets every loop" },
			0),
		new QuestionData(
			"Ticket #207: Which fix avoids null crash for saveData[\"email\"]?",
			new [] { "Check ContainsKey first", "Cast straight to string", "Use a busy loop" },
			0),
		new QuestionData(
			"Ticket #341: Best structure for fast studentId lookup?",
			new [] { "Dictionary", "List", "Queue" },
			0)
	};

	public override void _Ready()
	{
		GetNodes();
		BuildGameUi();
		LoadData();
		RefreshUi();
		StartOpeningDialogue();
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

	private void BuildGameUi()
	{
		var uiLayer = new CanvasLayer { Name = "ComputerGameplayUI" };
		AddChild(uiLayer);

		BuildStatsPanel(uiLayer);
		BuildLogPanel(uiLayer);
		BuildDialoguePanel(uiLayer);
		BuildMinigamePanel(uiLayer);
		BuildEndingPanel(uiLayer);
	}

	private void BuildStatsPanel(CanvasLayer uiLayer)
	{
		var statsPanel = new Panel
		{
			Position = new Vector2(850, 34),
			Size = new Vector2(290, 250),
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		uiLayer.AddChild(statsPanel);

		var title = new Label
		{
			Text = "COMEBACK DASHBOARD",
			Position = new Vector2(10, 8),
			Size = new Vector2(270, 20),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		statsPanel.AddChild(title);

		var statsBox = new VBoxContainer
		{
			Position = new Vector2(12, 34),
			Size = new Vector2(260, 200)
		};
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
	}

	private void BuildLogPanel(CanvasLayer uiLayer)
	{
		var logPanel = new Panel
		{
			Position = new Vector2(120, 420),
			Size = new Vector2(760, 200),
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		uiLayer.AddChild(logPanel);

		var logTitle = new Label
		{
			Text = "Daily Feed",
			Position = new Vector2(12, 8),
			Size = new Vector2(120, 20),
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		logPanel.AddChild(logTitle);

		logLabel = new Label
		{
			Position = new Vector2(12, 32),
			Size = new Vector2(735, 155),
			AutowrapMode = TextServer.AutowrapMode.WordSmart,
			VerticalAlignment = VerticalAlignment.Top,
			MouseFilter = Control.MouseFilterEnum.Ignore
		};
		logPanel.AddChild(logLabel);
	}

	private void BuildDialoguePanel(CanvasLayer uiLayer)
	{
		dialoguePanel = new Panel
		{
			Position = new Vector2(180, 60),
			Size = new Vector2(760, 180),
			Visible = false
		};
		uiLayer.AddChild(dialoguePanel);

		dialogueSpeakerLabel = new Label
		{
			Position = new Vector2(15, 10),
			Size = new Vector2(180, 22),
			Text = playerName
		};
		dialoguePanel.AddChild(dialogueSpeakerLabel);

		dialogueTextLabel = new Label
		{
			Position = new Vector2(15, 40),
			Size = new Vector2(730, 95),
			AutowrapMode = TextServer.AutowrapMode.WordSmart
		};
		dialoguePanel.AddChild(dialogueTextLabel);

		dialogueContinueButton = new Button
		{
			Text = "Continue",
			Position = new Vector2(620, 142),
			Size = new Vector2(120, 28)
		};
		dialogueContinueButton.Pressed += OnDialogueContinuePressed;
		dialoguePanel.AddChild(dialogueContinueButton);
	}

	private void BuildMinigamePanel(CanvasLayer uiLayer)
	{
		minigamePanel = new Panel
		{
			Position = new Vector2(240, 80),
			Size = new Vector2(700, 290),
			Visible = false
		};
		uiLayer.AddChild(minigamePanel);

		var questionTitle = new Label
		{
			Text = "Minigame: Bug Fix Sprint",
			Position = new Vector2(15, 10),
			Size = new Vector2(670, 24),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		minigamePanel.AddChild(questionTitle);

		minigameQuestionLabel = new Label
		{
			Position = new Vector2(15, 42),
			Size = new Vector2(670, 90),
			AutowrapMode = TextServer.AutowrapMode.WordSmart
		};
		minigamePanel.AddChild(minigameQuestionLabel);

		answerButtons = new Button[3];
		for (int i = 0; i < answerButtons.Length; i++)
		{
			var button = new Button
			{
				Position = new Vector2(15, 145 + i * 42),
				Size = new Vector2(670, 35)
			};
			int answerIndex = i;
			button.Pressed += () => OnAnswerSelected(answerIndex);
			answerButtons[i] = button;
			minigamePanel.AddChild(button);
		}
	}

	private void BuildEndingPanel(CanvasLayer uiLayer)
	{
		endingPanel = new Panel
		{
			Position = new Vector2(250, 90),
			Size = new Vector2(650, 260),
			Visible = false
		};
		uiLayer.AddChild(endingPanel);

		endingTitleLabel = new Label
		{
			Position = new Vector2(20, 16),
			Size = new Vector2(610, 34),
			HorizontalAlignment = HorizontalAlignment.Center
		};
		endingPanel.AddChild(endingTitleLabel);

		endingBodyLabel = new Label
		{
			Position = new Vector2(20, 58),
			Size = new Vector2(610, 180),
			AutowrapMode = TextServer.AutowrapMode.WordSmart
		};
		endingPanel.AddChild(endingBodyLabel);
	}

	private void StartOpeningDialogue()
	{
		if (hasStartedLaptopStory)
		{
			AddLog("Back on your laptop. Keep pushing toward graduation.");
			return;
		}

		hasStartedLaptopStory = true;
		QueueDialogue(
			new DialogueLine(playerName, "This is my final chance before graduation."),
			new DialogueLine(playerName, "If I stay disciplined on this laptop every day, I can still make a comeback."),
			new DialogueLine("System", "Use desktop apps to train skills, build portfolio, and clear coding tickets.")
		);
		ShowNextDialogueLine();
	}

	private void OnDialogueContinuePressed()
	{
		ShowNextDialogueLine();
	}

	private void QueueDialogue(params DialogueLine[] lines)
	{
		foreach (var line in lines)
		{
			dialogueQueue.Enqueue(line);
		}
	}

	private void ShowNextDialogueLine()
	{
		if (dialogueQueue.Count == 0)
		{
			dialoguePanel.Visible = false;
			SetMainButtonsDisabled(false);
			return;
		}

		var line = dialogueQueue.Dequeue();
		dialogueSpeakerLabel.Text = line.Speaker;
		dialogueTextLabel.Text = line.Text;
		dialoguePanel.Visible = true;
		SetMainButtonsDisabled(true);
	}

	private void LoadData()
	{
		if (GlobalVars.Instance == null || SaveManager.Instance == null)
		{
			GD.PrintErr("GlobalVars or SaveManager singleton missing.");
			return;
		}

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
		if (saveData.ContainsKey("laptop_intro_seen")) hasStartedLaptopStory = saveData["laptop_intro_seen"].AsBool();
	}

	private void SaveData()
	{
		if (currentSlot < 0 || SaveManager.Instance == null)
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
		saveData["laptop_intro_seen"] = hasStartedLaptopStory;
		saveData["scene_path"] = "res://scenes/Screen/screen.tscn";
		SaveManager.Instance.SaveGame(currentSlot, saveData);
	}

	private void OnStudyPressed()
	{
		if (!TrySpendEnergy(1, "Too tired for coding drills.")) return;

		knowledge += 2;
		codingSkill += 2;
		confidence += 1;
		AdvanceTime();
		AddLog("You finished focused algorithm practice.");

		if (codingSkill >= 8 && confidence >= 6)
		{
			QueueDialogue(new DialogueLine("System", "Momentum unlocked: you are now solving medium-level questions consistently."));
			ShowNextDialogueLine();
		}

		RefreshUi();
	}

	private void OnPortfolioPressed()
	{
		if (!TrySpendEnergy(2, "You need more energy to ship portfolio features.")) return;

		int gain = 7 + codingSkill / 2 + knowledge / 4;
		portfolioProgress = Mathf.Clamp(portfolioProgress + gain, 0, 100);
		confidence += 1;
		AdvanceTime();
		AddLog($"You pushed a project update (+{gain}% portfolio). Recruiters can now see clear progress.");
		RefreshUi();
	}

	private void OnNetworkPressed()
	{
		int roll = random.Next(0, 3);
		if (roll == 0)
		{
			knowledge += 1;
			confidence += 2;
			AddLog("You found a detailed internship prep roadmap from seniors.");
		}
		else if (roll == 1)
		{
			confidence = Mathf.Max(0, confidence - 1);
			AddLog("You compared yourself to others on social media. Slight confidence dip.");
		}
		else
		{
			codingSkill += 1;
			knowledge += 1;
			AddLog("You learned practical backend tips from a short technical blog.");
		}

		AdvanceTime();
		RefreshUi();
	}

	private void OnMinigamePressed()
	{
		if (minigameUsedToday)
		{
			AddLog("Bug Fix Sprint is available once per day. Try again next day.");
			return;
		}

		if (!TrySpendEnergy(1, "Need at least 1 energy for coding ticket simulation.")) return;

		currentQuestion = questions[random.Next(questions.Count)];
		minigameQuestionLabel.Text = currentQuestion.Question;
		for (int i = 0; i < answerButtons.Length; i++)
		{
			answerButtons[i].Text = currentQuestion.Options[i];
		}

		minigameUsedToday = true;
		minigamePanel.Visible = true;
		SetMainButtonsDisabled(true);
	}

	private void OnAnswerSelected(int answerIndex)
	{
		bool correct = answerIndex == currentQuestion.CorrectIndex;
		if (correct)
		{
			codingSkill += 3;
			confidence += 2;
			portfolioProgress = Mathf.Clamp(portfolioProgress + 8, 0, 100);
			AddLog("Great patch! Your solution passed review and boosted portfolio reputation.");
			QueueDialogue(new DialogueLine("Recruiter Email", "Thanks for the clean fix. We'd like to keep an eye on your work."));
		}
		else
		{
			knowledge += 1;
			confidence = Mathf.Max(0, confidence - 1);
			AddLog("Patch rejected, but you reviewed the postmortem and improved your understanding.");
		}

		AdvanceTime();
		minigamePanel.Visible = false;
		SetMainButtonsDisabled(false);
		RefreshUi();

		if (dialogueQueue.Count > 0)
		{
			ShowNextDialogueLine();
		}
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
		if (focusBlocks < FocusBlocksPerDay)
		{
			return;
		}

		focusBlocks = 0;
		currentDay++;
		energy = Mathf.Min(8, energy + 3);
		minigameUsedToday = false;
		AddLog("Day ended. You recovered some energy overnight.");

		if (currentDay > GraduationDay)
		{
			FinishRun();
		}
	}

	private void FinishRun()
	{
		isGameOver = true;
		SetMainButtonsDisabled(true);
		endingPanel.Visible = true;

		bool goodEnding = portfolioProgress >= GoodEndingPortfolio
			&& codingSkill >= GoodEndingCoding
			&& confidence >= GoodEndingConfidence;

		if (goodEnding)
		{
			endingTitleLabel.Text = "Ending A: Comeback Complete";
			endingBodyLabel.Text =
				$"You graduate with a strong story: portfolio {portfolioProgress}%, coding {codingSkill}, confidence {confidence}. " +
				"You secure interviews and finally feel ready for a good software job.";
			AddLog("Ending reached: Comeback Complete.");
		}
		else
		{
			endingTitleLabel.Text = "Ending B: Not Yet";
			endingBodyLabel.Text =
				$"You made progress (portfolio {portfolioProgress}%, coding {codingSkill}, confidence {confidence}), " +
				"but the comeback wasn't enough before graduation. The next cycle starts now.";
			AddLog("Ending reached: Not Yet.");
		}

		QueueDialogue(new DialogueLine("System", "Run finished. Press Exit to return and start again from a save slot."));
		ShowNextDialogueLine();
	}

	private void SetMainButtonsDisabled(bool disabled)
	{
		bool shouldDisable = disabled || isGameOver;
		thisPcButton.Disabled = shouldDisable;
		fileButton.Disabled = shouldDisable;
		browserButton.Disabled = shouldDisable;
		emailButton.Disabled = shouldDisable;
	}

	private void RefreshUi()
	{
		dayLabel.Text = $"{playerName} - Day {Mathf.Min(currentDay, GraduationDay)}/{GraduationDay}";
		knowledgeLabel.Text = $"Knowledge: {knowledge}";
		codingSkillLabel.Text = $"Coding Skill: {codingSkill}";
		energyLabel.Text = $"Energy: {energy}   Sessions: {focusBlocks}/{FocusBlocksPerDay}";
		confidenceLabel.Text = $"Confidence: {confidence}";
		portfolioLabel.Text = $"Portfolio: {portfolioProgress}%";
		SaveData();
	}

	private void AddLog(string message)
	{
		if (logLabel == null)
		{
			return;
		}

		logLabel.Text = $"• {message}\n{logLabel.Text}";
	}

	private readonly struct DialogueLine
	{
		public DialogueLine(string speaker, string text)
		{
			Speaker = speaker;
			Text = text;
		}

		public string Speaker { get; }
		public string Text { get; }
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
