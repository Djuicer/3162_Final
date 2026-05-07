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
	public string ReturnSpawnId { get; set; } = "";
	public string PendingSpawnId { get; set; } = "";

	public bool HasSeenDormitoryTutorial { get; set; }
	public bool HasReadDancePartyEmail { get; set; }
	public bool DancePartyUnlocked { get; set; }
	public bool PartyAttended { get; set; }
	public bool PartyNetworkingUnlocked { get; set; }
	public int PartyGoodChoices { get; set; }
	public bool PartyEventCompleted { get; set; }
	public bool Day5NpcMet { get; set; }
	public bool ITBallInvited { get; set; }
	public bool ITBallAttended { get; set; }
	public int ITBallGoodChoices { get; set; }
	public bool JoinedDeveloperGroup { get; set; }


	public string GetCurrentHintMessage()
	{
		if (JoinedDeveloperGroup)
			return "Keep building your skills and portfolio before graduation.";
		if (Profile.CurrentDay >= 3 && !DancePartyUnlocked)
			return "Check your email. There may be a social opportunity.";
		if (Profile.CurrentDay >= 3 && DancePartyUnlocked && !PartyAttended)
			return "You have a party invitation in your email. It may help your networking.";
		if (PartyNetworkingUnlocked)
			return "Your party connection may help your job prospects.";
		if (ITBallInvited)
			return "You have an IT Ball invitation in your email.";
		if (Profile.CurrentDay >= 5 && !Day5NpcMet)
			return "Someone in the Innovation Hub might have an opportunity for you.";

		if (Profile.ActionsLeft <= 0)
			return "You have no actions left. Go to bed and sleep.";

		if (Attributes.Energy <= 20)
			return "Your Energy is low. Consider sleeping soon.";

		if (Profile.CareerReadiness < 40 && Profile.CurrentDay >= 5)
			return "Graduation is close. Focus on Career Readiness.";

		if (Attributes.Knowledge < 40)
			return "Your Knowledge is low. Study CS is now done in the Computer Lab.";

		return "Build your skills before graduation.";
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
				Day5NpcMet = false;
				ITBallInvited = false;
				ITBallAttended = false;
				ITBallGoodChoices = 0;
				JoinedDeveloperGroup = false;
		ReturnScenePath = DefaultDormitoryScenePath;
		ReturnSpawnId = "";
		PendingSpawnId = "";
	}

	public void SetReturnContext(string scenePath, string spawnId)
	{
		ReturnScenePath = scenePath;
		ReturnSpawnId = spawnId;
	}

	public void QueuePendingSpawn(string spawnId)
	{
		PendingSpawnId = spawnId;
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

	public void NotifyStatChange(string statName, int delta)
	{
		if (delta == 0)
			return;

		StatusOverlay.NotifyStatChange(statName, delta);
	}
}
