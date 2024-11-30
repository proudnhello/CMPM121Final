extends Node

func _on_pressed():
		if(FileAccess.file_exists(("user://AutoSave.save"))):
			SceneSwitcher._start_scene_from_file("AutoSave");
		else:
			print("No save file found.");
