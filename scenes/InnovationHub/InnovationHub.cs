using Godot;

public partial class InnovationHub : Node2D
{
	private Area2D _workstationInteractArea;
	private Area2D _exitInteractArea;
	private Label _interactPromptLabel;
	private Label _feedbackLabel;
	private bool _canUseWorkstation;
	private bool _canUseExit;
	private bool _isTransitioning;

	public override void _Ready()
	{
		_workstationInteractArea = GetNodeOrNull<Area2D>("WorkstationInteractArea");
		_exitInteractArea = GetNodeOrNull<Area2D>("ExitInteractArea");
		_interactPromptLabel = GetNodeOrNull<Label>("UI/InteractPromptLabel");
		_feedbackLabel = GetNodeOrNull<Label>("UI/FeedbackLabel");

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

		if (_interactPromptLabel != null)
			_interactPromptLabel.Visible = false;

		SetFeedback("Interact with the workstation to Practice Coding.");
	}

	public override void _Process(double delta)
	{
		if (_interactPromptLabel == null)
			return;

		if (_canUseWorkstation)
		{
			_interactPromptLabel.Text = "Press E to Practice Coding";
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

		if (_canUseWorkstation)
		{
			TryStartCodingPractice();
			return;
		}

		if (_canUseExit)
		{
			_isTransitioning = true;
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
		state.ReturnScenePath = "res://scenes/InnovationHub/innovation_hub.tscn";
		GetTree().ChangeSceneToFile("res://scenes/Minigames/BugSquash/bug_squash.tscn");
	}

	private void SetFeedback(string message)
	{
		if (_feedbackLabel != null)
			_feedbackLabel.Text = $"Hint: {message}";
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
}
