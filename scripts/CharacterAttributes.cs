using Godot;

public class StatValue
{
	public string Name { get; }
	public int Min { get; }
	public int Max { get; }
	public int Current { get; private set; }

	public StatValue(string name, int current, int min = 0, int max = 100)
	{
		Name = name;
		Min = min;
		Max = max;
		Current = Mathf.Clamp(current, min, max);
	}

	public void Increase(int amount)
	{
		Current = Mathf.Clamp(Current + amount, Min, Max);
	}

	public void Decrease(int amount)
	{
		Current = Mathf.Clamp(Current - amount, Min, Max);
	}

	public override string ToString()
	{
		return $"{Name}: {Current}/{Max}";
	}
}

public partial class CharacterAttributes : RefCounted
{
	public StatValue Focus { get; } = new("Focus", 50);
	public StatValue Energy { get; } = new("Energy", 70);
	public StatValue Knowledge { get; } = new("Knowledge", 20);
	public StatValue Confidence { get; } = new("Confidence", 30);

	public void StudyCs()
	{
		Knowledge.Increase(10);
		Energy.Decrease(15);
		Focus.Decrease(5);
		PrintStats("Study CS");
	}

	public void CodingPractice()
	{
		Knowledge.Increase(8);
		Confidence.Increase(5);
		Energy.Decrease(10);
		PrintStats("Coding Practice");
	}

	public void Sleep()
	{
		Energy.Increase(30);
		Focus.Increase(10);
		PrintStats("Sleep");
	}

	public void Procrastinate()
	{
		Energy.Increase(10);
		Confidence.Decrease(5);
		PrintStats("Procrastinate");
	}

	public void PrintStats(string actionName)
	{
		GD.Print($"[Action] {actionName}");
		GD.Print($"  {Focus}");
		GD.Print($"  {Energy}");
		GD.Print($"  {Knowledge}");
		GD.Print($"  {Confidence}");
	}
}
