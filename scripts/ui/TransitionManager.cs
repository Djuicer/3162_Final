using Godot;
using System;

public partial class TransitionManager : CanvasLayer
{
	public static TransitionManager Instance { get; private set; }
	[Export] public float FadeDuration = 0.4f;
	private ColorRect _overlay;
	private bool _isTransitioning;

	public override void _Ready()
	{
		Instance = this;
		Layer = 128;
		ProcessMode = ProcessModeEnum.Always;

		_overlay = new ColorRect
		{
			Name = "FadeOverlay",
			Color = new Color(0, 0, 0, 0),
			AnchorLeft = 0,
			AnchorTop = 0,
			AnchorRight = 1,
			AnchorBottom = 1,
			OffsetLeft = 0,
			OffsetTop = 0,
			OffsetRight = 0,
			OffsetBottom = 0,
			MouseFilter = Control.MouseFilterEnum.Ignore,
			Visible = true
		};
		AddChild(_overlay);
	}

	public async void ChangeSceneToFileWithFade(string scenePath, Action beforeChange = null)
	{
		if (_isTransitioning || string.IsNullOrEmpty(scenePath))
			return;
		_isTransitioning = true;
		await FadeTo(1f);
		beforeChange?.Invoke();
		GetTree().ChangeSceneToFile(scenePath);
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		await FadeTo(0f);
		_isTransitioning = false;
	}

	public async void ChangeSceneToPackedWithFade(PackedScene scene, Action beforeChange = null)
	{
		if (_isTransitioning || scene == null)
			return;
		_isTransitioning = true;
		await FadeTo(1f);
		beforeChange?.Invoke();
		GetTree().ChangeSceneToPacked(scene);
		await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
		await FadeTo(0f);
		_isTransitioning = false;
	}

	private async System.Threading.Tasks.Task FadeTo(float alpha)
	{
		if (_overlay == null)
			return;
		_overlay.MouseFilter = Control.MouseFilterEnum.Stop;
		Tween tween = CreateTween();
		tween.SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(_overlay, "color:a", alpha, FadeDuration);
		await ToSignal(tween, Tween.SignalName.Finished);
		if (alpha <= 0.01f)
			_overlay.MouseFilter = Control.MouseFilterEnum.Ignore;
	}
}
