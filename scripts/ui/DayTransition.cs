using Godot;
using System.Collections.Generic;

public partial class DayTransition : Control
{
	private static readonly Dictionary<int, string> DayMessages = new()
	{
		{ 2, "Another chance to turn things around." },
		{ 3, "You have a new email. Check your computer." },
		{ 5, "Someone in the Innovation Hub may have an opportunity." },
		{ 7, "Final day. Make it count." }
	};

	private Label _dayLabel;
	private Label _messageLabel;
	private Button _continueButton;
	private bool _isContinuing;

	public override async void _Ready()
	{
		_dayLabel = GetNodeOrNull<Label>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/DayLabel");
		_messageLabel = GetNodeOrNull<Label>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/MessageLabel");
		_continueButton = GetNodeOrNull<Button>("CenterContainer/PanelContainer/MarginContainer/VBoxContainer/ContinueButton");

		if (_continueButton != null)
			_continueButton.Pressed += ContinueToDormitory;

		var state = GlobalVars.Instance;
		if (state == null)
			return;

		if (state.Profile.CurrentDay > state.Profile.FinalDay)
		{
			TransitionManager.Instance?.ChangeSceneToFileWithFade("res://scenes/Ending/ending.tscn");
			return;
		}

		if (_dayLabel != null)
			_dayLabel.Text = $"Day {state.Profile.CurrentDay}";

		if (_messageLabel != null)
			_messageLabel.Text = DayMessages.TryGetValue(state.Profile.CurrentDay, out string message)
				? message
				: "A new day begins.";

		await ToSignal(GetTree().CreateTimer(1.5f), SceneTreeTimer.SignalName.Timeout);
		ContinueToDormitory();
	}

	private void ContinueToDormitory()
	{
		if (_isContinuing)
			return;
		_isContinuing = true;
		TransitionManager.Instance?.ChangeSceneToFileWithFade("res://scenes/Domitory/dormitory.tscn");
	}
}
