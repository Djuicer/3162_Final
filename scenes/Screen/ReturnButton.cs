using Godot;
using System;

public partial class ReturnButton : TextureButton
{
	[Export]
	public Label TooltipLabel { get; set; }
	
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
		TransitionManager.Instance?.ChangeSceneToFileWithFade("res://scenes/Domitory/dormitory.tscn");
	}
}
