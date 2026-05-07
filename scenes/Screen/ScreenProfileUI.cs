using Godot;

public partial class ScreenProfileUI : Node2D
{
	private int _currentSlot = -1;
	private PlayerProfile _profile;
	private PlayerAttributes _attributes;
	private StudentActionService _actions;

	private Label _playerNameLabel;
	private Label _dayLabel;
	private Label _goalLabel;
	private Label _readinessLabel;
	private Label _focusLabel;
	private Label _energyLabel;
	private Label _knowledgeLabel;
	private Label _confidenceLabel;

	public override void _Ready()
	{
		CacheNodes();
		LoadProfileAndAttributes();
		_actions = new StudentActionService(_profile, _attributes);
		WireButtons();
		RefreshUI();
	}

	private void CacheNodes()
	{
		_playerNameLabel = GetNode<Label>("ProfilePanel/Margin/VBox/PlayerNameLabel");
		_dayLabel = GetNode<Label>("ProfilePanel/Margin/VBox/DayLabel");
		_goalLabel = GetNode<Label>("ProfilePanel/Margin/VBox/GoalLabel");
		_readinessLabel = GetNode<Label>("ProfilePanel/Margin/VBox/CareerReadinessLabel");
		_focusLabel = GetNode<Label>("ProfilePanel/Margin/VBox/FocusLabel");
		_energyLabel = GetNode<Label>("ProfilePanel/Margin/VBox/EnergyLabel");
		_knowledgeLabel = GetNode<Label>("ProfilePanel/Margin/VBox/KnowledgeLabel");
		_confidenceLabel = GetNode<Label>("ProfilePanel/Margin/VBox/ConfidenceLabel");
	}

	private void LoadProfileAndAttributes()
	{
		_currentSlot = GlobalVars.Instance.CurrentSlot;
		_profile = new PlayerProfile();
		_attributes = new PlayerAttributes();

		if (_currentSlot < 0)
		{
			GD.Print("No slot selected, using in-memory defaults.");
			return;
		}

		var saveData = SaveManager.Instance.LoadGame(_currentSlot);
		if (saveData.Count == 0)
		{
			GD.Print("No save data found, using in-memory defaults.");
			return;
		}

		if (saveData.ContainsKey("player_name"))
			_profile.PlayerName = saveData["player_name"].AsString();
		if (saveData.ContainsKey("current_day"))
			_profile.CurrentDay = saveData["current_day"].AsInt32();
		if (saveData.ContainsKey("career_readiness"))
			_profile.ChangeCareerReadiness(saveData["career_readiness"].AsInt32());

		if (saveData.ContainsKey("focus"))
			_attributes.ChangeFocus(saveData["focus"].AsInt32() - _attributes.Focus);
		if (saveData.ContainsKey("energy"))
			_attributes.ChangeEnergy(saveData["energy"].AsInt32() - _attributes.Energy);
		if (saveData.ContainsKey("knowledge"))
			_attributes.ChangeKnowledge(saveData["knowledge"].AsInt32() - _attributes.Knowledge);
		if (saveData.ContainsKey("confidence"))
			_attributes.ChangeConfidence(saveData["confidence"].AsInt32() - _attributes.Confidence);
	}

	private void WireButtons()
	{
		GetNode<Button>("ProfilePanel/Actions/StudyButton").Pressed += () => OnActionPressed("Study CS", () => _actions.StudyCS());
		GetNode<Button>("ProfilePanel/Actions/CodingButton").Pressed += () => OnActionPressed("Coding Practice", () => _actions.CodingPractice());
		GetNode<Button>("ProfilePanel/Actions/SleepButton").Pressed += () => OnActionPressed("Sleep", () => _actions.Sleep());
		GetNode<Button>("ProfilePanel/Actions/ProcrastinateButton").Pressed += () => OnActionPressed("Procrastinate", () => _actions.Procrastinate());
		GetNode<Button>("ProfilePanel/Actions/InternshipButton").Pressed += () => OnActionPressed("Apply For Internship", () => _actions.ApplyForInternship());
	}

	private void OnActionPressed(string actionName, System.Action action)
	{
		action.Invoke();
		RefreshUI();
		PrintDebugStatus(actionName);
		TrySaveCurrentState();
	}

	private void RefreshUI()
	{
		_playerNameLabel.Text = $"Name: {_profile.PlayerName}";
		_dayLabel.Text = $"Day: {_profile.CurrentDay}";
		_goalLabel.Text = $"Goal: {_profile.Goal}";
		_readinessLabel.Text = $"Career Readiness: {_profile.CareerReadiness}/100";
		_focusLabel.Text = $"Focus: {_attributes.Focus}/100";
		_energyLabel.Text = $"Energy: {_attributes.Energy}/100";
		_knowledgeLabel.Text = $"Knowledge: {_attributes.Knowledge}/100";
		_confidenceLabel.Text = $"Confidence: {_attributes.Confidence}/100";
	}

	private void PrintDebugStatus(string actionName)
	{
		GD.Print($"Action: {actionName}");
		GD.Print($"Day={_profile.CurrentDay}, Readiness={_profile.CareerReadiness}, Focus={_attributes.Focus}, Energy={_attributes.Energy}, Knowledge={_attributes.Knowledge}, Confidence={_attributes.Confidence}");
	}

	private void TrySaveCurrentState()
	{
		if (_currentSlot < 0)
			return;

		var saveData = SaveManager.Instance.LoadGame(_currentSlot);
		if (saveData.Count == 0)
			return;

		saveData["current_day"] = _profile.CurrentDay;
		saveData["career_readiness"] = _profile.CareerReadiness;
		saveData["focus"] = _attributes.Focus;
		saveData["energy"] = _attributes.Energy;
		saveData["knowledge"] = _attributes.Knowledge;
		saveData["confidence"] = _attributes.Confidence;
		SaveManager.Instance.SaveGame(_currentSlot, saveData);
	}
}
