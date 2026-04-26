using Godot;
using System.Collections.Generic;

public partial class Dormitory : Node2D
{
	[Export] private float fadeDuration = 1.0f;

	private int currentSlot = -1;

	private string playerName = "User";
	private int currentDay = 1;
	private int knowledge = 1;
	private int codingSkill = 1;
	private int energy = 5;
	private int confidence = 2;
	private int portfolioProgress = 0;
	private bool hasSeenOpening = false;
	private string currentStoryNode = "start";
	private bool endingReached = false;

	private ColorRect fadeOverlay;

	private CanvasLayer openingUI;
	private Panel dialoguePanel;
	private Panel attributePanel;
	private VBoxContainer choicesContainer;

	private Label speakerLabel;
	private Label dialogueLabel;
	private Button continueButton;
	private Label choicesHeaderLabel;
	private Label endingLabel;

	private Label dayLabel;
	private Label knowledgeLabel;
	private Label codingSkillLabel;
	private Label energyLabel;
	private Label confidenceLabel;
	private Label portfolioLabel;

	private TextureButton computerButton;
	private TextureButton outsideButton;

	private int dialogueIndex = 0;
	private List<(string speaker, string text)> openingDialogue;
	private Dictionary<string, StoryNode> storyNodes;

	public override void _Ready()
	{
		GetNodes();
		SetupInitialState();
		LoadGameData();
		BuildOpeningDialogue();
		BuildStoryNodes();
		UpdateAttributePanel();
		StartFadeIn();
	}

	private void GetNodes()
	{
		fadeOverlay = GetNodeOrNull<ColorRect>("FadeOverlay");

		openingUI = GetNodeOrNull<CanvasLayer>("OpeningUI");
		dialoguePanel = GetNodeOrNull<Panel>("OpeningUI/DialoguePanel");
		attributePanel = GetNodeOrNull<Panel>("OpeningUI/AttributePanel");
		choicesContainer = GetNodeOrNull<VBoxContainer>("OpeningUI/StoryChoicesPanel/MarginContainer/VBoxContainer");

		speakerLabel = GetNodeOrNull<Label>("OpeningUI/DialoguePanel/SpeakerLabel");
		dialogueLabel = GetNodeOrNull<Label>("OpeningUI/DialoguePanel/DialogueLabel");
		continueButton = GetNodeOrNull<Button>("OpeningUI/DialoguePanel/ContinueButton");
		choicesHeaderLabel = GetNodeOrNull<Label>("OpeningUI/StoryChoicesPanel/MarginContainer/VBoxContainer/ChoicesHeaderLabel");
		endingLabel = GetNodeOrNull<Label>("OpeningUI/StoryChoicesPanel/MarginContainer/VBoxContainer/EndingLabel");

		dayLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/DayLabel");
		knowledgeLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/KnowledgeLabel");
		codingSkillLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/CodingSkillLabel");
		energyLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/EnergyLabel");
		confidenceLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/ConfidenceLabel");
		portfolioLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/PortfolioLabel");

		computerButton = GetNodeOrNull<TextureButton>("Button/ComputerButton");
		outsideButton = GetNodeOrNull<TextureButton>("Button/OutsideButton");

		if (continueButton != null)
			continueButton.Pressed += OnContinuePressed;
	}

	private void SetupInitialState()
	{
		openingUI.Visible = false;
		dialoguePanel.Visible = false;
		attributePanel.Visible = false;

		if (computerButton != null) computerButton.Disabled = true;
		if (outsideButton != null) outsideButton.Disabled = true;

		fadeOverlay.Visible = true;
		fadeOverlay.Modulate = new Color(1, 1, 1, 1);
	}

