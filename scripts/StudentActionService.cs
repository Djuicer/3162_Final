using Godot;

public partial class StudentActionService : RefCounted
{
	private readonly PlayerProfile _profile;
	private readonly PlayerAttributes _attributes;

	public StudentActionService(PlayerProfile profile, PlayerAttributes attributes)
	{
		_profile = profile;
		_attributes = attributes;
	}

	public void StudyCS()
	{
		_attributes.ChangeKnowledge(10);
		_attributes.ChangeEnergy(-15);
		_attributes.ChangeFocus(-5);
	}

	public void CodingPractice()
	{
		_attributes.ChangeKnowledge(8);
		_attributes.ChangeConfidence(5);
		_attributes.ChangeEnergy(-10);
		_profile.ChangeCareerReadiness(5);
	}

	public void Sleep()
	{
		_attributes.ChangeEnergy(30);
		_attributes.ChangeFocus(10);
		_profile.CurrentDay += 1;
	}

	public void Procrastinate()
	{
		_attributes.ChangeEnergy(10);
		_attributes.ChangeConfidence(-5);
		_attributes.ChangeFocus(-5);
	}

	public void ApplyForInternship()
	{
		_attributes.ChangeEnergy(-10);
		_attributes.ChangeConfidence(-5);
		_profile.ChangeCareerReadiness(10);

		if (_attributes.Confidence < 20)
		{
			GD.Print("Player feels too nervous to confidently apply right now.");
		}
	}
}
