extends Node

@export var filepath = null;

func _start_scene_from_scratch():
	filepath = null;
	var _absolutePath = ProjectSettings.globalize_path("user://AutoSave.save");
	DirAccess.remove_absolute(_absolutePath)
	get_tree().change_scene_to_file("res://Scenes/Gameplay.tscn");


func _start_scene_from_file(newpath):
	filepath = newpath;
	get_tree().change_scene_to_file("res://Scenes/Gameplay.tscn");