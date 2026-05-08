using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class DanceParty : Node2D
{
	private enum PartyNpc { None, Classmate, Alumni }
	private Area2D _classmateArea;
	private Area2D _alumniArea;
	private Label _interactPromptLabel;
	private Label _instructionLabel;
	private Label _statusLabel;
	private Button _finishPartyButton;
	private CharacterBody2D _player;
	private DialogueBoxUI _dialogueUi;
	private bool _classmateDone;
	private bool _alumniDone;
	private bool _dialogueOpen;
	private PartyNpc _nearNpc = PartyNpc.None;

	public override void _Ready()
	{
		StatusOverlay.AttachTo(this, false);
		_classmateArea = GetNode<Area2D>("DancePartyClassmateNPC/InteractionArea");
		_alumniArea = GetNode<Area2D>("DancePartyRecruiterNPC/InteractionArea");
		_player = GetNodeOrNull<CharacterBody2D>("Player");
		_interactPromptLabel = GetNode<Label>("UI/InteractPromptLabel");
		_instructionLabel = GetNode<Label>("UI/DialoguePanel/ContentContainer/InstructionLabel");
		_statusLabel = GetNode<Label>("UI/DialoguePanel/ContentContainer/StatusLabel");
		_finishPartyButton = GetNode<Button>("UI/DialoguePanel/ContentContainer/ActionRow/FinishPartyButton");
		_dialogueUi = GD.Load<PackedScene>("res://scenes/UI/Dialogue/dialogue_box.tscn").Instantiate<DialogueBoxUI>();
		AddChild(_dialogueUi);
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
		_interactPromptLabel.Visible = false;
		_finishPartyButton.Disabled = true;
	}
	public override void _Process(double delta){_interactPromptLabel.Visible=!_dialogueOpen&&_nearNpc!=PartyNpc.None;if(_interactPromptLabel.Visible)_interactPromptLabel.Text="Press E to talk";}
	public override void _UnhandledInput(InputEvent @event){if(!_dialogueOpen&&@event.IsActionPressed("interact")){if(_nearNpc==PartyNpc.Classmate)_=OpenClassmateDialogueAsync();else if(_nearNpc==PartyNpc.Alumni)_=OpenAlumniDialogueAsync();}}
	private async Task OpenClassmateDialogueAsync(){if(_classmateDone){await _dialogueUi.ShowLineAsync("Classmate","We already talked.");_dialogueUi.HideAll();return;}await RunDialogueAsync("Classmate","Hey, I heard you're trying to get back on track before graduation. What have you been working on?",new List<string>{"I've been rebuilding my coding skills and working on my portfolio.","Honestly, I have no idea. I'm just hoping things work out.","I don't really care about graduation anymore."},i=>{if(i==0){var s=GlobalVars.Instance;s.Attributes.IncreaseConfidence(5);s.Attributes.IncreaseNetworking(5);s.PartyGoodChoices+=1;return "Great answer. You sounded focused and motivated.";}if(i==1){GlobalVars.Instance.Attributes.DecreaseConfidence(3);return "A hesitant answer made the conversation awkward.";}var ss=GlobalVars.Instance;ss.Attributes.DecreaseConfidence(5);ss.Attributes.DecreaseNetworking(3);return "That answer hurt your impression.";});_classmateDone=true;UpdateFinishAvailability();}
	private async Task OpenAlumniDialogueAsync(){if(_alumniDone){await _dialogueUi.ShowLineAsync("Alumni / Recruiter","We already talked.");_dialogueUi.HideAll();return;}await RunDialogueAsync("Alumni / Recruiter","I'm helping a few students connect with graduate IT opportunities. What kind of role are you aiming for?",new List<string>{"I'm aiming for a graduate developer role and preparing through projects and interview practice.","Anything that pays, I guess.","I haven't prepared, but I want a job anyway."},i=>{if(i==0){var s=GlobalVars.Instance;s.Profile.IncreaseCareerReadiness(5);s.Attributes.IncreaseNetworking(10);s.PartyGoodChoices+=1;return "Strong answer. You made a good professional impression.";}if(i==1){GlobalVars.Instance.Attributes.DecreaseNetworking(2);return "The answer sounded unclear and unprepared.";}var ss=GlobalVars.Instance;ss.Attributes.DecreaseConfidence(3);ss.Attributes.DecreaseNetworking(5);return "That answer lowered confidence in your readiness.";});_alumniDone=true;UpdateFinishAvailability();}
	private async Task RunDialogueAsync(string npc,string prompt,List<string> choices,System.Func<int,string> apply){_dialogueOpen=true;SetPlayerControl(false);await _dialogueUi.ShowLineAsync(npc,prompt);int idx=await _dialogueUi.ShowChoicesAsync(choices);await _dialogueUi.ShowLineAsync("You",choices[idx]);_statusLabel.Text=apply(idx);await _dialogueUi.ShowLineAsync(npc,_statusLabel.Text);_dialogueUi.HideAll();_dialogueOpen=false;SetPlayerControl(true);} 
	private void SetPlayerControl(bool enabled){if(_player!=null)_player.SetPhysicsProcess(enabled);} 
	private void UpdateFinishAvailability(){if(_classmateDone&&_alumniDone)_finishPartyButton.Disabled=false;}
	private void FinishParty(){var state=GlobalVars.Instance;if(state.PartyGoodChoices>=2){state.PartyNetworkingUnlocked=true;_statusLabel.Text="You made a strong impression. This connection may help you later.";}else{state.PartyNetworkingUnlocked=false;_statusLabel.Text="You attended the party, but did not make a strong connection.";}state.PartyEventCompleted=true;state.ShowReaction(state.PartyGoodChoices>=2?"Maybe I'm not as alone in this as I thought.":"I showed up. Next time I'll connect better.");TransitionManager.Instance?.ChangeSceneToFileWithFade("res://scenes/Domitory/dormitory.tscn");}
	private void OnNpcAreaEntered(PartyNpc npc){_nearNpc=npc;} private void OnNpcAreaExited(PartyNpc npc){if(_nearNpc==npc)_nearNpc=PartyNpc.None;}
}
