using Godot;

public partial class PlayerProfile : RefCounted
{
	public string PlayerName { get; set; } = "Player";
	public string YearLevel { get; set; } = "3rd Year";
	public string Semester { get; set; } = "Final Semester";
	public int CurrentDay { get; set; } = 1;
	public string Goal { get; set; } = "Make a comeback before graduation";
	public int CareerReadiness { get; private set; } = 0;

	public void ChangeCareerReadiness(int delta)
	{
		CareerReadiness = Mathf.Clamp(CareerReadiness + delta, 0, 100);
	}
}
