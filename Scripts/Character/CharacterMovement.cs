using Godot;
using System;

public partial class CharacterMovement : Node2D
{
	[Export] public float moveSpeed = 10f;
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//
		//
		Vector2 move = new Vector2(Input.GetAxis("move_left", "move_right"), Input.GetAxis("move_up", "move_down")).Normalized() * moveSpeed;
		Translate(move);
	}
	
	
}
