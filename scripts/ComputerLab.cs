using Godot;

public partial class ComputerLab : Node2D
{
    private const string DormitoryScenePath = "res://scenes/Domitory/dormitory.tscn";
    private const string FocusCatchScenePath = "res://scenes/Minigames/FocusCatch/focus_catch.tscn";

    private Area2D _pcInteractArea;
    private Area2D _exitInteractArea;
    private Label _interactPromptLabel;
    private Label _hintLabel;

    private bool _canUsePc;
    private bool _canExitLab;
    private string _hintText = "Interact with a PC to Study CS. Return to your dorm when finished.";

    public override void _Ready()
    {
        _pcInteractArea = GetNodeOrNull<Area2D>("PcInteractArea");
        _exitInteractArea = GetNodeOrNull<Area2D>("ExitInteractArea");
        _interactPromptLabel = GetNodeOrNull<Label>("UI/InteractPromptLabel");
        _hintLabel = GetNodeOrNull<Label>("UI/GuidancePanel/MarginContainer/VBoxContainer/HintLabel");

        if (_pcInteractArea != null)
        {
            _pcInteractArea.BodyEntered += OnPcAreaBodyEntered;
            _pcInteractArea.BodyExited += OnPcAreaBodyExited;
        }

        if (_exitInteractArea != null)
        {
            _exitInteractArea.BodyEntered += OnExitAreaBodyEntered;
            _exitInteractArea.BodyExited += OnExitAreaBodyExited;
        }

        if (_interactPromptLabel != null)
            _interactPromptLabel.Visible = false;

        RefreshHint();
    }

    public override void _Process(double delta)
    {
        RefreshHint();

        if (_interactPromptLabel == null)
            return;

        if (_canUsePc)
        {
            _interactPromptLabel.Text = "Press E to Study CS";
            _interactPromptLabel.Visible = true;
            return;
        }

        if (_canExitLab)
        {
            _interactPromptLabel.Text = "Press E to return to Dormitory";
            _interactPromptLabel.Visible = true;
            return;
        }

        _interactPromptLabel.Visible = false;
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!@event.IsActionPressed("interact"))
            return;

        if (_canUsePc)
        {
            StartStudyCs();
            return;
        }

        if (_canExitLab)
            GetTree().ChangeSceneToFile(DormitoryScenePath);
    }

    private void StartStudyCs()
    {
        var state = GlobalVars.Instance;

        if (state.Profile.ActionsLeft <= 0)
        {
            ShowMessage("You have no actions left. Go back to your dorm and sleep.");
            return;
        }

        if (state.Attributes.Energy <= 0)
        {
            ShowMessage("You are out of energy. Go back to your dorm and sleep.");
            return;
        }

        state.LastSceneBeforeMinigame = "res://scenes/ComputerLab/computer_lab.tscn";
        GetTree().ChangeSceneToFile(FocusCatchScenePath);
    }

    private void ShowMessage(string message)
    {
        if (_hintLabel != null)
            _hintText = message;
        _hintLabel.Text = _hintText;
    }

    private void RefreshHint()
    {
        if (_hintLabel == null)
            return;

        _hintLabel.Text = _hintText;
    }

    private void OnPcAreaBodyEntered(Node2D body)
    {
        if (body is CharacterBody2D)
            _canUsePc = true;
    }

    private void OnPcAreaBodyExited(Node2D body)
    {
        if (body is CharacterBody2D)
            _canUsePc = false;
    }

    private void OnExitAreaBodyEntered(Node2D body)
    {
        if (body is CharacterBody2D)
            _canExitLab = true;
    }

    private void OnExitAreaBodyExited(Node2D body)
    {
        if (body is CharacterBody2D)
            _canExitLab = false;
    }
}
