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
	for i in range(1, PlantDatabase.NumberOfPlantTypes + 1):
		# If the player presses a number key, plant the corresponding seed
		if Input.is_action_just_pressed("plant_seed" + str(i)):
			grid_manager._try_plant_seed(int(grid_position.x), int(grid_position.y), i)
	if Input.is_action_just_pressed("harvest"):
		grid_manager._try_harvest_plant(int(grid_position.x), int(grid_position.y))
