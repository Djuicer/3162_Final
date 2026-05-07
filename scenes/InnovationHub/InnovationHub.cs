using Godot;

public partial class InnovationHub : Node2D
{
	private CharacterBody2D _player;
	private Area2D _workstationInteractArea;
	private Area2D _exitInteractArea;
	private Area2D _developerStudentArea;
	private ColorRect _developerStudentVisual;
	private Label _interactPromptLabel;
	private Label _feedbackLabel;
	private Label _npcNameLabel;
	private Label _dialogueLabel;
	private VBoxContainer _dialogueChoices;
	private bool _canUseWorkstation;
	private bool _canUseExit;
	private bool _canTalkToDeveloperStudent;
	private bool _isTransitioning;
	private bool _dialogueOpen;

	public override void _Ready()
	{
		StatusOverlay.AttachTo(this, false);
		_workstationInteractArea = GetNodeOrNull<Area2D>("WorkstationInteractArea");
		_exitInteractArea = GetNodeOrNull<Area2D>("ExitInteractArea");
		_player = GetNodeOrNull<CharacterBody2D>("Player");
		_developerStudentArea = GetNodeOrNull<Area2D>("DeveloperStudentInteractArea");
		_developerStudentVisual = GetNodeOrNull<ColorRect>("DeveloperStudentVisual");
		_interactPromptLabel = GetNodeOrNull<Label>("UI/InteractPromptLabel");
		_feedbackLabel = GetNodeOrNull<Label>("UI/FeedbackLabel");
		_npcNameLabel = GetNodeOrNull<Label>("UI/GuidancePanel/NpcNameLabel");
		_dialogueLabel = GetNodeOrNull<Label>("UI/GuidancePanel/DialogueLabel");
		_dialogueChoices = GetNodeOrNull<VBoxContainer>("UI/GuidancePanel/DialogueChoices");

		if (_workstationInteractArea != null)
		{
			_workstationInteractArea.BodyEntered += OnWorkstationAreaBodyEntered;
			_workstationInteractArea.BodyExited += OnWorkstationAreaBodyExited;
		}

		if (_exitInteractArea != null)
		{
			_exitInteractArea.BodyEntered += OnExitAreaBodyEntered;
			_exitInteractArea.BodyExited += OnExitAreaBodyExited;
		}
		if (_developerStudentArea != null)
		{
			_developerStudentArea.BodyEntered += OnDeveloperStudentAreaBodyEntered;
			_developerStudentArea.BodyExited += OnDeveloperStudentAreaBodyExited;
		}

		if (_interactPromptLabel != null)
			_interactPromptLabel.Visible = false;
		ApplyPendingSpawnIfAny();

		bool day5Available = GlobalVars.Instance.Profile.CurrentDay >= 5;
		if (_developerStudentVisual != null)
			_developerStudentVisual.Visible = day5Available;
		if (_developerStudentArea != null)
			_developerStudentArea.Monitoring = day5Available;

		SetFeedback("Interact with the workstation to Practice Coding.");
	}

	public override void _Process(double delta)
	{
		if (_interactPromptLabel == null)
			return;

		if (_canUseWorkstation && !_dialogueOpen)
		{
			_interactPromptLabel.Text = "Press E to Practice Coding";
			_interactPromptLabel.Visible = true;
			return;
		}
		if (_canTalkToDeveloperStudent && !_dialogueOpen)
		{
			_interactPromptLabel.Text = "Press E to talk";
			_interactPromptLabel.Visible = true;
			return;
		}

		if (_canUseExit && !_dialogueOpen)
		{
			_interactPromptLabel.Text = "Press E to return to Dormitory";
			_interactPromptLabel.Visible = true;
			return;
		}

		_interactPromptLabel.Visible = false;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!@event.IsActionPressed("interact") || _isTransitioning)
			return;

		if (_canUseWorkstation)
		{
			TryStartCodingPractice();
			return;
		}
		if (_canTalkToDeveloperStudent)
		{
			OpenDeveloperStudentDialogue();
			return;
		}

