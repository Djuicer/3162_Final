using Godot;

public partial class PlayerProfile : RefCounted
{
    private int _currentDay = 1;
    private int _finalDay = 7;
    private int _actionsLeft = 3;
    private int _maxActionsPerDay = 3;
    private int _careerReadiness = 0;

    public string PlayerName { get; set; } = "Player";
    public string YearLevel { get; set; } = "3rd Year";
    public string Semester { get; set; } = "Final Semester";
    public string Goal { get; set; } = "Make a comeback before graduation";

    public int CurrentDay
    {
        get => _currentDay;
        set => _currentDay = Mathf.Max(1, value);
    }

    public int FinalDay
    {
        get => _finalDay;
        set => _finalDay = Mathf.Max(1, value);
    }

    public int ActionsLeft
    {
        get => _actionsLeft;
        set => _actionsLeft = Mathf.Clamp(value, 0, MaxActionsPerDay);
    }

    public int MaxActionsPerDay
    {
        get => _maxActionsPerDay;
        set => _maxActionsPerDay = Mathf.Max(1, value);
    }

    public int CareerReadiness
    {
        get => _careerReadiness;
        set => _careerReadiness = Mathf.Clamp(value, 0, 100);
    }

    public bool TrySpendAction(int cost)
    {
        if (cost <= 0 || ActionsLeft < cost)
            return false;

        ActionsLeft -= cost;
        return true;
    }

    public void ResetActionsForNewDay()
    {
        ActionsLeft = MaxActionsPerDay;
    }

    public void IncreaseCareerReadiness(int amount) => CareerReadiness += amount;
    public void DecreaseCareerReadiness(int amount) => CareerReadiness -= amount;

    public void ResetToDefaults()
    {
        CurrentDay = 1;
        FinalDay = 7;
        MaxActionsPerDay = 3;
        ActionsLeft = 3;
        CareerReadiness = 0;

        PlayerName = "Player";
        YearLevel = "3rd Year";
        Semester = "Final Semester";
        Goal = "Make a comeback before graduation";
    }
}
