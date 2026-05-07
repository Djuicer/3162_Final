using Godot;

public partial class ComputerLab : Node2D
{
	private Area2D _pcInteractArea;
	private Area2D _exitInteractArea;
	private Label _interactPromptLabel;
	private Label _feedbackLabel;
	private bool _canUsePc;
	private bool _canUseExit;
	private bool _isTransitioning;

	public override void _Ready()
	{
		_pcInteractArea = GetNodeOrNull<Area2D>("PcInteractArea");
		_exitInteractArea = GetNodeOrNull<Area2D>("ExitInteractArea");
		_interactPromptLabel = GetNodeOrNull<Label>("UI/InteractPromptLabel");
		_feedbackLabel = GetNodeOrNull<Label>("UI/FeedbackLabel");

		if (_pcInteractArea != null)
		{
			_pcInteractArea.BodyEntered += OnPcAreaBodyEntered;
			_pcInteractArea.BodyExited += OnPcAreaBodyExited;
		}

		if (_exitInteractArea != null)
		{
			_exitInteractArea.BodyEntered += OnExitAreaBodyEntered;
			_exitInteractArea.BodyExited += OnExitAreaBodyExited;
		}

		if (_interactPromptLabel != null)
			_interactPromptLabel.Visible = false;

		SetFeedback("Interact with a PC to Study CS.");
	}

	public override void _Process(double delta)
	{
		if (_interactPromptLabel == null)
			return;

		if (_canUsePc)
		{
			_interactPromptLabel.Text = "Press E to Study CS";
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

		if (_canUsePc)
		{
			TryStartStudyCs();
			return;
		}

		if (_canUseExit)
		{
			_isTransitioning = true;
			GetTree().ChangeSceneToFile("res://scenes/Domitory/dormitory.tscn");
		}
	}

	private void TryStartStudyCs()
	{
		var state = GlobalVars.Instance;
		if (state.Profile.ActionsLeft <= 0)
		{
			SetFeedback("You have no actions left. Go back to your dorm and sleep.");
			return;
		}

		_isTransitioning = true;
		state.ReturnScenePath = "res://scenes/ComputerLab/computer_lab.tscn";
		GetTree().ChangeSceneToFile("res://scenes/Minigames/FocusCatch/focus_catch.tscn");
	}

	private void SetFeedback(string message)
	{
		if (_feedbackLabel != null)
			_feedbackLabel.Text = $"Hint: {message}";
	}

	private void OnPcAreaBodyEntered(Node2D body)
	{
		if (body is CharacterBody2D)
			_canUsePc = true;
	}

	private void OnPcAreaBodyExited(Node2D body)
	{
		if (body is CharacterBody2D)
			_canUsePc = false;
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
