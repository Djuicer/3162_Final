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
	private Panel destinationPanel;

	private Label speakerLabel;
	private Label dialogueLabel;
	private Button continueButton;
	private Label choicesHeaderLabel;
	private Label endingLabel;
	private Label minigamePromptLabel;
	private Label destinationPromptLabel;

	private Label dayLabel;
	private Label knowledgeLabel;
	private Label codingSkillLabel;
	private Label energyLabel;
	private Label confidenceLabel;
	private Label portfolioLabel;

	private TextureButton computerButton;
	private TextureButton outsideButton;

	private bool openingDone = false;
	private int dialogueIndex = 0;
	private List<(string speaker, string text)> openingDialogue;
	private List<(string speaker, string text)> activeNodeDialogue = new();
	private Dictionary<string, StoryNode> storyNodes;
	private StoryNode activeNode;
	private StoryChoice pendingChoice;

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
		destinationPanel = GetNodeOrNull<Panel>("OpeningUI/DestinationPanel");

		speakerLabel = GetNodeOrNull<Label>("OpeningUI/DialoguePanel/SpeakerLabel");
		dialogueLabel = GetNodeOrNull<Label>("OpeningUI/DialoguePanel/DialogueLabel");
		continueButton = GetNodeOrNull<Button>("OpeningUI/DialoguePanel/ContinueButton");
		choicesHeaderLabel = GetNodeOrNull<Label>("OpeningUI/StoryChoicesPanel/MarginContainer/VBoxContainer/ChoicesHeaderLabel");
		endingLabel = GetNodeOrNull<Label>("OpeningUI/StoryChoicesPanel/MarginContainer/VBoxContainer/EndingLabel");
		minigamePromptLabel = GetNodeOrNull<Label>("OpeningUI/MiniGamePanel/MarginContainer/VBoxContainer/PromptLabel");
		destinationPromptLabel = GetNodeOrNull<Label>("OpeningUI/DestinationPanel/MarginContainer/VBoxContainer/PromptLabel");

		dayLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/DayLabel");
		knowledgeLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/KnowledgeLabel");
		codingSkillLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/CodingSkillLabel");
		energyLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/EnergyLabel");
		confidenceLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/ConfidenceLabel");
		portfolioLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/PortfolioLabel");

		computerButton = GetNodeOrNull<TextureButton>("Button/ComputerButton");
		outsideButton = GetNodeOrNull<TextureButton>("Button/OutsideButton");

		if (continueButton != null) continueButton.Pressed += OnContinuePressed;
		if (computerButton != null) computerButton.Pressed += () => OnHotspotClicked("computer");
		if (outsideButton != null) outsideButton.Pressed += () => OnHotspotClicked("door");

		for (int i = 1; i <= 3; i++)
		{
			var btn = GetNodeOrNull<Button>($"OpeningUI/MiniGamePanel/MarginContainer/VBoxContainer/AnswerButton{i}");
			if (btn != null)
			{
				int answerIndex = i;
				btn.Pressed += () => OnMiniGameAnswer(answerIndex);
			}
		}

		GetNodeOrNull<Button>("OpeningUI/DestinationPanel/MarginContainer/VBoxContainer/TAOfficeButton")?.Pressed += () => OnDestinationSelected("ta_office");
		GetNodeOrNull<Button>("OpeningUI/DestinationPanel/MarginContainer/VBoxContainer/CareerCenterButton")?.Pressed += () => OnDestinationSelected("career_center");
		GetNodeOrNull<Button>("OpeningUI/DestinationPanel/MarginContainer/VBoxContainer/CancelButton")?.Pressed += HideDestinationPanel;
	}

	private void SetupInitialState()
	{
		openingUI.Visible = false;
		dialoguePanel.Visible = false;
		attributePanel.Visible = false;
		if (minigamePanel != null) minigamePanel.Visible = false;
		if (destinationPanel != null) destinationPanel.Visible = false;

		if (computerButton != null) computerButton.Disabled = true;
		if (outsideButton != null) outsideButton.Disabled = true;
		fadeOverlay.Visible = true;
		fadeOverlay.Modulate = new Color(1, 1, 1, 1);
	}

	private void BuildOpeningDialogue()
	{
		openingDialogue = new List<(string speaker, string text)>
		{
			(playerName, "Third year already... and I still feel behind."),
			(playerName, "I need to make a comeback before graduation."),
			(playerName, "Every action now needs to be intentional.")
		};
	}

	private void BuildStoryNodes()
	{
		storyNodes = new Dictionary<string, StoryNode>
		{
			{ "start", new StoryNode("Monday night in your dorm. Pick your first move.", "Narrator", new List<StoryChoice>
				{
					new StoryChoice("Lock in and rebuild fundamentals", "study_planning", 1, 1, -1, 1, 0, requiredHotspot: "computer"),
					new StoryChoice("Rush internship applications now", "network_path", 0, 1, -1, 2, 5, requiredHotspot: "door", requiredDestination: "career_center"),
					new StoryChoice("Avoid pressure and doom-scroll", "burnout_warning", -1, 0, 1, -1, 0)
				})
			},
			{ "study_planning", new StoryNode("You map weak topics. TA office can clarify your blind spots.", "Mentor", new List<StoryChoice>
				{
					new StoryChoice("Take debug sprint minigame on your computer", "", 1, 1, -1, 1, 5, miniGameId: "debug_sprint", successNode: "study_win", failureNode: "study_fail", requiredHotspot: "computer"),
					new StoryChoice("Visit TA office", "mentor_path", 1, 1, -1, 2, 5, requiredHotspot: "door", requiredDestination: "ta_office")
				})
			},
			{ "study_win", new StoryNode("You solve the sprint and turn fixes into confidence.", "Narrator", new List<StoryChoice>
				{
					new StoryChoice("Convert fixes into portfolio write-up", "project_push", 1, 2, -1, 2, 20, requiredHotspot: "computer"),
					new StoryChoice("Use momentum for mock interviews", "finale", 1, 1, -1, 2, 10, requiredHotspot: "door", requiredDestination: "career_center")
				})
			},
			{ "study_fail", new StoryNode("Gaps exposed. You can recover with structure.", "Narrator", new List<StoryChoice>
				{
					new StoryChoice("Review mistakes with TA office", "mentor_path", 2, 1, -1, 1, 5, requiredHotspot: "door", requiredDestination: "ta_office"),
					new StoryChoice("Reset tomorrow", "burnout_warning", 0, 0, 1, 0, 0)
				})
			},
			{ "network_path", new StoryNode("Career center points out missing proof in your resume.", "Career Coach", new List<StoryChoice>
				{
					new StoryChoice("Message alumni and refine resume", "mentor_path", 1, 1, -1, 2, 10, requiredHotspot: "computer"),
					new StoryChoice("Attend weekend hackathon", "project_push", 1, 2, -2, 1, 15, requiredHotspot: "door", requiredDestination: "career_center")
				})
			},
			{ "burnout_warning", new StoryNode("Deadlines pile up. One focused action can reset the week.", "Inner Voice", new List<StoryChoice>
				{
					new StoryChoice("One focused 45-minute study block", "study_planning", 1, 1, -1, 1, 5, requiredHotspot: "computer"),
					new StoryChoice("Ask mentor for accountability", "mentor_path", 0, 1, 0, 2, 5, requiredHotspot: "door", requiredDestination: "ta_office")
				})
			},
			{ "mentor_path", new StoryNode("Mentor says: 'Show evidence, not intent.'", "Mentor", new List<StoryChoice>
				{
					new StoryChoice("Ship project MVP", "project_push", 1, 2, -2, 1, 20, requiredHotspot: "computer"),
					new StoryChoice("Practice interviews", "finale", 2, 1, -1, 1, 10, requiredHotspot: "door", requiredDestination: "career_center")
				})
			},
			{ "project_push", new StoryNode("You now have tangible output recruiters can evaluate.", "Narrator", new List<StoryChoice>
				{
					new StoryChoice("Publish demo and blog", "finale", 1, 2, -1, 1, 25, requiredHotspot: "computer"),
					new StoryChoice("Pitch project in recruiter calls", "finale", 1, 1, -1, 2, 10, requiredHotspot: "door", requiredDestination: "career_center")
				})
			},
			{ "finale", new StoryNode("Friday evening. You sent applications and now wait.", "Narrator", new List<StoryChoice>
				{
					new StoryChoice("See outcome", "ending", 0, 0, 0, 0, 0)
				})
			}
		};
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

	private void StartFadeIn()
	{
		var tween = CreateTween();
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

		if (activeNodeDialogue.Count == 0) return;
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
		UpdateAttributePanel();
		if (computerButton != null) computerButton.Disabled = false;
		if (outsideButton != null) outsideButton.Disabled = false;
		RenderCurrentStoryNode();
	}

	private void RenderCurrentStoryNode()
	{
		ClearChoiceButtons();
		if (endingLabel != null) endingLabel.Visible = false;
		HideDestinationPanel();
		if (minigamePanel != null) minigamePanel.Visible = false;
		pendingChoice = null;

		if (endingReached || currentStoryNode == "ending")
		{
			ShowEnding();
			return;
		}

		if (!storyNodes.ContainsKey(currentStoryNode)) currentStoryNode = "start";
		activeNode = storyNodes[currentStoryNode];
		activeNodeDialogue = new List<(string speaker, string text)> { (activeNode.Speaker, activeNode.Text) };
		dialogueIndex = 0;
		continueButton.Visible = true;
		speakerLabel.Text = activeNodeDialogue[0].speaker;
		dialogueLabel.Text = activeNodeDialogue[0].text;
		if (choicesHeaderLabel != null) choicesHeaderLabel.Text = "Choose your next move:";
	}

	private void ShowChoicesForActiveNode()
	{
		if (activeNode == null) return;
		foreach (var choice in activeNode.Choices)
		{
			var button = new Button { Text = choice.Text, CustomMinimumSize = new Vector2(0, 42), AutowrapMode = TextServer.AutowrapMode.WordSmart };
			button.Pressed += () => BeginChoiceResolution(choice);
			choicesContainer.AddChild(button);
		}
	}

	private void BeginChoiceResolution(StoryChoice choice)
	{
		pendingChoice = choice;
		if (string.IsNullOrEmpty(choice.RequiredHotspot))
		{
			ExecuteChoice(choice);
			return;
		}

		ClearChoiceButtons();
		if (choicesHeaderLabel != null)
			choicesHeaderLabel.Text = choice.RequiredHotspot == "door"
				? "Action required: click the door to travel."
				: "Action required: click the computer to proceed.";
	}

	private void OnHotspotClicked(string hotspot)
	{
		if (pendingChoice == null) return;
		if (pendingChoice.RequiredHotspot != hotspot)
		{
			dialogueLabel.Text = hotspot == "door" ? "This action needs computer work first." : "You need to go through the door for this action.";
			speakerLabel.Text = "System";
			return;
		}

		if (hotspot == "door" && !string.IsNullOrEmpty(pendingChoice.RequiredDestination))
		{
			ShowDestinationPanel();
			return;
		}

		ExecuteChoice(pendingChoice);
	}

	private void ShowDestinationPanel()
	{
		if (destinationPanel == null) return;
		destinationPanel.Visible = true;
		if (destinationPromptLabel != null)
			destinationPromptLabel.Text = "Choose where to go through the door:";
	}

	private void HideDestinationPanel()
	{
		if (destinationPanel != null)
			destinationPanel.Visible = false;
	}

	private void OnDestinationSelected(string destination)
	{
		if (pendingChoice == null) return;
		if (pendingChoice.RequiredDestination != destination)
		{
			dialogueLabel.Text = "That destination doesn't match this action. Try again.";
			speakerLabel.Text = "System";
			return;
		}

		HideDestinationPanel();
		ExecuteChoice(pendingChoice);
	}

	private void ExecuteChoice(StoryChoice choice)
	{
		knowledge = Mathf.Max(0, knowledge + choice.KnowledgeDelta);
		codingSkill = Mathf.Max(0, codingSkill + choice.CodingDelta);
		energy = Mathf.Clamp(energy + choice.EnergyDelta, 0, 10);
		confidence = Mathf.Max(0, confidence + choice.ConfidenceDelta);
		portfolioProgress = Mathf.Clamp(portfolioProgress + choice.PortfolioDelta, 0, 100);
		currentDay = Mathf.Min(currentDay + 1, 7);
		pendingChoice = null;

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
		minigameSuccessNode = choice.SuccessNode;
		minigameFailNode = choice.FailureNode;
		minigameCorrectAnswer = 2;
		if (minigamePanel == null) return;
		minigamePanel.Visible = true;
		if (minigamePromptLabel != null) minigamePromptLabel.Text = "Debug Sprint: pick the best O(n) fix.";
		SetMiniGameButton(1, "Use nested loops with comments");
		SetMiniGameButton(2, "Track seen values with a hash set");
		SetMiniGameButton(3, "Sort first, then compare all pairs");
		if (choicesHeaderLabel != null) choicesHeaderLabel.Text = "Minigame in progress";
	}

	private void SetMiniGameButton(int idx, string text)
	{
		var btn = GetNodeOrNull<Button>($"OpeningUI/MiniGamePanel/MarginContainer/VBoxContainer/AnswerButton{idx}");
		if (btn != null) btn.Text = text;
	}

	private void OnMiniGameAnswer(int answer)
	{
		if (minigamePanel == null || !minigamePanel.Visible) return;
		minigamePanel.Visible = false;
		if (answer == minigameCorrectAnswer)
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
			endingText = "Ending: Comeback Complete.";
		else if (portfolioProgress >= 40 && readinessScore >= 12)
			endingText = "Ending: Steady Recovery.";
		else
			endingText = "Ending: Not Over Yet.";

		speakerLabel.Text = "Narrator";
		dialogueLabel.Text = "Results week is here. Your actions decided this ending.";
		continueButton.Visible = false;
		if (choicesHeaderLabel != null) choicesHeaderLabel.Text = "Final outcome";
		if (endingLabel != null)
		{
			endingLabel.Visible = true;
			endingLabel.Text = endingText;
		}

		var restartButton = new Button { Text = "Start New Run", CustomMinimumSize = new Vector2(0, 42) };
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
		pendingChoice = null;
		UpdateAttributePanel();
		PersistProgress();
		RenderCurrentStoryNode();
	}

	private void ClearChoiceButtons()
	{
		if (choicesContainer == null) return;
		for (int i = choicesContainer.GetChildCount() - 1; i >= 0; i--)
		{
			if (choicesContainer.GetChild(i) is Button child) child.QueueFree();
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
		public string RequiredHotspot { get; }
		public string RequiredDestination { get; }

		public StoryChoice(string text, string nextNode, int knowledgeDelta, int codingDelta, int energyDelta, int confidenceDelta, int portfolioDelta, string miniGameId = "", string successNode = "", string failureNode = "", string requiredHotspot = "", string requiredDestination = "")
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
			RequiredHotspot = requiredHotspot;
			RequiredDestination = requiredDestination;
		}
	}
}
