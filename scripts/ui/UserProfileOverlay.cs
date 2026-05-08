using Godot;

public partial class UserProfileOverlay : CanvasLayer
{
	private PanelContainer _userProfileOverlay;
	private Label _headerLabel;
	private Label _energyLabel;
	private Label _focusLabel;
	private Label _knowledgeLabel;
	private Label _confidenceLabel;
	private Label _careerReadinessLabel;
	private Label _networkingLabel;
	private Label _portfolioLabel;
	private Label _hintLabel;
	private Label _closeHintLabel;

	public override void _Ready()
	{
		_userProfileOverlay = GetNode<PanelContainer>("UserProfileOverlay");
		_headerLabel = GetNode<Label>("UserProfileOverlay/ProfileFrame/MarginContainer/ProfileContent/HeaderLabel");
		_energyLabel = GetNode<Label>("UserProfileOverlay/ProfileFrame/MarginContainer/ProfileContent/AttributesContainer/EnergyLabel");
		_focusLabel = GetNode<Label>("UserProfileOverlay/ProfileFrame/MarginContainer/ProfileContent/AttributesContainer/FocusLabel");
		_knowledgeLabel = GetNode<Label>("UserProfileOverlay/ProfileFrame/MarginContainer/ProfileContent/AttributesContainer/KnowledgeLabel");
		_confidenceLabel = GetNode<Label>("UserProfileOverlay/ProfileFrame/MarginContainer/ProfileContent/AttributesContainer/ConfidenceLabel");
		_careerReadinessLabel = GetNode<Label>("UserProfileOverlay/ProfileFrame/MarginContainer/ProfileContent/AttributesContainer/CareerReadinessLabel");
		_networkingLabel = GetNode<Label>("UserProfileOverlay/ProfileFrame/MarginContainer/ProfileContent/AttributesContainer/NetworkingLabel");
		_portfolioLabel = GetNode<Label>("UserProfileOverlay/ProfileFrame/MarginContainer/ProfileContent/AttributesContainer/PortfolioLabel");
		_hintLabel = GetNode<Label>("UserProfileOverlay/ProfileFrame/MarginContainer/ProfileContent/HintLabel");
		_closeHintLabel = GetNode<Label>("CloseHintLabel");

		_userProfileOverlay.Visible = false;
		SetMouseFilterRecursive(GetNode<Control>("UserProfileOverlay/ProfileFrame"), Control.MouseFilterEnum.Ignore);
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
			else if (keyEvent.Keycode == Key.Escape && _userProfileOverlay.Visible)
			{
				Toggle();
				GetViewport().SetInputAsHandled();
			}
		}
	}

	public override void _Process(double delta)
	{
		if (_userProfileOverlay.Visible)
			Refresh();
	}

	private void Toggle()
	{
		_userProfileOverlay.Visible = !_userProfileOverlay.Visible;
		if (_userProfileOverlay.Visible)
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
		_energyLabel.Text = $"Energy: {state.Attributes.Energy}";
		_focusLabel.Text = $"Focus: {state.Attributes.Focus}";
		_knowledgeLabel.Text = $"Knowledge: {state.Attributes.Knowledge}";
		_confidenceLabel.Text = $"Confidence: {state.Attributes.Confidence}";
		_careerReadinessLabel.Text = $"Career Readiness: {state.Profile.CareerReadiness}";
		_networkingLabel.Text = $"Networking: {state.Attributes.Networking}";
		_portfolioLabel.Text = $"Portfolio: {state.Attributes.Portfolio}";
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
