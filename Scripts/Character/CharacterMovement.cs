using Godot;
using System;

public partial class CharacterMovement : Node2D
{
	[Export] GridManager gridManager;
	int movementDistance = -1;
	Vector2 gridPosition = Vector2.Zero;
	int maxDimension;
	
	public void Init(int distance, int _maxDimension, Vector2 startPosition) {
		movementDistance = distance;
		maxDimension = _maxDimension;
		gridPosition = startPosition;
		Position = new Vector2(gridPosition.X - Mathf.FloorToInt(maxDimension/2), gridPosition.Y - Mathf.FloorToInt(maxDimension/2) ) * distance;
	}

	public void MovePosition(int x, int y){
		// check extrema
		if (x == -1 && gridPosition.X == 0 || x == 1 && gridPosition.X == maxDimension
		|| y == -1 && gridPosition.Y == 0 || y == 1 && gridPosition.Y == maxDimension) return;

		Translate(new Vector2(x * movementDistance, y * movementDistance));
		gridPosition = new Vector2(gridPosition.X + x, gridPosition.Y + y);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (movementDistance == -1) {
			return;
		}
		if (Input.IsActionJustPressed("move_left")) {
			// Move left
			MovePosition(-1, 0);
		}
		if (Input.IsActionJustPressed("move_right")) {
			// Move right
			MovePosition(1, 0);
		}
		if (Input.IsActionJustPressed("move_up")) {
			// Move up
			//gridManager.HarvestPlant((int)gridPosition.X, (int)gridPosition.Y);
			MovePosition(0, -1);
		}
		if (Input.IsActionJustPressed("move_down")) {
			// Move down
			MovePosition(0, 1);
		}
		if (Input.IsActionJustPressed("plant_seed")) {
			// Move down
			gridManager.PlantSeed((int)gridPosition.X, (int)gridPosition.Y);
		}
		if (Input.IsActionJustPressed("harvest")) {
			gridManager.HarvestPlant((int)gridPosition.X, (int)gridPosition.Y);
		}
	}
	
	
}
