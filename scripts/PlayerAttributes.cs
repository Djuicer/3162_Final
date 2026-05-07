using Godot;

public partial class PlayerAttributes : RefCounted
{
	public int Focus { get; private set; } = 50;
	public int Energy { get; private set; } = 70;
	public int Knowledge { get; private set; } = 20;
	public int Confidence { get; private set; } = 30;

	public void ChangeFocus(int delta) => Focus = ClampStat(Focus + delta);
	public void ChangeEnergy(int delta) => Energy = ClampStat(Energy + delta);
	public void ChangeKnowledge(int delta) => Knowledge = ClampStat(Knowledge + delta);
	public void ChangeConfidence(int delta) => Confidence = ClampStat(Confidence + delta);

	private int ClampStat(int value) => Mathf.Clamp(value, 0, 100);
}
