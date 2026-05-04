using Godot;

public partial class Player : CharacterBody2D
{
	[Export] public float Speed = 120.0f;
	
	private AnimatedSprite2D sprite;

	public override void _Ready()
	{
		sprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
		Velocity = inputDirection * Speed;
		MoveAndSlide();

		UpdateAnimation(inputDirection);
	}

	private void UpdateAnimation(Vector2 inputDirection)
	{
		if (sprite == null)
		{
			return;
		}

		if (inputDirection == Vector2.Zero)
		{
			sprite.Stop();
			return;
		}

		if (Mathf.Abs(inputDirection.X) > Mathf.Abs(inputDirection.Y))
		{
			sprite.Animation = "walk_right";
			sprite.FlipH = inputDirection.X < 0;
		}
		else if (inputDirection.Y < 0)
		{
			sprite.Animation = "walk_up";
			sprite.FlipH = false;
		}
		else
		{
			sprite.Animation = "walk_down";
			sprite.FlipH = false;
		}

		if (!sprite.IsPlaying())
		{
			sprite.Play();
		}
	}
}
