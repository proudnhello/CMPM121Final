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
	gridRenderer.init(self);
	eventHappenings = EventHappenings.new();

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
	var grown = grid._step_time(curSeed);
	gridRenderer.render_grid();
	eventHappenings.check_events(actionTracker.get_time());
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

func _try_harvest_plant(x, y):
	if(!grid._can_harvest(x, y)): return;
	actionTracker.harvest_plant(x, y, grid._fetch_cell(x, y)[2]);
	_harvest_plant(x, y);

func _progress_time_button() -> void:
	var timeStepSeed = actionTracker.get_next_seed();
	var grownPlants = _step_time(timeStepSeed);
	actionTracker.step_time(timeStepSeed, grownPlants);

func _unplant_seed(actionInfo: Array):
	grid._unplant_seed(actionInfo);
	PlantSeedSignal.emit(actionInfo[3] - 1, 1);
	gridRenderer.render_cell(actionInfo[1], actionInfo[2]);

func _unharvest_plant(actionInfo: Array):
	grid._unharvest_plant(actionInfo);
	HarvestPlantSignal.emit(actionInfo[3] - 1, -1);
	gridRenderer.render_cell(actionInfo[1], actionInfo[2]);

func _unstep_time(actionInfo: Array):
	var waterSeed = actionInfo[1];
	var sunSeed = actionInfo[1] - 1;
	if(sunSeed < actionTracker.get_seed()):
		sunSeed = 0;
	grid._unstep_time(waterSeed, sunSeed, actionInfo);
	actionTracker.un_step_time();
	gridRenderer.render_grid();	
	eventHappenings.check_undo_events(actionTracker.get_time());

func _undo_action_button():
	var actionInfo = actionTracker.undo_action();
	#print("Action Info: ", actionInfo.stringify());
	if(actionInfo == null || actionInfo.size() == 0): return;

	if(actionInfo[0] == 0):
		_unstep_time(actionInfo);
	elif(actionInfo[0] == 1):
		_unplant_seed(actionInfo);
	elif(actionInfo[0] == 2):
		_unharvest_plant(actionInfo);

func _redo_action_button():
	var actionInfo = actionTracker.redo_action();
	print("program_counter: ", actionTracker.get_time());
	if(actionInfo == null || actionInfo.size() == 0): return;

	if(actionInfo[0] == 0):
		_step_time(actionInfo[1]);
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
	for i in range(actions.size()):
		actionArray.append(actions[i]);
	
	if(actions == null): return;
	get_tree().paused = true;
	grid._clear_board();
	inventory.ResetInventory();
	for i in range(actionArray.size()):
		var actionInfo = actionArray[i];
		#print("Loaded action info: ", actionInfo.stringify());
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
