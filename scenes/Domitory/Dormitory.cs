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

	private int dialogueIndex = 0;
	private List<(string speaker, string text)> openingDialogue;

	public override void _Ready()
	{
		GetNodes();
		SetupInitialState();
		LoadGameData();
		BuildOpeningDialogue();
		UpdateAttributePanel();
		StartFadeIn();
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

		if (continueButton != null)
			continueButton.Pressed += OnContinuePressed;
	}

	private void SetupInitialState()
	{
		openingUI.Visible = false;
		dialoguePanel.Visible = false;
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
