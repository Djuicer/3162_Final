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
	private Panel minigamePanel;

	private Label speakerLabel;
	private Label dialogueLabel;
	private Button continueButton;
	private Label choicesHeaderLabel;
	private Label endingLabel;
	private Label minigamePromptLabel;

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
	private List<(string speaker, string text)> activeNodeDialogue = new();
	private StoryNode activeNode;
	private bool openingDone = false;

	private string minigameSuccessNode = "";
	private string minigameFailNode = "";
	private int minigameCorrectAnswer = 0;

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
		minigamePanel = GetNodeOrNull<Panel>("OpeningUI/MiniGamePanel");

		speakerLabel = GetNodeOrNull<Label>("OpeningUI/DialoguePanel/SpeakerLabel");
		dialogueLabel = GetNodeOrNull<Label>("OpeningUI/DialoguePanel/DialogueLabel");
		continueButton = GetNodeOrNull<Button>("OpeningUI/DialoguePanel/ContinueButton");
		choicesHeaderLabel = GetNodeOrNull<Label>("OpeningUI/StoryChoicesPanel/MarginContainer/VBoxContainer/ChoicesHeaderLabel");
		endingLabel = GetNodeOrNull<Label>("OpeningUI/StoryChoicesPanel/MarginContainer/VBoxContainer/EndingLabel");
		minigamePromptLabel = GetNodeOrNull<Label>("OpeningUI/MiniGamePanel/MarginContainer/VBoxContainer/PromptLabel");

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

		for (int i = 1; i <= 3; i++)
		{
			var btn = GetNodeOrNull<Button>($"OpeningUI/MiniGamePanel/MarginContainer/VBoxContainer/AnswerButton{i}");
			if (btn != null)
			{
				int answerIndex = i;
				btn.Pressed += () => OnMiniGameAnswer(answerIndex);
			}
		}
	}

	private void SetupInitialState()
	{
		openingUI.Visible = false;
		dialoguePanel.Visible = false;
		attributePanel.Visible = false;
		if (minigamePanel != null) minigamePanel.Visible = false;

		if (computerButton != null) computerButton.Disabled = true;
		if (outsideButton != null) outsideButton.Disabled = true;

		fadeOverlay.Visible = true;
		fadeOverlay.Modulate = new Color(1, 1, 1, 1);
	}

	private void LoadGameData()
	{
		currentSlot = GlobalVars.Instance.CurrentSlot;
		if (currentSlot < 0) return;

		var saveData = SaveManager.Instance.LoadGame(currentSlot);
		if (saveData.Count == 0) return;

		if (saveData.ContainsKey("player_name")) playerName = saveData["player_name"].AsString();
		if (saveData.ContainsKey("current_day")) currentDay = saveData["current_day"].AsInt32();
		if (saveData.ContainsKey("knowledge")) knowledge = saveData["knowledge"].AsInt32();
		if (saveData.ContainsKey("coding_skill")) codingSkill = saveData["coding_skill"].AsInt32();
		if (saveData.ContainsKey("energy")) energy = saveData["energy"].AsInt32();
		if (saveData.ContainsKey("confidence")) confidence = saveData["confidence"].AsInt32();
		if (saveData.ContainsKey("portfolio_progress")) portfolioProgress = saveData["portfolio_progress"].AsInt32();
		if (saveData.ContainsKey("has_seen_opening")) hasSeenOpening = saveData["has_seen_opening"].AsBool();
		if (saveData.ContainsKey("story_node")) currentStoryNode = saveData["story_node"].AsString();
		if (saveData.ContainsKey("ending_reached")) endingReached = saveData["ending_reached"].AsBool();
	}

	private void BuildOpeningDialogue()
	{
		openingDialogue = new List<(string speaker, string text)>
		{
			(playerName, "Third year already... and I still feel behind."),
			(playerName, "If I keep drifting, graduation will come before I am ready."),
			(playerName, "I need to make a comeback this semester."),
			(playerName, "This week starts with one decision at a time.")
		};
	}

	private void BuildStoryNodes()
	{
		storyNodes = new Dictionary<string, StoryNode>
		{
			{ "start", new StoryNode("Dorm room, Monday night. One week to reset your trajectory.", "Narrator", new List<StoryChoice>
				{
					new StoryChoice("Lock in and rebuild fundamentals", "study_planning", 1, 1, -1, 1, 0),
					new StoryChoice("Rush internship applications now", "network_path", 0, 1, -1, 2, 5),
					new StoryChoice("Avoid pressure and doom-scroll", "burnout_warning", -1, 0, 1, -1, 0)
				})
			},
			{ "study_planning", new StoryNode("You map weak topics: algorithms, databases, and system design basics.", "Mentor", new List<StoryChoice>
				{
					new StoryChoice("Take the debug sprint minigame", "", 1, 1, -1, 1, 5, "debug_sprint", "study_win", "study_fail"),
					new StoryChoice("Visit TA office hours first", "mentor_path", 1, 1, -1, 2, 5)
				})
			},
			{ "study_win", new StoryNode("You solved the bugs fast. Your confidence spikes and your notes finally make sense.", "Narrator", new List<StoryChoice>
				{
					new StoryChoice("Convert fixes into portfolio write-up", "project_push", 1, 2, -1, 2, 20),
					new StoryChoice("Use momentum for mock interviews", "finale", 1, 1, -1, 2, 10)
				})
			},
			{ "study_fail", new StoryNode("The sprint exposed gaps, but now you know exactly what to practice.", "Narrator", new List<StoryChoice>
				{
					new StoryChoice("Review mistakes and retry with mentor", "mentor_path", 2, 1, -1, 1, 5),
					new StoryChoice("Take a break and reset tomorrow", "burnout_warning", 0, 0, 1, 0, 0)
				})
			},
			{ "network_path", new StoryNode("Career center says your resume is weak, but your communication is improving.", "Career Coach", new List<StoryChoice>
				{
					new StoryChoice("Polish resume and message alumni", "mentor_path", 1, 1, -1, 2, 10),
					new StoryChoice("Attend hackathon this weekend", "project_push", 1, 2, -2, 1, 15)
				})
			},
			{ "burnout_warning", new StoryNode("Deadlines pile up. You need one small win to stop spiraling.", "Inner Voice", new List<StoryChoice>
				{
					new StoryChoice("One focused 45-minute study block", "study_planning", 1, 1, -1, 1, 5),
					new StoryChoice("Ask a friend to keep you accountable", "mentor_path", 0, 1, 0, 2, 5)
				})
			},
			{ "mentor_path", new StoryNode("Mentor says: 'Show impact. Recruiters trust proof.'", "Mentor", new List<StoryChoice>
				{
					new StoryChoice("Ship project MVP before Friday", "project_push", 1, 2, -2, 1, 20),
					new StoryChoice("Drill DSA and behavioral stories", "finale", 2, 1, -1, 1, 10)
				})
			},
			{ "project_push", new StoryNode("You ship visible progress and finally have proof of growth.", "Narrator", new List<StoryChoice>
				{
					new StoryChoice("Publish demo + technical blog", "finale", 1, 2, -1, 1, 25),
					new StoryChoice("Refine pitch for recruiter calls", "finale", 1, 1, -1, 2, 10)
				})
			},
			{ "finale", new StoryNode("Friday evening. Applications sent. Results incoming.", "Narrator", new List<StoryChoice>
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
			if (hasSeenOpening) FinishOpeningScene(); else StartOpeningDialogue();
		}));
	}

	private void StartOpeningDialogue()
	{
		openingDone = false;
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
			openingDone = true;
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
		if (!openingDone)
		{
			dialogueIndex++;
			ShowDialogueLine();
			return;
		}

		if (activeNodeDialogue.Count == 0)
			return;

		dialogueIndex++;
		if (dialogueIndex >= activeNodeDialogue.Count)
		{
			activeNodeDialogue.Clear();
			continueButton.Visible = false;
			ShowChoicesForActiveNode();
			return;
		}

		var line = activeNodeDialogue[dialogueIndex];
		speakerLabel.Text = line.speaker;
		dialogueLabel.Text = line.text;
	}

	private void FinishOpeningScene()
	{
		openingDone = true;
		openingUI.Visible = true;
		dialoguePanel.Visible = true;
		attributePanel.Visible = true;
		if (minigamePanel != null) minigamePanel.Visible = false;

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

		activeNode = storyNodes[currentStoryNode];
		activeNodeDialogue = BuildNodeDialogue(activeNode);
		dialogueIndex = 0;
		continueButton.Visible = true;
		dialogueLabel.Text = activeNodeDialogue[0].text;
		speakerLabel.Text = activeNodeDialogue[0].speaker;
		if (choicesHeaderLabel != null) choicesHeaderLabel.Text = "Choose your next move:";
	}

	private List<(string speaker, string text)> BuildNodeDialogue(StoryNode node)
	{
		var lines = new List<(string speaker, string text)> { (node.Speaker, node.Text) };

		if (currentStoryNode.StartsWith("study"))
		{
			if (knowledge + codingSkill >= 8)
				lines.Add((playerName, "I can feel the comeback momentum now."));
			else
				lines.Add((playerName, "Still shaky, but this branch is giving me structure."));
		}
		else if (confidence <= 1)
		{
			lines.Add((playerName, "I need a small win right now."));
		}

		return lines;
	}

	private void ShowChoicesForActiveNode()
	{
		if (activeNode == null) return;

		foreach (var choice in activeNode.Choices)
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

		if (!string.IsNullOrEmpty(choice.MiniGameId))
		{
			StartMiniGame(choice);
			UpdateAttributePanel();
			PersistProgress();
			return;
		}

		currentStoryNode = choice.NextNode;
		if (currentStoryNode == "ending") endingReached = true;

		UpdateAttributePanel();
		PersistProgress();
		RenderCurrentStoryNode();
	}

	private void StartMiniGame(StoryChoice choice)
	{
		if (minigamePanel == null) return;

		minigameSuccessNode = choice.SuccessNode;
		minigameFailNode = choice.FailureNode;
		minigameCorrectAnswer = 2;

		minigamePanel.Visible = true;
		if (minigamePromptLabel != null)
			minigamePromptLabel.Text = "Debug Sprint: Which fix reduces time complexity from O(n²) to O(n)?";

		SetMiniGameButton(1, "Use nested loops and add comments");
		SetMiniGameButton(2, "Use a hash set to track seen values");
		SetMiniGameButton(3, "Sort first, then still compare all pairs");

		if (choicesHeaderLabel != null) choicesHeaderLabel.Text = "Minigame in progress";
		ClearChoiceButtons();
		continueButton.Visible = false;
	}

	private void SetMiniGameButton(int idx, string text)
	{
		var btn = GetNodeOrNull<Button>($"OpeningUI/MiniGamePanel/MarginContainer/VBoxContainer/AnswerButton{idx}");
		if (btn != null) btn.Text = text;
	}

	private void OnMiniGameAnswer(int answer)
	{
		if (minigamePanel == null || !minigamePanel.Visible) return;

		bool success = answer == minigameCorrectAnswer;
		minigamePanel.Visible = false;

		if (success)
		{
			codingSkill += 2;
			confidence += 1;
			portfolioProgress = Mathf.Clamp(portfolioProgress + 10, 0, 100);
			currentStoryNode = minigameSuccessNode;
		}
		else
		{
			knowledge += 1;
			energy = Mathf.Max(0, energy - 1);
			currentStoryNode = minigameFailNode;
		}

		UpdateAttributePanel();
		PersistProgress();
		RenderCurrentStoryNode();
	}

	private void ShowEnding()
	{
		string endingText;
		int readinessScore = knowledge + codingSkill + confidence + (portfolioProgress / 10);

		if (portfolioProgress >= 70 && readinessScore >= 18)
			endingText = "Ending: Comeback Complete. You secure a strong internship-to-full-time track before graduation.";
		else if (portfolioProgress >= 40 && readinessScore >= 12)
			endingText = "Ending: Steady Recovery. You earn interviews and a realistic path to a good job after graduation.";
		else
			endingText = "Ending: Not Over Yet. This semester was rough, but you now know exactly what to fix before graduating.";

		dialogueLabel.Text = "Results week is here. Your choices and stats defined this outcome.";
		speakerLabel.Text = "Narrator";
		continueButton.Visible = false;
		if (choicesHeaderLabel != null) choicesHeaderLabel.Text = "Final outcome";

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
		if (choicesContainer == null) return;
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
		if (currentSlot < 0) return;

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
		public string Speaker { get; }
		public List<StoryChoice> Choices { get; }

		public StoryNode(string text, string speaker, List<StoryChoice> choices)
		{
			Text = text;
			Speaker = speaker;
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
		public string MiniGameId { get; }
		public string SuccessNode { get; }
		public string FailureNode { get; }

		public StoryChoice(string text, string nextNode, int knowledgeDelta, int codingDelta, int energyDelta, int confidenceDelta, int portfolioDelta, string miniGameId = "", string successNode = "", string failureNode = "")
		{
			Text = text;
			NextNode = nextNode;
			KnowledgeDelta = knowledgeDelta;
			CodingDelta = codingDelta;
			EnergyDelta = energyDelta;
			ConfidenceDelta = confidenceDelta;
			PortfolioDelta = portfolioDelta;
			MiniGameId = miniGameId;
			SuccessNode = successNode;
			FailureNode = failureNode;
		}
	}
}
