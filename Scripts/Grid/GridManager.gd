extends Node

var options: Dictionary
@export var player: Node;
@export var cellScene: PackedScene;
@export var numberOfPlantTypes := 3;
@export var inventory: Node2D;
@export var actionTracker: Node;
@export var gridRenderer: Node;

var eventHappenings: EventHappenings
var grid: Grid;
var baseSeed: int;

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	options = GameData.itemData["game settings"];
	grid = Grid.new();
	grid._create();
	eventHappenings = EventHappenings.new();
	gridRenderer.init(self, eventHappenings);

	seed(actionTracker.get_seed());
	baseSeed = actionTracker.get_seed();

	# Set the player's movement distance		
	player.init( gridRenderer.get_cell_size(),
						options["gridSize"]-1, 
							   Vector2(floor(options["gridSize"]/2),
										   floor((options["gridSize"]/2))));

	if(SceneSwitcher.filepath != null):
		print("save file: ", SceneSwitcher.filepath);
		_load_file(SceneSwitcher.filepath);

func _fetch_cell_as_array(x, y) -> PackedInt32Array:
	return grid._fetch_cell(x, y);

func _step_time(curSeed) -> PackedInt32Array:
	eventHappenings.check_events(actionTracker.get_time());
	var grown = grid._step_time(curSeed);
	gridRenderer.render_grid();
	#print("program_counter: ", actionTracker.get_time(), " ", actionTracker.redo_actions.size());
	return grown;

signal PlantSeedSignal(plantType: int, number: int)

func _can_plant(x, y, plantType) -> bool:
	if(grid._fetch_cell(x, y)[2] != 0 || inventory.GetItemAmnt( plantType - 1) <= 0):
		return false;
	else:
		return true;

func _plant_seed(x, y, plantType):
	PlantSeedSignal.emit(plantType -1, -1);
	grid._plant_seed(x, y, plantType);
	gridRenderer.render_cell(x, y);

signal HarvestPlantSignal(plantType: int, number: int);

func _harvest_plant(x, y):
	HarvestPlantSignal.emit(grid._fetch_cell(x, y)[2] -1, 1);
	grid._harvest_plant(x, y);
	gridRenderer.render_cell(x, y);

func _try_plant_seed(x, y, plantType):
	if(!_can_plant(x, y, plantType)): return;
	actionTracker.plant_seed(x, y, plantType);
	_plant_seed(x, y, plantType);
	actionTracker.new_action();

func _try_harvest_plant(x, y):
	if(!grid._can_harvest(x, y)): return;
	actionTracker.harvest_plant(x, y, grid._fetch_cell(x, y)[2]);
	_harvest_plant(x, y);
	actionTracker.new_action();

func _progress_time_button() -> void:
	var grown = _step_time(actionTracker.get_next_seed());
	actionTracker.step_time(grown);
	actionTracker.new_action();

func _unplant_seed(actionInfo: Array):
	grid._unplant_seed(actionInfo);
	PlantSeedSignal.emit(actionInfo[3] - 1, 1);
	gridRenderer.render_cell(actionInfo[1], actionInfo[2]);

func _unharvest_plant(actionInfo: Array):
	grid._unharvest_plant(actionInfo);
	HarvestPlantSignal.emit(actionInfo[3] - 1, -1);
	gridRenderer.render_cell(actionInfo[1], actionInfo[2]);

func _unstep_time(actionInfo: Array):
	actionTracker.un_step_time();
	var waterSeed = actionInfo[1];
	var sunSeed = actionInfo[1] - 1;
	if(sunSeed < actionTracker.get_seed()):
		sunSeed = 0;

	grid._unstep_time(waterSeed, sunSeed, actionInfo);

	var time = actionTracker.get_time()-1;
	time = 0 if (time < 0) else time;
	eventHappenings.check_undo_events(time);
	gridRenderer.render_grid();
	#print("program_counter: ", actionTracker.get_time());

func _undo_action_button():
	var actionInfo = actionTracker.undo_action();
	if(actionInfo == null || actionInfo.size() == 0): return;

	if(actionInfo[0] == 0):
		_unstep_time(actionInfo);
	elif(actionInfo[0] == 1):
		_unplant_seed(actionInfo);
	elif(actionInfo[0] == 2):
		_unharvest_plant(actionInfo);

func _redo_action_button():
	var actionInfo = actionTracker.redo_action();
	if(actionInfo == null || actionInfo.size() == 0): return;

	if(actionInfo[0] == 0):
		_step_time(actionInfo[1]);
		actionTracker.increment_program_counter();
	elif(actionInfo[0] == 1):
		_plant_seed(actionInfo[1], actionInfo[2], actionInfo[3]);
	elif(actionInfo[0] == 2):
		_harvest_plant(actionInfo[1], actionInfo[2]);


func _save(saveName):
	actionTracker.save(saveName);

func _load_file(saveName):
	print("Loading save file: ", saveName);
	var actions = actionTracker.load(saveName);
	var actionArray = [];
	if actions == null: return;
	for i in range(actions.size()):
		actionArray.append(actions[i]);
	
	if(actions == null): return;
	get_tree().paused = true;
	grid._clear_board();
	inventory.ResetInventory();
	eventHappenings.reset();
	for i in range(actionArray.size()):
		var actionInfo = actionArray[i];
		if(actionInfo[0] == 0):
			_step_time(actionInfo[1]);
			print("Loaded step time");
		elif(actionInfo[0] == 1):
			_plant_seed(actionInfo[1], actionInfo[2], actionInfo[3]);
			print("Loaded plant seed at ", actionInfo[1], " ",actionInfo[2]);
		elif(actionInfo[0] == 2):
			_harvest_plant(actionInfo[1], actionInfo[2]);
			print("Loaded harvest plant at ", actionInfo[1], " ",actionInfo[2]);
	get_tree().paused = false;
	gridRenderer.render_grid();
	print(actionTracker.get_time());
