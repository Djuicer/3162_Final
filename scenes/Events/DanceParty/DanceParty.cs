using Godot;

public partial class DanceParty : Node2D
{
	private enum PartyNpc { None, Classmate, Alumni }

	private Area2D _classmateArea;
	private Area2D _alumniArea;
	private Label _interactPromptLabel;
	private Label _instructionLabel;
	private Label _statusLabel;
	private Label _npcNameLabel;
	private Label _dialogueLabel;
	private Button _finishPartyButton;
	private VBoxContainer _choiceButtons;

	private bool _classmateDone;
	private bool _alumniDone;
	private bool _dialogueOpen;
	private PartyNpc _nearNpc = PartyNpc.None;

	public override void _Ready()
	{
		StatusOverlay.AttachTo(this, false);
		_classmateArea = GetNode<Area2D>("DancePartyClassmateNPC/InteractionArea");
		_alumniArea = GetNode<Area2D>("DancePartyRecruiterNPC/InteractionArea");
		_interactPromptLabel = GetNode<Label>("UI/InteractPromptLabel");
		_instructionLabel = GetNode<Label>("UI/DialoguePanel/ContentContainer/InstructionLabel");
		_statusLabel = GetNode<Label>("UI/DialoguePanel/ContentContainer/StatusLabel");
		_npcNameLabel = GetNode<Label>("UI/DialoguePanel/ContentContainer/NpcNameLabel");
		_dialogueLabel = GetNode<Label>("UI/DialoguePanel/ContentContainer/DialogueLabel");
		_finishPartyButton = GetNode<Button>("UI/DialoguePanel/ContentContainer/ActionRow/FinishPartyButton");
		_choiceButtons = GetNode<VBoxContainer>("UI/DialoguePanel/ContentContainer/Choices");

		_classmateArea.BodyEntered += _ => OnNpcAreaEntered(PartyNpc.Classmate);
		_classmateArea.BodyExited += _ => OnNpcAreaExited(PartyNpc.Classmate);
		_alumniArea.BodyEntered += _ => OnNpcAreaEntered(PartyNpc.Alumni);
		_alumniArea.BodyExited += _ => OnNpcAreaExited(PartyNpc.Alumni);

		_finishPartyButton.Pressed += FinishParty;

		var state = GlobalVars.Instance;
		state.PartyGoodChoices = 0;
		state.PartyNetworkingUnlocked = false;

		_instructionLabel.Text = "Talk to people and make a good impression.";
		_statusLabel.Text = "Talk to both people before finishing the party.";
		_npcNameLabel.Text = "";
		_dialogueLabel.Text = "";
		_interactPromptLabel.Visible = false;
		_finishPartyButton.Disabled = true;

		if (state.PartyEventCompleted)
		{
			_dialogueLabel.Text = "The party is already over. You can head back.";
			_classmateDone = true;
			_alumniDone = true;
			_finishPartyButton.Disabled = false;
		}
	}

