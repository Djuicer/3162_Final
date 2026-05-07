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

	private ColorRect fadeOverlay;

	private CanvasLayer openingUI;
	private Panel dialoguePanel;
	private Panel attributePanel;

	private Label speakerLabel;
	private Label dialogueLabel;
	private Button continueButton;

	private Label dayLabel;
	private Label knowledgeLabel;
	private Label codingSkillLabel;
	private Label energyLabel;
	private Label confidenceLabel;
	private Label portfolioLabel;

	private TextureButton computerButton;
	private TextureButton outsideButton;
	private Area2D computerInteractArea;
	private Area2D bedInteractArea;
	private Area2D doorInteractArea;
	private CharacterBody2D player;
	private Label interactPromptLabel;
	private Label guidanceStatsLabel;
	private Label dormitoryHintLabel;
	private Panel tutorialPanel;
	private Button tutorialOkButton;
	private PackedScene computerScene;
	private PackedScene endingScene;
	private bool canUseComputer = false;
	private bool canUseBed = false;
	private bool canUseDoor = false;
	private bool travelMenuOpen = false;
	private Panel travelMenuPanel;
	private Button travelComputerLabButton;
	private Button travelInnovationHubButton;
	private Button travelCareerCentreButton;
	private Button travelCancelButton;

	private int dialogueIndex = 0;
	private List<(string speaker, string text)> openingDialogue;

	public override void _Ready()
	{
		StatusOverlay.AttachTo(this);
		GetNodes();
		SetupInitialState();
		LoadGameData();
		BuildOpeningDialogue();
		UpdateAttributePanel();
		RefreshGuidanceUi();
		StartFadeIn();

		if (interactPromptLabel != null)
			interactPromptLabel.Visible = false;
	}

	private void GetNodes()
	{
		fadeOverlay = GetNodeOrNull<ColorRect>("FadeOverlay");

		openingUI = GetNodeOrNull<CanvasLayer>("OpeningUI");
		dialoguePanel = GetNodeOrNull<Panel>("OpeningUI/DialoguePanel");
		attributePanel = GetNodeOrNull<Panel>("OpeningUI/AttributePanel");

		speakerLabel = GetNodeOrNull<Label>("OpeningUI/DialoguePanel/SpeakerLabel");
		dialogueLabel = GetNodeOrNull<Label>("OpeningUI/DialoguePanel/DialogueLabel");
		continueButton = GetNodeOrNull<Button>("OpeningUI/DialoguePanel/ContinueButton");

		dayLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/DayLabel");
		knowledgeLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/KnowledgeLabel");
		codingSkillLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/CodingSkillLabel");
		energyLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/EnergyLabel");
		confidenceLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/ConfidenceLabel");
		portfolioLabel = GetNode<Label>("OpeningUI/AttributePanel/MarginContainer/VBoxContainer/PortfolioLabel");

		computerButton = GetNodeOrNull<TextureButton>("Button/ComputerButton");
		outsideButton = GetNodeOrNull<TextureButton>("Button/OutsideButton");
		computerInteractArea = GetNodeOrNull<Area2D>("ComputerInteractArea");
		bedInteractArea = GetNodeOrNull<Area2D>("BedInteractArea");
		doorInteractArea = GetNodeOrNull<Area2D>("DoorInteractArea");
		player = GetNodeOrNull<CharacterBody2D>("Player");
		interactPromptLabel = GetNodeOrNull<Label>("OpeningUI/InteractPromptLabel");
		travelMenuPanel = GetNodeOrNull<Panel>("OpeningUI/TravelMenuPanel");
		travelComputerLabButton = GetNodeOrNull<Button>("OpeningUI/TravelMenuPanel/MarginContainer/VBoxContainer/ComputerLabButton");
		travelInnovationHubButton = GetNodeOrNull<Button>("OpeningUI/TravelMenuPanel/MarginContainer/VBoxContainer/InnovationHubButton");
		travelCareerCentreButton = GetNodeOrNull<Button>("OpeningUI/TravelMenuPanel/MarginContainer/VBoxContainer/CareerCentreButton");
		travelCancelButton = GetNodeOrNull<Button>("OpeningUI/TravelMenuPanel/MarginContainer/VBoxContainer/CancelButton");
		guidanceStatsLabel = GetNodeOrNull<Label>("OpeningUI/GuidancePanel/MarginContainer/VBoxContainer/StatsLabel");
		dormitoryHintLabel = GetNodeOrNull<Label>("OpeningUI/GuidancePanel/MarginContainer/VBoxContainer/HintLabel");
		tutorialPanel = GetNodeOrNull<Panel>("OpeningUI/TutorialPanel");
		tutorialOkButton = GetNodeOrNull<Button>("OpeningUI/TutorialPanel/MarginContainer/VBoxContainer/OkButton");
		computerScene = ResourceLoader.Load<PackedScene>("res://scenes/Screen/screen.tscn");
		endingScene = ResourceLoader.Load<PackedScene>("res://scenes/Ending/ending.tscn");

		if (fadeOverlay == null) GD.PrintErr("Missing node: FadeOverlay");
		if (openingUI == null) GD.PrintErr("Missing node: OpeningUI");
		if (dialoguePanel == null) GD.PrintErr("Missing node: OpeningUI/DialoguePanel");
		if (attributePanel == null) GD.PrintErr("Missing node: OpeningUI/AttributePanel");
		if (speakerLabel == null) GD.PrintErr("Missing node: OpeningUI/DialoguePanel/SpeakerLabel");
		if (dialogueLabel == null) GD.PrintErr("Missing node: OpeningUI/DialoguePanel/DialogueLabel");
		if (continueButton == null) GD.PrintErr("Missing node: OpeningUI/DialoguePanel/ContinueButton");

		if (dayLabel == null) GD.PrintErr("Missing node: OpeningUI/AttributePanel/DayLabel");
		if (knowledgeLabel == null) GD.PrintErr("Missing node: OpeningUI/AttributePanel/KnowledgeLabel");
		if (codingSkillLabel == null) GD.PrintErr("Missing node: OpeningUI/AttributePanel/CodingSkillLabel");
		if (energyLabel == null) GD.PrintErr("Missing node: OpeningUI/AttributePanel/EnergyLabel");
		if (confidenceLabel == null) GD.PrintErr("Missing node: OpeningUI/AttributePanel/ConfidenceLabel");
		if (portfolioLabel == null) GD.PrintErr("Missing node: OpeningUI/AttributePanel/PortfolioLabel");

		if (computerButton == null) GD.PrintErr("Missing node: Button/ComputerButton");
		if (outsideButton == null) GD.PrintErr("Missing node: Button/OutsideButton");

		if (computerInteractArea != null)
		{
			computerInteractArea.BodyEntered += OnComputerAreaBodyEntered;
			computerInteractArea.BodyExited += OnComputerAreaBodyExited;
		}
		if (bedInteractArea != null)
		{
			bedInteractArea.BodyEntered += OnBedAreaBodyEntered;
			bedInteractArea.BodyExited += OnBedAreaBodyExited;
		}
		if (doorInteractArea != null)
		{
			doorInteractArea.BodyEntered += OnDoorAreaBodyEntered;
			doorInteractArea.BodyExited += OnDoorAreaBodyExited;
		}
		if (interactPromptLabel != null)
			interactPromptLabel.Visible = false;
		if (travelMenuPanel != null)
			travelMenuPanel.Visible = false;

		if (continueButton != null)
			continueButton.Pressed += OnContinuePressed;

		if (tutorialOkButton != null)
			tutorialOkButton.Pressed += OnTutorialOkPressed;

		if (travelComputerLabButton != null)
			travelComputerLabButton.Pressed += () => TravelTo("res://scenes/ComputerLab/computer_lab.tscn");
		if (travelInnovationHubButton != null)
			travelInnovationHubButton.Pressed += () => TravelTo("res://scenes/InnovationHub/innovation_hub.tscn");
		if (travelCareerCentreButton != null)
			travelCareerCentreButton.Pressed += () => TravelTo("res://scenes/CareerCentre/career_centre.tscn");
		if (travelCancelButton != null)
			travelCancelButton.Pressed += CloseTravelMenu;
	}


	public override void _Process(double delta)
	{
		RefreshGuidanceUi();

		if (interactPromptLabel == null)
			return;

		bool showPrompt = canUseComputer && !computerButton.Disabled;
		if (travelMenuOpen)
		{
			interactPromptLabel.Visible = false;
			return;
		}

		if (canUseDoor)
		{
			interactPromptLabel.Text = "Press E to travel";
			interactPromptLabel.Visible = true;
			return;
		}

		if (canUseBed)
		{
			interactPromptLabel.Text = "Press E to sleep";
			interactPromptLabel.Visible = true;
			return;
		}

		if (showPrompt)
		{
			interactPromptLabel.Text = "Press E to use computer";
			interactPromptLabel.Visible = true;
			return;
		}

		interactPromptLabel.Visible = false;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (travelMenuOpen && @event.IsActionPressed("ui_cancel"))
		{
			CloseTravelMenu();
			return;
		}

		if (!@event.IsActionPressed("interact"))
			return;

		if (canUseBed)
		{
			SleepAtBed();
			return;
		}

		if (canUseComputer && !computerButton.Disabled)
		{
			if (computerScene != null)
				GetTree().ChangeSceneToPacked(computerScene);
			else
				GD.PrintErr("Computer scene is missing.");
		}

		if (canUseDoor)
		{
			OpenTravelMenu();
			return;
		}
	}

	private void OnComputerAreaBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D)
			canUseComputer = true;
	}

	private void OnComputerAreaBodyExited(Node2D body)
	{
		if (body is CharacterBody2D)
			canUseComputer = false;
	}

	private void OnBedAreaBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D)
			canUseBed = true;
	}

	private void OnBedAreaBodyExited(Node2D body)
	{
		if (body is CharacterBody2D)
			canUseBed = false;
	}

	private void OnDoorAreaBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D)
			canUseDoor = true;
	}

	private void OnDoorAreaBodyExited(Node2D body)
	{
		if (body is CharacterBody2D)
			canUseDoor = false;
	}

	private void OpenTravelMenu()
	{
		travelMenuOpen = true;
		if (travelMenuPanel != null)
			travelMenuPanel.Visible = true;
		if (interactPromptLabel != null)
			interactPromptLabel.Visible = false;
		SetPlayerMovementEnabled(false);
	}

	private void CloseTravelMenu()
	{
		travelMenuOpen = false;
		if (travelMenuPanel != null)
			travelMenuPanel.Visible = false;
		SetPlayerMovementEnabled(true);
	}

	private void TravelTo(string scenePath)
	{
		CloseTravelMenu();
		GetTree().ChangeSceneToFile(scenePath);
	}

	private void SetPlayerMovementEnabled(bool enabled)
	{
		if (player == null)
			return;
		player.SetPhysicsProcess(enabled);
		player.SetProcess(enabled);
	}

	private void SleepAtBed()
	{
		var state = GlobalVars.Instance;
		state.Attributes.IncreaseEnergy(30);
		state.Attributes.IncreaseFocus(10);
		state.Profile.CurrentDay += 1;
		state.Profile.ResetActionsForNewDay();

		GD.Print($"[Sleep] Day={state.Profile.CurrentDay}/{state.Profile.FinalDay}, Actions={state.Profile.ActionsLeft}/{state.Profile.MaxActionsPerDay}, Focus={state.Attributes.Focus}/100, Energy={state.Attributes.Energy}/100, Knowledge={state.Attributes.Knowledge}/100, Confidence={state.Attributes.Confidence}/100, CareerReadiness={state.Profile.CareerReadiness}/100");

		RefreshGuidanceUi();

		if (state.Profile.CurrentDay > state.Profile.FinalDay)
		{
			GD.Print("Final results coming soon.");
						if (endingScene != null)
			{
				GetTree().ChangeSceneToPacked(endingScene);
			}
			else
			{
				GD.PrintErr("Ending scene is missing.");
			}
		}
	}


	private void RefreshGuidanceUi()
	{
		var state = GlobalVars.Instance;
		if (state == null)
			return;

		if (guidanceStatsLabel != null)
		{
			guidanceStatsLabel.Text = $"Day {state.Profile.CurrentDay} / {state.Profile.FinalDay}\nActions Left {state.Profile.ActionsLeft} / {state.Profile.MaxActionsPerDay}\nEnergy {state.Attributes.Energy} / 100";
		}

		if (dormitoryHintLabel != null)
			dormitoryHintLabel.Text = $"Hint: {state.GetDormitoryHintMessage()}";
	}

	private void ShowTutorialIfNeeded()
	{
		var state = GlobalVars.Instance;
		if (tutorialPanel == null || state == null)
			return;

		if (!state.HasSeenDormitoryTutorial)
		{
			tutorialPanel.Visible = true;
			state.HasSeenDormitoryTutorial = true;
		}
		else
		{
			tutorialPanel.Visible = false;
		}
	}

	private void OnTutorialOkPressed()
	{
		if (tutorialPanel != null)
			tutorialPanel.Visible = false;
	}

	private void SetupInitialState()
	{
		openingUI.Visible = true;
		dialoguePanel.Visible = false;
		ShowTutorialIfNeeded();
		attributePanel.Visible = false;

		computerButton.Disabled = true;
		outsideButton.Disabled = true;

		fadeOverlay.Visible = true;
		fadeOverlay.Modulate = new Color(1, 1, 1, 1);
	}

	private void LoadGameData()
	{
		currentSlot = GlobalVars.Instance.CurrentSlot;

		if (currentSlot < 0)
		{
			GD.PrintErr("No save slot selected.");
			return;
		}

		var saveData = SaveManager.Instance.LoadGame(currentSlot);

		if (saveData.Count == 0)
		{
			GD.PrintErr($"Slot {currentSlot} has no save data.");
			return;
		}

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
	}

	private void BuildOpeningDialogue()
	{
		openingDialogue = new List<(string speaker, string text)>
		{
			(playerName, "Final year already..."),
			(playerName, "Everyone else seems to know what they're doing."),
			(playerName, "I've been telling myself I still have time."),
			(playerName, "But if I keep waiting, graduation is going to hit me first."),
			(playerName, "I need to start locking in right now.")
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
		ShowTutorialIfNeeded();
		attributePanel.Visible = true;

		UpdateAttributePanel();

		computerButton.Disabled = false;
		outsideButton.Disabled = true;
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

		var saveData = SaveManager.Instance.LoadGame(currentSlot);
		saveData["has_seen_opening"] = true;
		SaveManager.Instance.SaveGame(currentSlot, saveData);
	}
}
