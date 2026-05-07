using Godot;

[GlobalClass]
public partial class GlobalVars : Node
{
	private const string DefaultDormitoryScenePath = "res://scenes/Domitory/dormitory.tscn";
	public static GlobalVars Instance { get; private set; }

	public string SaveSelectMode { get; set; } = "start";
	public int CurrentSlot { get; set; } = -1;
	public PlayerProfile Profile { get; private set; } = new PlayerProfile();
	public PlayerAttributes Attributes { get; private set; } = new PlayerAttributes();

	public string IntroScenePath { get; set; } = DefaultDormitoryScenePath;
	public string ReturnScenePath { get; set; } = "res://scenes/Domitory/dormitory.tscn";

	public bool HasSeenDormitoryTutorial { get; set; }
	public bool HasReadDancePartyEmail { get; set; }
	public bool DancePartyUnlocked { get; set; }
	public bool PartyAttended { get; set; }
	public bool PartyNetworkingUnlocked { get; set; }
	public int PartyGoodChoices { get; set; }
	public bool PartyEventCompleted { get; set; }


	public string GetCurrentHintMessage()
	{
		if (Profile.CurrentDay >= 3 && !DancePartyUnlocked)
			return "You have a new email. Check your inbox.";
		if (Profile.CurrentDay >= 3 && DancePartyUnlocked && !PartyAttended)
			return "You have a party invitation in your email. It may help your networking.";
		if (PartyNetworkingUnlocked)
			return "Your party connection may help your job prospects.";

		if (Profile.ActionsLeft <= 0)
			return "You have no actions left. Go to bed and sleep.";

		if (Attributes.Energy <= 20)
			return "Your Energy is low. Consider sleeping soon.";

		if (Profile.CareerReadiness < 40 && Profile.CurrentDay >= 5)
			return "Graduation is close. Focus on Career Readiness.";

		if (Attributes.Knowledge < 40)
			return "Your Knowledge is low. Study CS is now done in the Computer Lab.";

		return "Check your email for opportunities and events.";
	}

	public string GetDormitoryHintMessage()
	{
		if (Profile.ActionsLeft <= 0)
			return "No actions left: sleep at bed.";
		if (Attributes.Energy <= 20)
			return "Low Energy: sleep soon.";
		return "Computer Lab: Study CS. Innovation Hub: Practice Coding. Career Centre: Practice Interviews.";
	}

	public void ResetGame()
	{
		Profile.ResetToDefaults();
		Attributes.ResetToDefaults();
		HasReadDancePartyEmail = false;
		DancePartyUnlocked = false;
			PartyAttended = false;
			PartyNetworkingUnlocked = false;
			PartyGoodChoices = 0;
			PartyEventCompleted = false;
	}

	public override void _EnterTree()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			QueueFree();
		}
	}
}