	public override void _Process(double delta)
	{
		_interactPromptLabel.Visible = !_dialogueOpen && _nearNpc != PartyNpc.None;
		if (_interactPromptLabel.Visible)
			_interactPromptLabel.Text = "Press E to talk";
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!_dialogueOpen && @event.IsActionPressed("interact"))
		{
			if (_nearNpc == PartyNpc.Classmate)
				OpenClassmateDialogue();
			else if (_nearNpc == PartyNpc.Alumni)
				OpenAlumniDialogue();
		}
	}

	private void StartClassmateDialogue()
	{
		_dialogueOpen = true;
		_npcNameLabel.Text = "Classmate";
		ClearChoices();
		_dialogueLabel.Text = "Hey, I heard you're trying to get back on track before graduation. What have you been working on?";
		AddChoice("I've been rebuilding my coding skills and working on my portfolio.", () => { var s = GlobalVars.Instance; s.Attributes.IncreaseConfidence(5); s.Attributes.IncreaseNetworking(5); s.PartyGoodChoices += 1; CompleteClassmate("Great answer. You sounded focused and motivated."); });
		AddChoice("Honestly, I have no idea. I'm just hoping things work out.", () => { GlobalVars.Instance.Attributes.DecreaseConfidence(3); CompleteClassmate("A hesitant answer made the conversation awkward."); });
		AddChoice("I don't really care about graduation anymore.", () => { var s = GlobalVars.Instance; s.Attributes.DecreaseConfidence(5); s.Attributes.DecreaseNetworking(3); CompleteClassmate("That answer hurt your impression."); });
	}

	private void StartAlumniDialogue()
	{
		_dialogueOpen = true;
		_npcNameLabel.Text = "Alumni / Recruiter";
		ClearChoices();
		_dialogueLabel.Text = "I'm helping a few students connect with graduate IT opportunities. What kind of role are you aiming for?";
		AddChoice("I'm aiming for a graduate developer role and preparing through projects and interview practice.", () => { var s = GlobalVars.Instance; s.Profile.IncreaseCareerReadiness(5); s.Attributes.IncreaseNetworking(10); s.PartyGoodChoices += 1; CompleteAlumni("Strong answer. You made a good professional impression."); });
		AddChoice("Anything that pays, I guess.", () => { GlobalVars.Instance.Attributes.DecreaseNetworking(2); CompleteAlumni("The answer sounded unclear and unprepared."); });
		AddChoice("I haven't prepared, but I want a job anyway.", () => { var s = GlobalVars.Instance; s.Attributes.DecreaseConfidence(3); s.Attributes.DecreaseNetworking(5); CompleteAlumni("That answer lowered confidence in your readiness."); });
	}

	private void CompleteClassmate(string message)
	{
		_classmateDone = true;
		_statusLabel.Text = message;
		_npcNameLabel.Text = "";
		ClearChoices();
		_dialogueOpen = false;
		UpdateFinishAvailability();
	}

	private void CompleteAlumni(string message)
	{
		_alumniDone = true;
		_statusLabel.Text = message;
		_npcNameLabel.Text = "";
		ClearChoices();
		_dialogueOpen = false;
		UpdateFinishAvailability();
	}

	private void UpdateFinishAvailability()
	{
		if (_classmateDone && _alumniDone)
			_finishPartyButton.Disabled = false;
	}

	private void FinishParty()
	{
		var state = GlobalVars.Instance;
		if (state.PartyGoodChoices >= 2)
		{
			state.PartyNetworkingUnlocked = true;
			_statusLabel.Text = "You made a strong impression. This connection may help you later.";
		}
		else
		{
			state.PartyNetworkingUnlocked = false;
			_statusLabel.Text = "You attended the party, but did not make a strong connection.";
		}
		state.PartyEventCompleted = true;

		TransitionManager.Instance?.ChangeSceneToFileWithFade("res://scenes/Domitory/dormitory.tscn");
	}

	private void AddChoice(string text, System.Action onPressed)
	{
		var button = new Button { Text = text, CustomMinimumSize = new Vector2(0, 38) };
		button.Pressed += () => onPressed();
		_choiceButtons.AddChild(button);
	}

	private void ClearChoices()
	{
		foreach (Node child in _choiceButtons.GetChildren())
			child.QueueFree();
	}

	private void OpenClassmateDialogue()
	{
		if (_classmateDone)
		{
			_statusLabel.Text = "You already spoke with them.";
			return;
		}
		StartClassmateDialogue();
	}

	private void OpenAlumniDialogue()
	{
		if (_alumniDone)
		{
			_statusLabel.Text = "You already spoke with them.";
			return;
		}
		StartAlumniDialogue();
	}

	private void OnNpcAreaEntered(PartyNpc npc)
	{
		_nearNpc = npc;
	}

	private void OnNpcAreaExited(PartyNpc npc)
	{
		if (_nearNpc == npc)
			_nearNpc = PartyNpc.None;
	}
}
