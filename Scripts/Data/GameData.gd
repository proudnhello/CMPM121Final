extends Node

var itemData = {};

var dataFilepath = "res://Events/gridevents.json"

func LoadJSONFile(filepath: String):
	if FileAccess.file_exists(filepath):
		var dataFile = FileAccess.open(filepath, FileAccess.READ);
		var parsedResult = JSON.parse_string(dataFile.get_as_text());
		if parsedResult is Dictionary:
			return parsedResult;
		else:
			print("Error parsing JSON file: ", filepath);
	else:
		print("File not found: ", filepath);
		

func _ready() -> void:
	itemData = LoadJSONFile(dataFilepath);

