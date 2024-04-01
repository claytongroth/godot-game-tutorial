using Godot;
using System;

public partial class Player : Area2D
{
	[Signal]
	public delegate void HitEventHandler();
	
	[Export]
	public int Speed = 400;
	
	private Vector2 _screenSize;
	
	public override void _Ready()
	{
		_screenSize = GetViewport().GetVisibleRect().Size;
	}

	public override void _Process(double delta)
	{
		var velocity = new Vector2();

		if (Input.IsActionPressed("move_right"))
			velocity.X += 1;

		if (Input.IsActionPressed("move_left"))
			velocity.X -= 1;
		
		if (Input.IsActionPressed("move_up"))
			velocity.Y -= 1;

		if (Input.IsActionPressed("move_down"))
			velocity.Y += 1;

		var animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");

		if (velocity.Length() > 0)
		{
			velocity = velocity.Normalized() * Speed;
			animatedSprite.Play();
		}
		else
			animatedSprite.Stop();

		Position += velocity * (float)delta;
		Position = new Vector2(
			x: Mathf.Clamp(Position.X, 0, _screenSize.X),
			y: Mathf.Clamp(Position.Y, 0, _screenSize.Y));

		if (velocity.X != 0)
		{
			animatedSprite.Animation = "walk";
			animatedSprite.FlipH = velocity.X < 0;
		}
		else if (velocity.Y != 0)
		{
			animatedSprite.Animation = "up";
			animatedSprite.FlipV = velocity.Y > 0;
		}
		
		if (velocity.X < 0)
			animatedSprite.FlipH = true;
		else
			animatedSprite.FlipH = false;
	}
	
	private void OnBodyEntered(Node2D body)
	{
		GD.Print("BODY HIT");
		Hide(); // Player disappears after being hit.
		EmitSignal(SignalName.Hit);
		// Must be deferred as we can't change physics properties on a physics callback.
		GetNode<CollisionShape2D>("CollisionShape2D").SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
	}

	public void Start(Vector2 pos)
	{
		GD.Print("START!");
		Position = pos;
		Show();
		GetNode<CollisionShape2D>("CollisionShape2D").Disabled = false;
	}

}



