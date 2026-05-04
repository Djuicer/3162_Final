using Godot;
using System.Threading.Tasks;

public partial class ComputerButton : TextureButton
{
	[Export] public Label TooltipLabel { get; set; }
	[Export] public PackedScene TargetScene { get; set; }
	[Export] public string HoverText { get; set; } = "Check Computer";
	[Export] public bool ShowInteractionLine { get; set; } = true;
	[Export] public Label InteractionLabel { get; set; }
	[Export] public CanvasItem InteractionPanel { get; set; }
	[Export] public string InteractionText { get; set; } = "You sit down at your desk.";
	[Export] public float TransitionDelaySeconds { get; set; } = 0.8f;

	private Vector2 _originalScale;
	private bool _isTransitioning;

	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		Pressed += OnPressed;
		_originalScale = Scale;

		if (TooltipLabel != null)
		{
			TooltipLabel.Text = HoverText;
			TooltipLabel.Visible = false;
		}
	}

	private void OnMouseEntered()
	{
		if (TooltipLabel != null)
			TooltipLabel.Visible = true;

		Scale = _originalScale * 1.05f;
		Modulate = new Color(1.08f, 1.08f, 1.08f, 1f);
	}

	private void OnMouseExited()
	{
		if (TooltipLabel != null)
			TooltipLabel.Visible = false;

		Scale = _originalScale;
		Modulate = Colors.White;
	}

	private async void OnPressed()
	{
		if (_isTransitioning)
			return;

		_isTransitioning = true;
		Disabled = true;
		if (TooltipLabel != null)
			TooltipLabel.Visible = false;

		if (ShowInteractionLine && InteractionLabel != null)
		{
			if (InteractionPanel != null)
				InteractionPanel.Visible = true;

			InteractionLabel.Text = InteractionText;
			await ToSignal(GetTree().CreateTimer(Mathf.Max(0.1f, TransitionDelaySeconds)), SceneTreeTimer.SignalName.Timeout);
		}

		if (TargetScene != null)
		{
			GetTree().ChangeSceneToPacked(TargetScene);
		}
		else
		{
			GD.PrintErr("NO Target! Put a Scence onto the attribute of TargetScene.");
			Disabled = false;
			_isTransitioning = false;
		}
	}
}
