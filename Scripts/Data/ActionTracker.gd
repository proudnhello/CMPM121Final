extends Node

var actions = []
var redo_actions = []
var localSeed = 0
var program_counter = 0

func _init(seed_value = null):
	if seed_value == null:
		localSeed = Time.get_ticks_msec()
		if localSeed == 0:
			localSeed = 1
	else:
		localSeed = seed_value

	actions.append([-1, localSeed])
	program_counter = 0

func get_seed():
	return localSeed

func get_next_seed():
	var step_seed = localSeed + program_counter
	program_counter += 1
	return step_seed

func step_time(step_seed, grown_plants):
	var type_seed = [0, step_seed]
	actions.append(type_seed + Array(grown_plants))
	redo_actions.clear()

func un_step_time():
	program_counter -= 1

func plant_seed(x, y, plant_type):
	actions.append([1, x, y, plant_type])
	redo_actions.clear()

func harvest_plant(x, y, plant_type):
	actions.append([2, x, y, plant_type])
	redo_actions.clear()

func undo_action():
	if len(actions) <= 1:
		return null
	var action = actions.pop_back()
	redo_actions.append(action)
	return action

func redo_action():
	if len(redo_actions) == 0:
		return null
	var action = redo_actions.pop_back()
	actions.append(action)
	return action

func save(save_name):
	var save_file = FileAccess.open("user://" + save_name + ".save", FileAccess.WRITE)
	var autosave_file = FileAccess.open("user://AutoSave.save", FileAccess.READ)
	while not autosave_file.eof_reached():
		var line = autosave_file.get_line()
		save_file.store_line(line)

func auto_save():
	var save_name = "AutoSave"
	var save_file = FileAccess.open("user://" + save_name + ".save", FileAccess.WRITE)
	var action_array = actions.duplicate()
	for action in action_array:
		var json_action = JSON.stringify(action)
		save_file.store_line(json_action)
	save_file.store_line("")
	var redo_action_array = redo_actions.duplicate()
	for action in redo_action_array:
		var json_action = JSON.stringify(action)
		save_file.store_line(json_action)

func load(save_name):
	var save_path = "user://" + save_name + ".save"
	if not FileAccess.file_exists(save_path):
		return null

	actions.clear()
	redo_actions.clear()

	var save_file = FileAccess.open(save_path, FileAccess.READ)
	var parse_stage = 0
	while save_file.get_position() < save_file.get_length():
		var json_action = save_file.get_line()
		if json_action == "":
			parse_stage += 1
			continue
		var parse_json = JSON.new()
		parse_json.parse(json_action)
		var action = parse_json.data
		if parse_stage == 0:
			actions.append(action)
		else:
			print("Redo action" + str(action))
			redo_actions.append(action)

	var action_array = actions.duplicate()
	if action_array[0][0] != -1:
		print("No localSeed found in save file")
		return null
	localSeed = action_array[0][1]

	program_counter = -1
	for action in action_array:
		if action[0] == 0:
			program_counter += 1

	return action_array
