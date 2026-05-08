using Godot;

public partial class FloatingStatPopup : Control
{
	private TextureRect _iconTextureRect;
	private Label _valueLabel;

	public override void _Ready()
	{
		_iconTextureRect = GetNode<TextureRect>("IconTextureRect");
		_valueLabel = GetNode<Label>("ValueLabel");
		MouseFilter = MouseFilterEnum.Ignore;
	}

	public void Configure(string statName, int delta, bool showIcon)
	{
		if (_valueLabel == null)
			return;

		_valueLabel.Text = $"{(delta > 0 ? "+" : "")}{delta} {statName}";
		_valueLabel.Modulate = delta > 0 ? new Color(0.6f, 1f, 0.6f, 1f) : new Color(1f, 0.6f, 0.6f, 1f);
		if (_iconTextureRect != null)
			_iconTextureRect.Visible = showIcon;
	}
}
