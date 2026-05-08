using Godot;

public partial class ITBall : Node2D
{
	private enum NpcType { None, AppDeveloper, ProjectLead }
	private Area2D _appDevArea;
	private Area2D _leadArea;
	private Label _promptLabel;
	private Label _npcNameLabel;
	private Label _dialogueLabel;
	private Label _statusLabel;
	private VBoxContainer _choices;
	private Button _finishButton;
	private bool _dialogueOpen;
	private NpcType _nearNpc = NpcType.None;
	private bool _appDevDone;
	private bool _leadDone;

	public override void _Ready()
	{
		StatusOverlay.AttachTo(this, false);
		_appDevArea = GetNode<Area2D>("ITBallAppDeveloperNPC/InteractionArea");
		_leadArea = GetNode<Area2D>("ITBallProjectLeadNPC/InteractionArea");
		_promptLabel = GetNode<Label>("UI/InteractPromptLabel");
		_npcNameLabel = GetNode<Label>("UI/Panel/VBox/NpcNameLabel");
		_dialogueLabel = GetNode<Label>("UI/Panel/VBox/DialogueLabel");
		_statusLabel = GetNode<Label>("UI/Panel/VBox/StatusLabel");
		_choices = GetNode<VBoxContainer>("UI/Panel/VBox/Choices");
		_finishButton = GetNode<Button>("UI/Panel/VBox/ActionRow/FinishEventButton");

		_appDevArea.BodyEntered += _ => _nearNpc = NpcType.AppDeveloper;
		_appDevArea.BodyExited += _ => { if (_nearNpc == NpcType.AppDeveloper) _nearNpc = NpcType.None; };
		_leadArea.BodyEntered += _ => _nearNpc = NpcType.ProjectLead;
		_leadArea.BodyExited += _ => { if (_nearNpc == NpcType.ProjectLead) _nearNpc = NpcType.None; };
		_finishButton.Pressed += OnFinishEventPressed;

		GlobalVars.Instance.ITBallGoodChoices = 0;
		_finishButton.Disabled = true;
		_promptLabel.Visible = false;
		_statusLabel.Text = "Talk to both developers before finishing the event.";
	}

	public override void _Process(double delta)
	{
		_promptLabel.Visible = !_dialogueOpen && _nearNpc != NpcType.None;
		if (_promptLabel.Visible)
			_promptLabel.Text = "Press E to talk";
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!_dialogueOpen && @event.IsActionPressed("interact"))
		{
			if (_nearNpc == NpcType.AppDeveloper) OpenAppDeveloperDialogue();
			if (_nearNpc == NpcType.ProjectLead) OpenProjectLeadDialogue();
		}
	}

	private void OpenAppDeveloperDialogue()
	{
		if (_appDevDone) { _statusLabel.Text = "You already spoke with them."; return; }
		_dialogueOpen = true;
		_npcNameLabel.Text = "App Developer";
		_dialogueLabel.Text = "We're building a student productivity app, but we need someone reliable. What can you contribute?";
		ClearChoices();
		AddChoice("I've been practicing consistently, fixing bugs, and building my portfolio. I can help implement and test features.", () => { var s = GlobalVars.Instance; s.Attributes.IncreasePortfolio(10); s.Attributes.IncreaseNetworking(10); s.Attributes.IncreaseConfidence(5); s.ITBallGoodChoices += 1; CompleteAppDev(); });
		AddChoice("I'm still learning, but I'm willing to take small tasks and improve.", () => { var s = GlobalVars.Instance; s.Attributes.IncreasePortfolio(3); s.Attributes.IncreaseNetworking(5); s.Attributes.IncreaseConfidence(2); CompleteAppDev(); });
		AddChoice("I mostly want to join if it makes money quickly.", () => { var s = GlobalVars.Instance; s.Attributes.DecreaseConfidence(5); s.Attributes.DecreaseNetworking(5); CompleteAppDev(); });
	}

	private void OpenProjectLeadDialogue()
	{
		if (_leadDone) { _statusLabel.Text = "You already spoke with them."; return; }
		_dialogueOpen = true;
		_npcNameLabel.Text = "Project Lead";
		_dialogueLabel.Text = "We need someone who can work with others and finish what they start. Why should we trust you?";
		ClearChoices();
		AddChoice("I've been rebuilding discipline this week. I know I still have a lot to learn, but I can communicate, show up, and deliver small features.", () => { var s = GlobalVars.Instance; s.Attributes.IncreasePortfolio(10); s.Attributes.IncreaseNetworking(10); s.Attributes.IncreaseConfidence(5); s.ITBallGoodChoices += 1; CompleteLead(); });
		AddChoice("I can try, but I might need help staying on track.", () => { var s = GlobalVars.Instance; s.Attributes.IncreaseConfidence(2); s.Attributes.IncreaseNetworking(3); CompleteLead(); });
		AddChoice("I prefer working alone and I don't like feedback.", () => { var s = GlobalVars.Instance; s.Attributes.DecreaseConfidence(5); s.Attributes.DecreaseNetworking(5); CompleteLead(); });
	}

	private void CompleteAppDev() { _appDevDone = true; CloseDialogue("You finished talking with the app developer."); }
	private void CompleteLead() { _leadDone = true; CloseDialogue("You finished talking with the project lead."); }

	private void CloseDialogue(string message)
	{
		_statusLabel.Text = message;
		_dialogueOpen = false;
		_npcNameLabel.Text = "";
		_dialogueLabel.Text = "";
		ClearChoices();
		if (_appDevDone && _leadDone)
			_finishButton.Disabled = false;
	}

	private void OnFinishEventPressed()
	{
		var s = GlobalVars.Instance;
		bool goodDialogue = s.ITBallGoodChoices >= 2;
		bool skillReady = s.Attributes.Knowledge >= 60;
		bool portfolioReady = s.Attributes.Portfolio >= 40;
		bool confidenceReady = s.Attributes.Confidence >= 40;
		bool networkingReady = s.Attributes.Networking >= 30;

		if (goodDialogue && skillReady && portfolioReady && confidenceReady && networkingReady)
		{
			s.JoinedDeveloperGroup = true;
			_statusLabel.Text = "The developer group is impressed. They invite you to join their app project.";
		}
		else if (goodDialogue)
		{
			s.JoinedDeveloperGroup = false;
			_statusLabel.Text = "You made a good impression, but you need stronger skills and portfolio work before joining the project.";
		}
		else
		{
			s.JoinedDeveloperGroup = false;
			_statusLabel.Text = "The developers decide you are not ready for the project yet.";
		}

		GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn");
	}

	private void AddChoice(string text, System.Action fn)
	{
		var b = new Button { Text = text, CustomMinimumSize = new Vector2(0, 36) };
		b.Pressed += () => fn();
		_choices.AddChild(b);
	}

	private void ClearChoices()
	{
		foreach (Node child in _choices.GetChildren())
			child.QueueFree();
	}
}
