using Godot;

public partial class ComputerScreen : Node2D
{
	private Label _profileLabel;
	private Label _notesLabel;
	private Label _feedbackLabel;
	private TextureButton _procrastinateButton;
	private TextureButton _emailButton;
	private TextureButton _returnButton;
	private bool _isTransitioning;

	public override void _Ready()
	{
		StatusOverlay.AttachTo(this, false);
		GetNodes();
		ConnectButtons();
		RefreshUi();
		ShowFeedback("Check your profile, read email, or procrastinate for a quick break.");
	}

	private void GetNodes()
	{
		_procrastinateButton = GetNodeOrNull<TextureButton>("DesktopButtons/BrowserButton");
		_emailButton = GetNodeOrNull<TextureButton>("DesktopButtons/EmailButton");
		_returnButton = GetNodeOrNull<TextureButton>("DesktopButtons/ReturnButton");

		_profileLabel = GetNodeOrNull<Label>("ComputerInfoPanel/MarginContainer/ContentContainer/ProfileLabel");
		_notesLabel = GetNodeOrNull<Label>("ComputerInfoPanel/MarginContainer/ContentContainer/NotesLabel");
		_feedbackLabel = GetNodeOrNull<Label>("ComputerInfoPanel/MarginContainer/ContentContainer/FeedbackLabel");
	}

	private void ConnectButtons()
	{
		if (_procrastinateButton != null)
			_procrastinateButton.Pressed += OnProcrastinatePressed;
		if (_emailButton != null)
			_emailButton.Pressed += OnEmailPressed;
		if (_returnButton != null)
			_returnButton.Pressed += OnBackPressed;
	}

	private void OnBackPressed()
	{
		if (_isTransitioning)
			return;
		_isTransitioning = true;
		var state = GlobalVars.Instance;
		string returnScene = state?.ReturnScenePath;
		if (string.IsNullOrEmpty(returnScene))
			returnScene = "res://scenes/Domitory/dormitory.tscn";
		if (!string.IsNullOrEmpty(state?.ReturnSpawnId))
			state.QueuePendingSpawn(state.ReturnSpawnId);
		TransitionManager.Instance?.ChangeSceneToFileWithFade(returnScene);
	}
	private void OnEmailPressed() { if (!_isTransitioning) { _isTransitioning = true; TransitionManager.Instance?.ChangeSceneToFileWithFade("res://scenes/Computer/Email/email.tscn"); } }

	private void OnProcrastinatePressed()
	{
		var state = GlobalVars.Instance;
		if (!state.Profile.TrySpendAction(1))
		{
			ShowFeedback("You have no actions left. Go to bed and sleep.");
			RefreshUi();
			return;
		}

		state.Attributes.IncreaseEnergy(10);
		state.Attributes.DecreaseConfidence(5);
		state.Attributes.DecreaseFocus(5);
		RefreshUi();
		ShowFeedback("You procrastinated for a while. You feel slightly rested, but less confident.");
		state.ShowReaction("I rested a bit, but I don't feel great about wasting time.");
	}

	private void RefreshUi()
	{
		var state = GlobalVars.Instance;
		var profile = state.Profile;

		if (_profileLabel != null)
			_profileLabel.Text = $"Name: {profile.PlayerName}\nDay: {profile.CurrentDay} / {profile.FinalDay}\nActions Left: {profile.ActionsLeft} / {profile.MaxActionsPerDay}\nGoal: {profile.Goal}";
		if (_notesLabel != null)
			_notesLabel.Text = "Notes:\n- Check email for opportunities.\n- Procrastinate if you need a small Energy boost.\n- Use the dorm door to travel to the Computer Lab, Innovation Hub, or Career Centre.\n- Sleep at your bed when you run out of actions.";
	}

	private void ShowFeedback(string message)
	{
		if (_feedbackLabel != null)
			_feedbackLabel.Text = $"Feedback: {message}";
	}
}
