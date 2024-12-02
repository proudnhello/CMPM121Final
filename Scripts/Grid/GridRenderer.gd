extends Node2D

var options : Dictionary
var grid_sprites : Array
var cell_visuals : PackedScene
var grid_manager : Node

func init(gd: Node) -> void:
	options = GameData.itemData["game settings"];
	grid_manager = gd
	grid_sprites = []
	for i in range(options["gridSize"]):
		grid_sprites.append([])
		for j in range(options["gridSize"]):
			grid_sprites[i].append(null)
	cell_visuals = ResourceLoader.load("res://Scenes/Cell.tscn")
	render_grid()

func get_cell_size() -> int:
	var cell_sprite = cell_visuals.instantiate().get_node("Background")
	var size = int(floor(cell_sprite.texture.get_width() * cell_sprite.scale.x))
	cell_sprite.queue_free()
	return size

func render_grid() -> void:
	for i in range(options["gridSize"]):
		for j in range(options["gridSize"]):
			render_cell(i, j)

func render_cell(x : int, y : int) -> void:
	var cell = grid_manager._fetch_cell_as_array(x, y)
	if grid_sprites[x][y] == null:
		var cell_node = cell_visuals.instantiate()
		var cell_sprite = cell_node.get_node("Background")
		cell_node.position = Vector2(
			ceil(x - (options.gridSize / 2)) * cell_sprite.texture.get_width() * cell_sprite.scale.x,
			ceil(y - (options.gridSize / 2)) * cell_sprite.texture.get_width() * cell_sprite.scale.y
		)
		grid_sprites[x][y] = cell_node
		grid_manager.add_child(grid_sprites[x][y])
	grid_sprites[x][y].update_labels(cell)
