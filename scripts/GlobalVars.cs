using Godot;

[GlobalClass]
public partial class GlobalVars : Node
{
	public static GlobalVars Instance { get; private set; }

	public string SaveSelectMode { get; set; } = "start";
	public int CurrentSlot { get; set; } = -1;

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
