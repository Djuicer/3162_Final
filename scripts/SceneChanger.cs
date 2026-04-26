using Godot;

[GlobalClass]
public partial class SceneChanger : Node
{
	[Export] private PackedScene targetScene;   // 直接在编辑器中拖入场景文件
	[Export] private bool quitGame = false;

	private Button button;

	public override void _Ready()
	{
		// 尝试获取父节点（或自身）的 Button
		button = Owner as Button ?? GetParent<Button>();
		if (button == null)
			button = GetNodeOrNull<Button>("..");

		if (button != null)
			button.Pressed += OnButtonPressed;
		else
			GD.PrintErr("SceneChanger: 未找到 Button，请将此脚本作为 Button 的子节点或挂载到 Button 上。");
	}

	private void OnButtonPressed()
	{
		if (quitGame)
		{
			GetTree().Quit();
			return;
		}

		if (targetScene != null)
		{
			GetTree().ChangeSceneToPacked(targetScene);
		}
		else
		{
			GD.PrintErr("SceneChanger: 未设置目标场景（PackedScene 为空）");
		}
	}
}
