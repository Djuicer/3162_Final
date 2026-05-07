using Godot;

public partial class DanceParty : Node2D
{
	private Label _instructionLabel;
	private Label _statusLabel;
	private Label _dialogueLabel;
	private Button _talkClassmateButton;
	private Button _talkAlumniButton;
	private Button _finishPartyButton;
	private VBoxContainer _choiceButtons;

	private bool _classmateDone;
	private bool _alumniDone;

	public override void _Ready()
	{
		_instructionLabel = GetNode<Label>("UI/Panel/VBox/InstructionLabel");
		_statusLabel = GetNode<Label>("UI/Panel/VBox/StatusLabel");
		_dialogueLabel = GetNode<Label>("UI/Panel/VBox/DialogueLabel");
		_talkClassmateButton = GetNode<Button>("UI/Panel/VBox/ActionRow/TalkClassmateButton");
		_talkAlumniButton = GetNode<Button>("UI/Panel/VBox/ActionRow/TalkAlumniButton");
		_finishPartyButton = GetNode<Button>("UI/Panel/VBox/ActionRow/FinishPartyButton");
		_choiceButtons = GetNode<VBoxContainer>("UI/Panel/VBox/Choices");

		_talkClassmateButton.Pressed += StartClassmateDialogue;
		_talkAlumniButton.Pressed += StartAlumniDialogue;
		_finishPartyButton.Pressed += FinishParty;

		var state = GlobalVars.Instance;
		state.PartyGoodChoices = 0;
		state.PartyNetworkingUnlocked = false;

		_instructionLabel.Text = "Talk to people and make a good impression.";
		_statusLabel.Text = "Talk to both people before finishing the party.";
		_dialogueLabel.Text = "";
		_finishPartyButton.Disabled = true;

		if (state.PartyEventCompleted)
		{
			_dialogueLabel.Text = "The party is already over. You can head back.";
			_talkClassmateButton.Disabled = true;
			_talkAlumniButton.Disabled = true;
			_finishPartyButton.Disabled = false;
		}
	}

	private void StartClassmateDialogue()
	{
		if (_classmateDone) return;
		ClearChoices();
		_dialogueLabel.Text = "Hey, I heard you're trying to get back on track before graduation. What have you been working on?";
		AddChoice("I've been rebuilding my coding skills and working on my portfolio.", () => { var s = GlobalVars.Instance; s.Attributes.IncreaseConfidence(5); s.Attributes.IncreaseNetworking(5); s.PartyGoodChoices += 1; CompleteClassmate("Great answer. You sounded focused and motivated."); });
		AddChoice("Honestly, I have no idea. I'm just hoping things work out.", () => { GlobalVars.Instance.Attributes.DecreaseConfidence(3); CompleteClassmate("A hesitant answer made the conversation awkward."); });
		AddChoice("I don't really care about graduation anymore.", () => { var s = GlobalVars.Instance; s.Attributes.DecreaseConfidence(5); s.Attributes.DecreaseNetworking(3); CompleteClassmate("That answer hurt your impression."); });
	}

	private void StartAlumniDialogue()
	{
		if (_alumniDone) return;
		ClearChoices();
		_dialogueLabel.Text = "I'm helping a few students connect with graduate IT opportunities. What kind of role are you aiming for?";
		AddChoice("I'm aiming for a graduate developer role and preparing through projects and interview practice.", () => { var s = GlobalVars.Instance; s.Profile.IncreaseCareerReadiness(5); s.Attributes.IncreaseNetworking(10); s.PartyGoodChoices += 1; CompleteAlumni("Strong answer. You made a good professional impression."); });
		AddChoice("Anything that pays, I guess.", () => { GlobalVars.Instance.Attributes.DecreaseNetworking(2); CompleteAlumni("The answer sounded unclear and unprepared."); });
		AddChoice("I haven't prepared, but I want a job anyway.", () => { var s = GlobalVars.Instance; s.Attributes.DecreaseConfidence(3); s.Attributes.DecreaseNetworking(5); CompleteAlumni("That answer lowered confidence in your readiness."); });
	}

	private void CompleteClassmate(string message)
	{
		_classmateDone = true;
		_talkClassmateButton.Disabled = true;
		_statusLabel.Text = message;
		ClearChoices();
		UpdateFinishAvailability();
	}

	private void CompleteAlumni(string message)
	{
		_alumniDone = true;
		_talkAlumniButton.Disabled = true;
		_statusLabel.Text = message;
		ClearChoices();
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

		GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn");
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
}
