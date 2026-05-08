using Godot;

public partial class CareerCentre : Node2D
{
	private CharacterBody2D _player;
	private Area2D _interviewInteractArea;
	private Area2D _exitInteractArea;
	private Label _interactPromptLabel;
	private Label _feedbackLabel;
	private bool _canUseInterviewBooth;
	private bool _canUseExit;
	private bool _isTransitioning;

	public override void _Ready()
	{
		StatusOverlay.AttachTo(this, false);
		_interviewInteractArea = GetNodeOrNull<Area2D>("CareerCentreInterviewBooth/InteractionArea");
		_exitInteractArea = GetNodeOrNull<Area2D>("ExitInteractArea");
		_player = GetNodeOrNull<CharacterBody2D>("Player");
		_interactPromptLabel = GetNodeOrNull<Label>("UI/InteractPromptLabel");
		_feedbackLabel = GetNodeOrNull<Label>("UI/FeedbackLabel");

		if (_interviewInteractArea != null)
		{
			_interviewInteractArea.BodyEntered += OnInterviewAreaBodyEntered;
			_interviewInteractArea.BodyExited += OnInterviewAreaBodyExited;
		}

		if (_exitInteractArea != null)
		{
			_exitInteractArea.BodyEntered += OnExitAreaBodyEntered;
			_exitInteractArea.BodyExited += OnExitAreaBodyExited;
		}

		if (_interactPromptLabel != null)
			_interactPromptLabel.Visible = false;
		ApplyPendingSpawnIfAny();

		SetFeedback("Interact with the interview booth to Practice Interview.");
	}

	public override void _Process(double delta)
	{
		if (_interactPromptLabel == null)
			return;

		if (_canUseInterviewBooth)
		{
			_interactPromptLabel.Text = "Press E to Practice Interview";
			_interactPromptLabel.Visible = true;
			return;
		}

		if (_canUseExit)
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

		if (_canUseInterviewBooth)
		{
			TryStartInterviewPractice();
			return;
		}

		if (_canUseExit)
		{
			_isTransitioning = true;
			GlobalVars.Instance.QueuePendingSpawn("DoorReturnSpawn");
			TransitionManager.Instance?.ChangeSceneToFileWithFade("res://scenes/Domitory/dormitory.tscn");
		}
	}

	private void TryStartInterviewPractice()
	{
		var state = GlobalVars.Instance;
		if (state.Profile.ActionsLeft <= 0)
		{
			SetFeedback("You have no actions left. Go back to your dorm and sleep.");
			return;
		}

		_isTransitioning = true;
		state.SetReturnContext("res://scenes/CareerCentre/career_centre.tscn", "CareerCentreBoothReturnSpawn");
		TransitionManager.Instance?.ChangeSceneToFileWithFade("res://scenes/Minigames/InterviewRhythm/interview_rhythm.tscn");
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

	private void OnInterviewAreaBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D)
			_canUseInterviewBooth = true;
	}

	private void OnInterviewAreaBodyExited(Node2D body)
	{
		if (body is CharacterBody2D)
			_canUseInterviewBooth = false;
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
}
