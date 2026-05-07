using Godot;

public partial class PlayerController : CharacterBody2D
{
	[Export] public float MoveSpeed = 140.0f;

	private AnimatedSprite2D animatedSprite;
	private Vector2 lastDirection = Vector2.Down;

	public override void _Ready()
	{
		animatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = inputDirection * MoveSpeed;
		MoveAndSlide();

		if (animatedSprite == null)
			return;

		if (inputDirection != Vector2.Zero)
		{
			lastDirection = inputDirection;
			animatedSprite.Play(GetAnimationName("walk", inputDirection));
		}
		else
		{
			animatedSprite.Play(GetAnimationName("idle", lastDirection));
		}
	}

	private string GetAnimationName(string prefix, Vector2 direction)
	{
		if (Mathf.Abs(direction.X) > Mathf.Abs(direction.Y))
		{
			return direction.X < 0 ? $"{prefix}_left" : $"{prefix}_right";
		}

		return direction.Y < 0 ? $"{prefix}_up" : $"{prefix}_down";
	}
}
