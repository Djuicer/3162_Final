using Godot;

public partial class PlayerProfile : RefCounted
{
    private int _currentDay = 1;
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

    public int CareerReadiness
    {
        get => _careerReadiness;
        set => _careerReadiness = Mathf.Clamp(value, 0, 100);
    }

    public void IncreaseCareerReadiness(int amount) => CareerReadiness += amount;
    public void DecreaseCareerReadiness(int amount) => CareerReadiness -= amount;
}