	private void LoadGameData()
	{
		currentSlot = GlobalVars.Instance.CurrentSlot;

		if (currentSlot < 0)
			return;

		var saveData = SaveManager.Instance.LoadGame(currentSlot);
		if (saveData.Count == 0)
			return;

		if (saveData.ContainsKey("player_name"))
			playerName = saveData["player_name"].AsString();
		if (saveData.ContainsKey("current_day"))
			currentDay = saveData["current_day"].AsInt32();
		if (saveData.ContainsKey("knowledge"))
			knowledge = saveData["knowledge"].AsInt32();
		if (saveData.ContainsKey("coding_skill"))
			codingSkill = saveData["coding_skill"].AsInt32();
		if (saveData.ContainsKey("energy"))
			energy = saveData["energy"].AsInt32();
		if (saveData.ContainsKey("confidence"))
			confidence = saveData["confidence"].AsInt32();
		if (saveData.ContainsKey("portfolio_progress"))
			portfolioProgress = saveData["portfolio_progress"].AsInt32();
		if (saveData.ContainsKey("has_seen_opening"))
			hasSeenOpening = saveData["has_seen_opening"].AsBool();
		if (saveData.ContainsKey("story_node"))
			currentStoryNode = saveData["story_node"].AsString();
		if (saveData.ContainsKey("ending_reached"))
			endingReached = saveData["ending_reached"].AsBool();
	}

	private void BuildOpeningDialogue()
	{
		openingDialogue = new List<(string speaker, string text)>
		{
			(playerName, "Third year already... and I still feel behind."),
			(playerName, "If I keep drifting, graduation will come before I am ready."),
			(playerName, "I need to make a comeback this semester."),
			(playerName, "Every day from now on has to count.")
		};
	}

	private void BuildStoryNodes()
	{
		storyNodes = new Dictionary<string, StoryNode>
		{
			{
				"start",
				new StoryNode(
					"Monday 8:00 PM. Midterm results just came out. Your advisor says this week can still define your future.",
					new List<StoryChoice>
					{
						new StoryChoice("Lock in and review weak topics", "study_path", 2, 1, -1, 0, 1),
						new StoryChoice("Apply to internships immediately", "network_path", 0, 1, -1, 2, 5),
						new StoryChoice("Ignore stress and play games all night", "burnout_warning", -1, 0, 1, -1, 0)
					})
			},
			{
				"study_path",
				new StoryNode(
					"You focus on algorithms and systems. The concepts finally start to click.",
					new List<StoryChoice>
					{
						new StoryChoice("Build a mini full-stack side project", "project_push", 1, 2, -2, 1, 20),
						new StoryChoice("Join TA office hours to ask for mentorship", "mentor_path", 1, 1, -1, 2, 5)
					})
			},
			{
				"network_path",
				new StoryNode(
					"At the career center, you realize your resume is too thin but your communication is improving.",
					new List<StoryChoice>
					{
						new StoryChoice("Polish resume and message alumni", "mentor_path", 1, 1, -1, 2, 10),
						new StoryChoice("Attend hackathon this weekend", "project_push", 1, 2, -2, 1, 15)
					})
			},
			{
				"burnout_warning",
				new StoryNode(
					"You wake up feeling worse. Panic creeps in as deadlines stack up.",
					new List<StoryChoice>
					{
						new StoryChoice("Reset: one focused Pomodoro block", "study_path", 1, 1, -1, 1, 5),
						new StoryChoice("Ask a friend for accountability", "mentor_path", 0, 1, 0, 2, 5)
					})
			},
			{
				"mentor_path",
				new StoryNode(
					"Your mentor gives blunt advice: 'Companies hire evidence, not promises.'",
					new List<StoryChoice>
					{
						new StoryChoice("Ship your project MVP before Friday", "finale", 1, 2, -2, 1, 20),
						new StoryChoice("Do mock interviews and DSA review", "finale", 2, 1, -1, 1, 10)
					})
			},
			{
				"project_push",
				new StoryNode(
					"You pull a few late nights and make visible progress. Recruiters now have something concrete to evaluate.",
					new List<StoryChoice>
					{
						new StoryChoice("Publish demo and write technical blog", "finale", 1, 2, -1, 1, 25),
						new StoryChoice("Practice interview storytelling", "finale", 1, 1, -1, 2, 10)
					})
			},
			{
				"finale",
				new StoryNode(
					"Friday evening. You send your applications and wait for responses.",
					new List<StoryChoice>
					{
						new StoryChoice("See outcome", "ending", 0, 0, 0, 0, 0)
					})
			}
		};
	}

