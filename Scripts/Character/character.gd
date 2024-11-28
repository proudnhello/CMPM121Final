'''
C# code for the CharacterMovement class
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
			MovePosition(0, -1);
		}
		if (Input.IsActionJustPressed("move_down")) {
			// Move down
			MovePosition(0, 1);
		}
		for (int i = 1; i <= gridManager.numberOfPlantTypes; i++) {
			// If the player presses a number key, plant the corresponding seed
			if (Input.IsActionJustPressed("plant_seed" + i)) {
				gridManager.TryPlantSeed((int)gridPosition.X, (int)gridPosition.Y, i);
			}
		}
		if (Input.IsActionJustPressed("harvest")) {
			gridManager.TryHarvestPlant((int)gridPosition.X, (int)gridPosition.Y);
		}
	}
	
	
}
'''
extends Node2D

@export var grid_manager: Node
var movement_distance = -1
var grid_position = Vector2.ZERO
var max_dimension = 0

func init(distance: int, _max_dimension: int, start_position: Vector2) -> void:
	movement_distance = distance
	max_dimension = _max_dimension
	grid_position = start_position
	position = (grid_position - Vector2(floor(max_dimension / 2.0), floor(max_dimension / 2.0))) * distance

func move_position(x: int, y: int) -> void:
	# check extrema
	if (x == -1 and grid_position.x == 0 or x == 1 and grid_position.x == max_dimension
	or y == -1 and grid_position.y == 0 or y == 1 and grid_position.y == max_dimension):
		return

	translate(Vector2(x * movement_distance, y * movement_distance))
	grid_position += Vector2(x, y)

func _process(_delta: float) -> void:
	if movement_distance == -1:
		return
	if Input.is_action_just_pressed("move_left"):
		# Move left
		move_position(-1, 0)
	if Input.is_action_just_pressed("move_right"):
		# Move right
		move_position(1, 0)
	if Input.is_action_just_pressed("move_up"):
		# Move up
		move_position(0, -1)
	if Input.is_action_just_pressed("move_down"):
		# Move down
		move_position(0, 1)
	for i in range(1, grid_manager.numberOfPlantTypes + 1):
		# If the player presses a number key, plant the corresponding seed
		if Input.is_action_just_pressed("plant_seed" + str(i)):
			grid_manager.TryPlantSeed(int(grid_position.x), int(grid_position.y), i)
	if Input.is_action_just_pressed("harvest"):
		grid_manager.TryHarvestPlant(int(grid_position.x), int(grid_position.y))
