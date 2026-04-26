using Godot;

public partial class MainMenu : Control
{
	private void OnStartPressed()
	{
		var global = GetNode<GlobalVars>("/root/GVars");
		global.SaveSelectMode = "start";
		GetTree().ChangeSceneToFile("res://scenes/Control/SaveSelect.tscn");
	}

	private void OnLoadPressed()
	{
		var global = GetNode<GlobalVars>("/root/GVars");
		global.SaveSelectMode = "load";
		GetTree().ChangeSceneToFile("res://scenes/Control/SaveSelect.tscn");
	}

	private void OnSettingsPressed()
	{
		GD.Print("设置界面待实现");
	}

	private void OnExitPressed()
	{
		GetTree().Quit();
	}
}
