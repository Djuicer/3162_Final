using Godot;

public partial class UserProfileOverlay : CanvasLayer
{
	private PanelContainer _profilePanel;
	private Label _headerLabel;
	private Label _profileInfoLabel;
	private Label _attributesLabel;
	private Label _routeStatusLabel;
	private Label _hintLabel;
	private Label _closeHintLabel;

	public override void _Ready()
	{
		_profilePanel = GetNode<PanelContainer>("ProfilePanel");
		_headerLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/HeaderLabel");
		_profileInfoLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/ProfileInfoContainer/ProfileInfoLabel");
		_attributesLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/AttributesContainer/AttributesLabel");
		_routeStatusLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/RouteStatusContainer/RouteStatusLabel");
		_hintLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/HintLabel");
		_closeHintLabel = GetNode<Label>("CloseHintLabel");

		_profilePanel.Visible = false;
		SetMouseFilterRecursive(GetNode<Control>("ProfilePanel/ProfileFrame"), Control.MouseFilterEnum.Ignore);
		SetMouseFilterRecursive(GetNode<Control>("ProfilePanel/MarginContainer"), Control.MouseFilterEnum.Ignore);
		Refresh();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey keyEvent && keyEvent.Pressed && !keyEvent.Echo)
		{
			if (keyEvent.Keycode == Key.Tab)
			{
				Toggle();
				GetViewport().SetInputAsHandled();
			}
			else if (keyEvent.Keycode == Key.Escape && _profilePanel.Visible)
			{
				Toggle();
				GetViewport().SetInputAsHandled();
			}
		}
	}

	public override void _Process(double delta)
	{
		if (_profilePanel.Visible)
			Refresh();
	}

	private void Toggle()
	{
		_profilePanel.Visible = !_profilePanel.Visible;
		if (_profilePanel.Visible)
		{
			Refresh();
			_closeHintLabel.Visible = true;
		}
		else
		{
			_closeHintLabel.Visible = false;
		}
	}

	private void Refresh()
	{
		if (GlobalVars.Instance == null)
			return;

		var state = GlobalVars.Instance;
		_headerLabel.Text = "User Profile";
		_profileInfoLabel.Text =
			$"Current Day: {state.Profile.CurrentDay} / {state.Profile.FinalDay}\n" +
			$"Actions Left: {state.Profile.ActionsLeft} / {state.Profile.MaxActionsPerDay}\n" +
			$"Career Readiness: {state.Profile.CareerReadiness}";

		_attributesLabel.Text =
			$"Energy: {state.Attributes.Energy}\n" +
			$"Focus: {state.Attributes.Focus}\n" +
			$"Knowledge: {state.Attributes.Knowledge}\n" +
			$"Confidence: {state.Attributes.Confidence}\n" +
			$"Networking: {state.Attributes.Networking}\n" +
			$"Portfolio: {state.Attributes.Portfolio}";

		_routeStatusLabel.Text =
			$"Dance Party connection: {(state.PartyNetworkingUnlocked ? "Yes" : "No")}\n" +
			$"IT Ball invite: {(state.ITBallInvited ? "Yes" : "No")}\n" +
			$"Developer group joined: {(state.JoinedDeveloperGroup ? "Yes" : "No")}";

		_hintLabel.Text = "Press Tab (or Esc) to close";
	}

	private static void SetMouseFilterRecursive(Node node, Control.MouseFilterEnum mouseFilter)
	{
		if (node is Control control)
			control.MouseFilter = mouseFilter;

		foreach (Node child in node.GetChildren())
			SetMouseFilterRecursive(child, mouseFilter);
	}
}