	private void StartFadeIn()
	{
		Tween tween = CreateTween();
		tween.TweenProperty(fadeOverlay, "modulate:a", 0.0f, fadeDuration);
		tween.TweenCallback(Callable.From(() =>
		{
			fadeOverlay.Visible = false;
			if (hasSeenOpening)
				FinishOpeningScene();
			else
				StartOpeningDialogue();
		}));
	}

	private void StartOpeningDialogue()
	{
		openingUI.Visible = true;
		dialoguePanel.Visible = true;
		attributePanel.Visible = false;

		dialogueIndex = 0;
		ShowDialogueLine();
	}

	private void ShowDialogueLine()
	{
		if (dialogueIndex >= openingDialogue.Count)
		{
			MarkOpeningAsSeen();
			FinishOpeningScene();
			return;
		}

		var line = openingDialogue[dialogueIndex];
		speakerLabel.Text = line.speaker;
		dialogueLabel.Text = line.text;
	}

	private void OnContinuePressed()
	{
		dialogueIndex++;
		ShowDialogueLine();
	}

	private void FinishOpeningScene()
	{
		openingUI.Visible = true;
		dialoguePanel.Visible = false;
		attributePanel.Visible = true;

		UpdateAttributePanel();
		RenderCurrentStoryNode();

		if (computerButton != null) computerButton.Disabled = false;
		if (outsideButton != null) outsideButton.Disabled = false;
	}

	private void RenderCurrentStoryNode()
	{
		if (choicesContainer == null || dialogueLabel == null)
			return;

		ClearChoiceButtons();
		endingLabel.Visible = false;

		if (endingReached || currentStoryNode == "ending")
		{
			ShowEnding();
			return;
		}

		if (!storyNodes.ContainsKey(currentStoryNode))
			currentStoryNode = "start";

		StoryNode node = storyNodes[currentStoryNode];
		dialogueLabel.Text = node.Text;
		speakerLabel.Text = playerName;

		if (choicesHeaderLabel != null)
			choicesHeaderLabel.Text = "Choose your next move:";

		foreach (var choice in node.Choices)
		{
			Button choiceButton = new Button();
			choiceButton.Text = choice.Text;
			choiceButton.AutowrapMode = TextServer.AutowrapMode.WordSmart;
			choiceButton.CustomMinimumSize = new Vector2(0, 42);
			choiceButton.Pressed += () => ApplyChoice(choice);
			choicesContainer.AddChild(choiceButton);
		}
	}

	private void ApplyChoice(StoryChoice choice)
	{
		knowledge = Mathf.Max(0, knowledge + choice.KnowledgeDelta);
		codingSkill = Mathf.Max(0, codingSkill + choice.CodingDelta);
		energy = Mathf.Clamp(energy + choice.EnergyDelta, 0, 10);
		confidence = Mathf.Max(0, confidence + choice.ConfidenceDelta);
		portfolioProgress = Mathf.Clamp(portfolioProgress + choice.PortfolioDelta, 0, 100);
		currentDay = Mathf.Min(currentDay + 1, 7);
		currentStoryNode = choice.NextNode;

		if (currentStoryNode == "ending")
			endingReached = true;

		UpdateAttributePanel();
		PersistProgress();
		RenderCurrentStoryNode();
	}

