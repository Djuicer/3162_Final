using Godot;
using System.Threading.Tasks;

public partial class MainMenu : Control
{
	private const float PressDelaySeconds = 0.18f;
	private Node _audioController;

	public override void _Ready()
	{
		_audioController = GetNodeOrNull<Node>("/root/AudioControllerScene");
		StartMenuMusic();

		WireButtonAudio("StartButton");
		WireButtonAudio("ContinueButton");
		WireButtonAudio("OptionButton");
		WireButtonAudio("ExitButton");
	}

	private void WireButtonAudio(string buttonPath)
	{
		var button = GetNodeOrNull<Button>(buttonPath);
		if (button == null || _audioController == null)
		{
			return;
		}

		button.MouseEntered += () => _audioController.Call("play_hover_button");
		button.Pressed += () => _audioController.Call("play_press_ui_button");
	}

	private async void OnStartPressed()
	{
		var global = GetNode<GlobalVars>("/root/GVars");
		global.SaveSelectMode = "start";
		StopMenuMusic();
		await PlayPressAndDelay();
		GetTree().ChangeSceneToFile("res://scenes/Control/SaveSelect.tscn");
	}

	private async void OnLoadPressed()
	{
		var global = GetNode<GlobalVars>("/root/GVars");
		global.SaveSelectMode = "load";
		StopMenuMusic();
		await PlayPressAndDelay();
		GetTree().ChangeSceneToFile("res://scenes/Control/SaveSelect.tscn");
	}

	private void OnSettingsPressed()
	{
		GD.Print("设置界面待实现");
	}

	private async void OnExitPressed()
	{
		await PlayPressAndDelay();
		GetTree().Quit();
	}

	private async Task PlayPressAndDelay()
	{
		if (_audioController != null)
		{
			_audioController.Call("play_press_ui_button");
		}

		await ToSignal(GetTree().CreateTimer(PressDelaySeconds), "timeout");
	}

	private void StopMenuMusic()
	{
		if (_audioController != null)
		{
			_audioController.Call("stop_music");
		}
	}

	private void StartMenuMusic()
	{
		if (_audioController != null)
		{
			_audioController.Call("play_music");
		}
	}
}
