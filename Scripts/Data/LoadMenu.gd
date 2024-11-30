extends Node

# # Called when the node enters the scene tree for the first time.
# func _ready() -> void:
# 	var loadMenu = this;
# 	var popupMenu = loadMenu.GetPopup();
# 	popup_menu.connect("id_pressed", self.callable("LoadFromFile"))


# #id is defined inside the Godot editor
# #click on SaveMenu -> Items to see or make changes to id 
# func _load_from_file(fileID):
# 		match fileID:
# 			1:
# 				$GridManager.Load("File1");
# 			2:
# 				$GridManager.Load("File2");
# 			3:
# 				$GridManager.Load("File3");


@export var grid_manager: Node

func _ready() -> void:
	var loadMenu = self
	var popupMenu = loadMenu.get_popup()
	popupMenu.connect("id_pressed", Callable(self, "_load_from_file"))

func _load_from_file(fileID: int) -> void:
	match fileID:
		1:
			grid_manager.Load("File1")
		2:
			grid_manager.Load("File2")
		3:
			grid_manager.Load("File3")