	private void ShowEnding()
	{
		string endingText;
		int readinessScore = knowledge + codingSkill + confidence + (portfolioProgress / 10);

		if (portfolioProgress >= 70 && readinessScore >= 18)
		{
			endingText = "Ending: Comeback Complete. You secure a strong internship-to-full-time track before graduation.";
		}
		else if (portfolioProgress >= 40 && readinessScore >= 12)
		{
			endingText = "Ending: Steady Recovery. You earn interviews and a realistic path to a good job after graduation.";
		}
		else
		{
			endingText = "Ending: Not Over Yet. This semester was rough, but you now know exactly what to fix before graduating.";
		}

		dialogueLabel.Text = "Results week is here. Your efforts shape the outcome.";
		speakerLabel.Text = "Narrator";
		if (choicesHeaderLabel != null)
			choicesHeaderLabel.Text = "Final outcome";

		endingLabel.Visible = true;
		endingLabel.Text = endingText;

		Button restartButton = new Button();
		restartButton.Text = "Start New Comeback Run";
		restartButton.CustomMinimumSize = new Vector2(0, 42);
		restartButton.Pressed += RestartRun;
		choicesContainer.AddChild(restartButton);
	}

	private void RestartRun()
	{
		currentDay = 1;
		knowledge = 1;
		codingSkill = 1;
		energy = 5;
		confidence = 2;
		portfolioProgress = 0;
		currentStoryNode = "start";
		endingReached = false;

		UpdateAttributePanel();
		PersistProgress();
		RenderCurrentStoryNode();
	}

	private void ClearChoiceButtons()
	{
		if (choicesContainer == null)
			return;

		for (int i = choicesContainer.GetChildCount() - 1; i >= 0; i--)
		{
			Node child = choicesContainer.GetChild(i);
			if (child is Button && child.Name != "ChoicesHeaderLabel")
				child.QueueFree();
		}
	}

	private void UpdateAttributePanel()
	{
		dayLabel.Text = $"Day {currentDay} / 7";
		knowledgeLabel.Text = $"Knowledge: {knowledge}";
		codingSkillLabel.Text = $"Coding Skill: {codingSkill}";
		energyLabel.Text = $"Energy: {energy}";
		confidenceLabel.Text = $"Confidence: {confidence}";
		portfolioLabel.Text = $"Portfolio: {portfolioProgress}%";
	}

	private void MarkOpeningAsSeen()
	{
		hasSeenOpening = true;
		PersistProgress();
	}

	private void PersistProgress()
	{
		if (currentSlot < 0)
			return;

		var saveData = SaveManager.Instance.LoadGame(currentSlot);
		saveData["player_name"] = playerName;
		saveData["current_day"] = currentDay;
		saveData["knowledge"] = knowledge;
		saveData["coding_skill"] = codingSkill;
		saveData["energy"] = energy;
		saveData["confidence"] = confidence;
		saveData["portfolio_progress"] = portfolioProgress;
		saveData["has_seen_opening"] = hasSeenOpening;
		saveData["story_node"] = currentStoryNode;
		saveData["ending_reached"] = endingReached;
		saveData["scene_path"] = "res://scenes/Domitory/dormitory.tscn";

		SaveManager.Instance.SaveGame(currentSlot, saveData);
	}

	private class StoryNode
	{
		public string Text { get; }
		public List<StoryChoice> Choices { get; }

		public StoryNode(string text, List<StoryChoice> choices)
		{
			Text = text;
			Choices = choices;
		}
	}

	private class StoryChoice
	{
		public string Text { get; }
		public string NextNode { get; }
		public int KnowledgeDelta { get; }
		public int CodingDelta { get; }
		public int EnergyDelta { get; }
		public int ConfidenceDelta { get; }
		public int PortfolioDelta { get; }

		public StoryChoice(string text, string nextNode, int knowledgeDelta, int codingDelta, int energyDelta, int confidenceDelta, int portfolioDelta)
		{
			Text = text;
			NextNode = nextNode;
			KnowledgeDelta = knowledgeDelta;
			CodingDelta = codingDelta;
			EnergyDelta = energyDelta;
			ConfidenceDelta = confidenceDelta;
			PortfolioDelta = portfolioDelta;
		}
	}
}
