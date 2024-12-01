extends Node

@export var grid_manager: Node

func _ready() -> void:
	var loadMenu = self
	var popupMenu = loadMenu.get_popup()
	popupMenu.connect("id_pressed", Callable(self, "_save_to_file"))

func _save_to_file(fileID: int) -> void:
	match fileID:
		1:
			grid_manager._save("File1")
		2:
			grid_manager._save("File2")
		3:
			grid_manager._save("File3")
