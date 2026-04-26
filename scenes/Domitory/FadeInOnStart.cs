using Godot;

public partial class FadeInOnStart : Node2D  // 如果你的根节点是 Node2D，用这个。如果是 Control，用 Control
{
	[Export] private float fadeDuration = 1.0f;
	private ColorRect fadeOverlay;
	private Tween tween;

	public override void _Ready()
	{
		// 找到场景中的 FadeOverlay
		fadeOverlay = GetNode<ColorRect>("FadeOverlay");
		if (fadeOverlay == null)
		{
			GD.PrintErr("未找到 FadeOverlay 节点，请在根节点下添加一个 ColorRect 并命名为 FadeOverlay");
			return;
		}

		// 确保初始为完全不透明（黑色全屏）
		fadeOverlay.Modulate = new Color(1, 1, 1, 1);

		// 开始淡入（降低 alpha）
		tween = CreateTween();
		tween.TweenProperty(fadeOverlay, "modulate:a", 0.0f, fadeDuration);
		tween.TweenCallback(Callable.From(() => {
			// 可选：淡入完成后隐藏或删除遮罩
			fadeOverlay.Visible = false;
		}));
	}
}
