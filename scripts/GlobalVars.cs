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

	public bool HasSeenDormitoryTutorial { get; set; }
	public string LastSceneBeforeMinigame { get; set; } = DefaultDormitoryScenePath;


	public string GetCurrentHintMessage()
	{
		if (Profile.ActionsLeft <= 0)
			return "You have no actions left. Go to bed and sleep.";

		if (Attributes.Energy <= 20)
			return "Your Energy is low. Consider sleeping soon.";

		if (Profile.CareerReadiness < 40 && Profile.CurrentDay >= 5)
			return "Graduation is close. Focus on Career Readiness.";

		if (Attributes.Knowledge < 40)
			return "Your Knowledge is low. Study CS or practice coding.";

		return "Choose an activity to improve your comeback.";
	}

	public string GetDormitoryHintMessage()
	{
		if (Profile.ActionsLeft <= 0)
			return "No actions left: sleep at bed.";
		if (Attributes.Energy <= 20)
			return "Low Energy: sleep soon.";
		return "Go to the Computer Lab to Study CS. Use dorm computer for profile and other actions.";
	}

	public void ResetGame()
	{
		Profile.ResetToDefaults();
		Attributes.ResetToDefaults();
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
