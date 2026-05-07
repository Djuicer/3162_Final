using Godot;
using System.Collections.Generic;

public partial class EmailScreen : Control
{
	private record MailItem(string Id, string Subject, string Body);

	private ItemList _emailList;
	private Label _subjectLabel;
	private Label _bodyLabel;
	private Label _statusLabel;
	private Button _attendPartyButton;
	private Button _viewEventDetailsButton;
	private readonly List<MailItem> _mails = new();

	public override void _Ready()
	{
		_emailList = GetNode<ItemList>("Panel/Margin/VBox/Content/HBox/EmailList");
		_subjectLabel = GetNode<Label>("Panel/Margin/VBox/Content/HBox/Detail/Subject");
		_bodyLabel = GetNode<Label>("Panel/Margin/VBox/Content/HBox/Detail/Body");
		_statusLabel = GetNode<Label>("Panel/Margin/VBox/Status");
		_attendPartyButton = GetNode<Button>("Panel/Margin/VBox/Actions/AttendPartyButton");
		_viewEventDetailsButton = GetNode<Button>("Panel/Margin/VBox/Actions/ViewEventDetailsButton");
		var backButton = GetNode<Button>("Panel/Margin/VBox/Actions/BackButton");

		_emailList.ItemSelected += OnEmailSelected;
		_attendPartyButton.Pressed += OnAttendPartyPressed;
		_viewEventDetailsButton.Pressed += OnViewEventDetailsPressed;
		backButton.Pressed += () => GetTree().ChangeSceneToFile("res://scenes/Screen/screen.tscn");

		BuildInbox();
	}

	private void BuildInbox()
	{
		_mails.Clear();
		_emailList.Clear();
		_attendPartyButton.Visible = false;
		_viewEventDetailsButton.Visible = false;

		_mails.Add(new MailItem(
			"welcome",
			"Welcome to Final Semester",
			"Final semester has started. You have 7 days to improve your skills, confidence, and career readiness before graduation."));

		if (GlobalVars.Instance.Profile.CurrentDay >= 3)
		{
			_mails.Add(new MailItem(
				"dance_party",
				"CS Society Dance Party Tonight",
				"The CS Society is hosting a dance party tonight. It could be a chance to relax, meet people, and build your network."));
		}
		if (GlobalVars.Instance.ITBallInvited)
		{
			_mails.Add(new MailItem(
				"it_ball",
				"IT Ball Invitation",
				"You've been invited to the IT Ball. It could be a chance to meet developers working on real projects."));
		}

		foreach (var mail in _mails)
			_emailList.AddItem(mail.Subject);

		if (_mails.Count > 0)
		{
			_emailList.Select(0);
			ShowMail(0);
		}
	}

	private void OnEmailSelected(long index)
	{
		ShowMail((int)index);
	}

	private void ShowMail(int index)
	{
		if (index < 0 || index >= _mails.Count)
			return;

		var mail = _mails[index];
		_subjectLabel.Text = mail.Subject;
		_bodyLabel.Text = mail.Body;
		_statusLabel.Text = "";

		bool isDanceParty = mail.Id == "dance_party";
		bool isItBall = mail.Id == "it_ball";
		_attendPartyButton.Visible = isDanceParty && !GlobalVars.Instance.PartyAttended;
		_viewEventDetailsButton.Visible = isItBall;
		if (isDanceParty)
		{
			GlobalVars.Instance.HasReadDancePartyEmail = true;
			GlobalVars.Instance.DancePartyUnlocked = true;
			if (GlobalVars.Instance.PartyAttended)
			{
				_statusLabel.Text = GlobalVars.Instance.PartyNetworkingUnlocked
					? "You already attended this event. You made a useful connection at the party."
					: "You already attended this event.";
			}
		}
		if (isItBall)
			_statusLabel.Text = "Open details to learn more about this opportunity.";
	}

	private void OnAttendPartyPressed()
	{
		var state = GlobalVars.Instance;
		if (state.Profile.ActionsLeft <= 0)
		{
			_statusLabel.Text = "You have no actions left today. Sleep and try again tomorrow.";
			return;
		}

		if (!state.Profile.TrySpendAction(1))
		{
			_statusLabel.Text = "You have no actions left today. Sleep and try again tomorrow.";
			return;
		}

		state.Attributes.DecreaseEnergy(10);
		state.PartyAttended = true;
		state.PartyGoodChoices = 0;
		_attendPartyButton.Visible = false;
		state.ReturnScenePath = "res://scenes/Computer/Email/email.tscn";
		GetTree().ChangeSceneToFile("res://scenes/Events/DanceParty/dance_party.tscn");
	}

	private void OnViewEventDetailsPressed()
	{
		_statusLabel.Text = "The IT Ball event will be added later.";
	}
}
