using Godot;
using System;

public partial class HoverButton : TextureButton
{
	[Export]
	public Label TooltipLabel { get; set; }
	
	[Export]
	public PackedScene TargetScene { get; set; }
	
	public override void _Ready()
	{
		this.MouseEntered += OnMouseEntered;
		this.MouseExited += OnMouseExited;
		this.Pressed += OnPressed;
		
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
	
	private void OnPressed()
	{
		if (TargetScene != null)
		{
			GetTree().ChangeSceneToPacked(TargetScene);
		}
		else
		{
			GD.PrintErr("NO Target! Put a Scence onto the attribute of TargetScene.");
		}
	}
}
