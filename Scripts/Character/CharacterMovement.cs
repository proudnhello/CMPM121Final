using Godot;
using System;

public partial class CharacterMovement : Node2D
{
	public int movementDistance = -1;
	Vector2 gridPosition = Vector2.Zero;

	int maxDimension;
	
	public void Init(int distance, int _maxDimension, Vector2 startPosition) {
		movementDistance = distance;
		maxDimension = _maxDimension;
		gridPosition = startPosition;
		Position = new Vector2(gridPosition.X - Mathf.FloorToInt(maxDimension/2), gridPosition.Y - Mathf.FloorToInt(maxDimension/2) ) * distance;
		GD.Print("Movement distance set to " + distance);
	}

	public void SetPosition(int x, int y){
		// check extrema
		if (x == -1 && gridPosition.X == 0 || x == 1 && gridPosition.X == maxDimension
		|| y == -1 && gridPosition.Y == 0 || y == 1 && gridPosition.Y == maxDimension) return;

		Translate(new Vector2(x * movementDistance, y * movementDistance));
		gridPosition = new Vector2(gridPosition.X + x, gridPosition.Y + y);
		GD.Print(gridPosition);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (movementDistance == -1) {
			return;
		}
		if (Input.IsActionJustPressed("move_left")) {
			// Move left
			SetPosition(-1, 0);
		}
		if (Input.IsActionJustPressed("move_right")) {
			// Move right
			SetPosition(1, 0);
		}
		if (Input.IsActionJustPressed("move_up")) {
			// Move up
			SetPosition(0, -1);
		}
		if (Input.IsActionJustPressed("move_down")) {
			// Move down
			SetPosition(0, 1);
		}
	}
	
	
}