		if (_canUseExit)
		{
			_isTransitioning = true;
			GlobalVars.Instance.QueuePendingSpawn("DoorReturnSpawn");
			GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn");
		}
	}

	private void TryStartCodingPractice()
	{
		var state = GlobalVars.Instance;
		if (state.Profile.ActionsLeft <= 0)
		{
			SetFeedback("You have no actions left. Go back to your dorm and sleep.");
			return;
		}

		_isTransitioning = true;
		state.SetReturnContext("res://scenes/InnovationHub/innovation_hub.tscn", "InnovationHubWorkstationReturnSpawn");
		GetTree().ChangeSceneToFile("res://scenes/Minigames/BugSquash/bug_squash.tscn");
	}

	private void ApplyPendingSpawnIfAny()
	{
		var state = GlobalVars.Instance;
		if (state == null || _player == null || string.IsNullOrEmpty(state.PendingSpawnId))
			return;

		var marker = GetNodeOrNull<Marker2D>(state.PendingSpawnId);
		if (marker != null)
			_player.GlobalPosition = marker.GlobalPosition;
		state.PendingSpawnId = "";
	}

	private void SetFeedback(string message)
	{
		if (_feedbackLabel != null)
			_feedbackLabel.Text = $"Hint: {message}";
	}

	private void OpenDeveloperStudentDialogue()
	{
		var state = GlobalVars.Instance;
		if (state.Profile.CurrentDay < 5)
			return;

		if (state.Day5NpcMet)
		{
			SetFeedback(state.ITBallInvited
				? "We already talked. Check your email for the IT Ball details."
				: "Maybe another opportunity will come later.");
			return;
		}

		_dialogueOpen = true;
		if (_npcNameLabel != null) _npcNameLabel.Text = "Developer Student";
		if (_dialogueLabel != null) _dialogueLabel.Text = "I've seen you around the project room. A few of us are going to the IT Ball to meet developers and look for people to join an app project. What are you trying to build?";
		ClearDialogueChoices();
		AddDialogueChoice("I've been improving my coding skills and building a portfolio. I want to contribute to a real project.", () =>
		{
			var s = GlobalVars.Instance;
			s.Attributes.IncreaseConfidence(5);
			s.Attributes.IncreaseNetworking(10);
			s.Attributes.IncreasePortfolio(5);
			s.Day5NpcMet = true;
			s.ITBallInvited = true;
			EndDeveloperDialogue("The developer student is impressed and invites you to the IT Ball.");
		});
		AddDialogueChoice("I'm still figuring things out, but I want to learn from people who are building real projects.", () =>
		{
			var s = GlobalVars.Instance;
			s.Attributes.IncreaseConfidence(2);
			s.Attributes.IncreaseNetworking(5);
			s.Day5NpcMet = true;
			s.ITBallInvited = true;
			EndDeveloperDialogue("The developer student sees potential and invites you to the IT Ball.");
		});
		AddDialogueChoice("I mostly just want quick money. I don't really care what the project is.", () =>
		{
			var s = GlobalVars.Instance;
			s.Attributes.DecreaseConfidence(3);
			s.Attributes.DecreaseNetworking(5);
			s.Day5NpcMet = true;
			s.ITBallInvited = false;
			EndDeveloperDialogue("The developer student does not think you are ready for the opportunity.");
		});
	}

	private void EndDeveloperDialogue(string message)
	{
		SetFeedback(message);
		_dialogueOpen = false;
		if (_npcNameLabel != null) _npcNameLabel.Text = "";
		if (_dialogueLabel != null) _dialogueLabel.Text = "";
		ClearDialogueChoices();
	}

	private void AddDialogueChoice(string text, System.Action onPressed)
	{
		if (_dialogueChoices == null)
			return;
		var button = new Button { Text = text, CustomMinimumSize = new Vector2(0, 36) };
		button.Pressed += () => onPressed();
		_dialogueChoices.AddChild(button);
	}

	private void ClearDialogueChoices()
	{
		if (_dialogueChoices == null)
			return;
		foreach (Node child in _dialogueChoices.GetChildren())
			child.QueueFree();
	}

	private void OnWorkstationAreaBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D)
			_canUseWorkstation = true;
	}

	private void OnWorkstationAreaBodyExited(Node2D body)
	{
		if (body is CharacterBody2D)
			_canUseWorkstation = false;
	}

	private void OnExitAreaBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D)
			_canUseExit = true;
	}

	private void OnExitAreaBodyExited(Node2D body)
	{
		if (body is CharacterBody2D)
			_canUseExit = false;
	}

	private void OnDeveloperStudentAreaBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D && GlobalVars.Instance.Profile.CurrentDay >= 5)
			_canTalkToDeveloperStudent = true;
	}

	private void OnDeveloperStudentAreaBodyExited(Node2D body)
	{
		if (body is CharacterBody2D)
			_canTalkToDeveloperStudent = false;
	}
}
