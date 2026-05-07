using Godot;

public partial class DesktopIconButton : TextureButton
{
	[Export]
	public Label TooltipLabel { get; set; }

	public override void _Ready()
	{
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;

		if (TooltipLabel != null)
		{
			TooltipLabel.Visible = false;
		}
	}

	private void OnMouseEntered()
	{
		if (TooltipLabel != null)
		{
			TooltipLabel.Visible = true;
		}
	}

	private void OnMouseExited()
	{
		if (TooltipLabel != null)
		{
			TooltipLabel.Visible = false;
		}
	}
}
