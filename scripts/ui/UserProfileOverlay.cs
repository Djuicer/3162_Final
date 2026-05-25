using Godot;

public partial class UserProfileOverlay : CanvasLayer
{
	private PanelContainer _profilePanel;
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
		_profilePanel = GetNode<PanelContainer>("ProfilePanel");
		_headerLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/HeaderLabel");
		_energyLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/AttributesContainer/EnergyRow/EnergyLabel");
		_focusLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/AttributesContainer/FocusRow/FocusLabel");
		_knowledgeLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/AttributesContainer/KnowledgeRow/KnowledgeLabel");
		_confidenceLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/AttributesContainer/ConfidenceRow/ConfidenceLabel");
		_careerReadinessLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/AttributesContainer/CareerReadinessRow/CareerReadinessLabel");
		_networkingLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/AttributesContainer/NetworkingRow/NetworkingLabel");
		_portfolioLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/AttributesContainer/PortfolioRow/PortfolioLabel");
		_hintLabel = GetNode<Label>("ProfilePanel/MarginContainer/ProfileContent/HintLabel");
		_closeHintLabel = GetNode<Label>("CloseHintLabel");

		_profilePanel.Visible = false;
		SetMouseFilterRecursive(GetNode<Control>("ProfilePanel"), Control.MouseFilterEnum.Ignore);
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
