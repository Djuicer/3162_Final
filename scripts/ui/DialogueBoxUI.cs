using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class DialogueBoxUI : CanvasLayer
{
	[Export] public float CharactersPerSecond = 55.0f;
	[Export] public float PopupDurationSeconds = 0.25f;

	private Control _root;
	private Control _dialoguePanel;
	private Label _speakerNameLabel;
	private RichTextLabel _dialogueTextLabel;
	private Label _continueHintLabel;
	private Control _choiceContainer;
	private VBoxContainer _choiceList;
	private bool _skipRequested;
	private bool _isTyping;
	private bool _awaitingContinue;
	private TaskCompletionSource<bool> _continueTcs;

	public override void _Ready()
	{
		_root = GetNode<Control>("Root");
		_dialoguePanel = GetNode<Control>("Root/DialoguePanel");
		_speakerNameLabel = GetNode<Label>("Root/DialoguePanel/MarginContainer/Content/SpeakerNameLabel");
		_dialogueTextLabel = GetNode<RichTextLabel>("Root/DialoguePanel/MarginContainer/Content/DialogueTextLabel");
		_continueHintLabel = GetNode<Label>("Root/DialoguePanel/MarginContainer/Content/ContinueHintLabel");
		_choiceContainer = GetNode<Control>("Root/ChoiceContainer");
		_choiceList = GetNode<VBoxContainer>("Root/ChoiceContainer/MarginContainer/ChoiceList");
		_dialoguePanel.GuiInput += OnDialoguePanelGuiInput;

		_root.Visible = false;
		_choiceContainer.Visible = false;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (!_root.Visible)
			return;
		if (@event.IsActionPressed("interact") || @event.IsActionPressed("ui_accept"))
		{
			TryAdvance();
			GetViewport().SetInputAsHandled();
		}
	}

	public async Task ShowLineAsync(string speaker, string line)
	{
		_root.Visible = true;
		_choiceContainer.Visible = false;
		_speakerNameLabel.Text = speaker;
		_continueHintLabel.Visible = false;
		await AnimatePopupAsync();
		await TypeLineAsync(line);
		await WaitForContinueAsync();
	}

	public async Task<int> ShowChoicesAsync(IReadOnlyList<string> choices)
	{
		ClearChoices();
		_choiceContainer.Visible = true;
		var tcs = new TaskCompletionSource<int>();
		for (int i = 0; i < choices.Count; i++)
		{
			int index = i;
			var button = new Button { Text = choices[i], AutowrapMode = TextServer.AutowrapMode.WordSmart, SizeFlagsHorizontal = Control.SizeFlags.ExpandFill, CustomMinimumSize = new Vector2(0, 42) };
			button.Pressed += () => tcs.TrySetResult(index);
			_choiceList.AddChild(button);
		}

		int selected = await tcs.Task;
		_choiceContainer.Visible = false;
		ClearChoices();
		return selected;
	}

	public void HideAll()
	{
		_root.Visible = false;
		_choiceContainer.Visible = false;
		_dialogueTextLabel.Text = string.Empty;
		_speakerNameLabel.Text = string.Empty;
		_continueHintLabel.Visible = false;
		ClearChoices();
	}

	private async Task AnimatePopupAsync()
	{
		_dialoguePanel.Modulate = new Color(1, 1, 1, 0);
		Vector2 target = _dialoguePanel.Position;
		_dialoguePanel.Position = target + new Vector2(0, 18);
		var tween = CreateTween();
		tween.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
		tween.TweenProperty(_dialoguePanel, "position", target, PopupDurationSeconds);
		tween.Parallel().TweenProperty(_dialoguePanel, "modulate", Colors.White, PopupDurationSeconds);
		await ToSignal(tween, Tween.SignalName.Finished);
	}

	private async Task TypeLineAsync(string text)
	{
		_isTyping = true;
		_skipRequested = false;
		_dialogueTextLabel.Text = text;
		_dialogueTextLabel.VisibleCharacters = 0;
		double interval = 1.0 / Math.Max(1.0f, CharactersPerSecond);
		while (_dialogueTextLabel.VisibleCharacters < text.Length)
		{
			if (_skipRequested)
			{
				_dialogueTextLabel.VisibleCharacters = text.Length;
				break;
			}
			_dialogueTextLabel.VisibleCharacters += 1;
			await ToSignal(GetTree().CreateTimer(interval), SceneTreeTimer.SignalName.Timeout);
		}
		_isTyping = false;
		_skipRequested = false;
	}

	private void OnDialoguePanelGuiInput(InputEvent @event)
	{
		if (!_root.Visible || _choiceContainer.Visible)
			return;
		if (@event is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
		{
			TryAdvance();
			GetViewport().SetInputAsHandled();
		}
	}

	private async Task WaitForContinueAsync()
	{
		_continueHintLabel.Text = "Click dialogue / Press E, Enter, or Space";
		_continueHintLabel.Visible = true;
		_awaitingContinue = true;
		_continueTcs = new TaskCompletionSource<bool>();
		await _continueTcs.Task;
		_awaitingContinue = false;
		_continueHintLabel.Visible = false;
	}

	private void TryAdvance()
	{
		if (_choiceContainer.Visible)
			return;
		if (_isTyping)
		{
			_skipRequested = true;
			return;
		}
		if (_awaitingContinue)
			_continueTcs?.TrySetResult(true);
	}

	private void ClearChoices()
	{
		foreach (Node child in _choiceList.GetChildren())
			child.QueueFree();
	}
}
