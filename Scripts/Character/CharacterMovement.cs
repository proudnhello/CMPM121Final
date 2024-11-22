using Godot;
using System;

public partial class CharacterMovement : Node2D
{
	public int movementDistance = -1;
	
	public void SetMovementDistance(int distance) {
		movementDistance = distance;
		GD.Print("Movement distance set to " + distance);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (movementDistance == -1) {
			return;
		}
		if (Input.IsActionJustPressed("move_left")) {
			// Move left
			Translate(new Vector2(-movementDistance, 0));
		}
		if (Input.IsActionJustPressed("move_right")) {
			// Move right
			Translate(new Vector2(movementDistance, 0));
		}
		if (Input.IsActionJustPressed("move_up")) {
			// Move up
			Translate(new Vector2(0, -movementDistance));
		}
		if (Input.IsActionJustPressed("move_down")) {
			// Move down
			Translate(new Vector2(0, movementDistance));
		}
	}
	
	
}
