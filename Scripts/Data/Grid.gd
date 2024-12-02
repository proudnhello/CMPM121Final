extends Resource

class_name Grid
@export var currentCells: PackedInt32Array;
var options: Dictionary;

# This is my attempt at creating a system that relies on a single PackedIntArray to store all the data required for the grid.
# In theory, the array will store 4 int values for each "cell", and the grid will be a 1D array of these "cell" structures.

# Cell will be structured like this:
# [waterLevel, sunLevel, plantType, plantLevel]

# Any cell corredsponding to row i and column j will start at index (i * width + j) * 4. It will be 4 integers (16 bytes) long.

var CELLSIZE = 4;
var NUMCELLS;

var PlantInfo;

func _create() -> void:
	options = GameData.itemData["game settings"];
	PlantInfo = PlantDatabase.retreivePlants();

	NUMCELLS = ((options["gridSize"] * options["gridSize"]));
	currentCells = PackedInt32Array();
	for i in range(NUMCELLS):
		currentCells.push_back(0);
		currentCells.push_back(0);
		currentCells.push_back(0);
		currentCells.push_back(0);

func _fetch_index(x, y) -> int:
	return (y * options["gridSize"] + x) * CELLSIZE;

func _fetch_cell(x, y) -> PackedInt32Array:
	if (x < 0 || x >= options["gridSize"] || y < 0 || y >= options["gridSize"]):
		#-1000 is an invalid value for waterLevel, so it will be used to indicate that the cell is out of bounds.
		return PackedInt32Array([-1000, 0, 0, 0]);

	var begin = _fetch_index(x, y);
	return currentCells.slice(begin, begin + CELLSIZE);

# Set the values of the cell at x, y to those contained in newValues.
func _set_cell(x, y, newValues: PackedInt32Array):
	var begin = _fetch_index(x, y);
	currentCells[begin] = newValues[0];
	currentCells[begin + 1] = newValues[1]
	currentCells[begin + 2] = newValues[2];
	currentCells[begin + 3] = newValues[3];

# Check if the cell at x, y has a plant and the plant is level 3 (is harvestable).
func _can_harvest(x, y) -> bool:
	var cell = _fetch_cell(x, y);
	if (cell[2] == 0 || cell[3] < PlantDatabase.get_requirements(cell[2]).maxGrowthLevel):
		return false;
	else:
		return true;

# Clear the board by setting all cells to [0, 0, 0, 0].
func _clear_board():
	for i in range(options["gridSize"]):
		for j in range(options["gridSize"]):
			_set_cell(i, j, PackedInt32Array([0, 0, 0, 0]));

# Check if the cell at x, y is a valid cell by comparing the waterIncreaseLevel to -1000.
func _is_valid(x, y) -> bool:
	if (_fetch_cell(x, y)[0] == -1000):
		return false;
	else:
		return true;

# Check if the plant in the cell can grow
func _check_plant_requirements(x, y):
	var cell = _fetch_cell(x, y);

	# If the cell has no plant, it can't grow.
	if (cell[2] == 0):
		return false;

	# Fetch the adjacent cells and count the number of adjacent plants and like plants.
	var adjacentPlants = 0;
	var likePlants = 0;
	for i in range(-1, 2):
		for j in range(-1, 2):
			if (i == 0 && j == 0):
				continue;
			if (_is_valid(x + i, y + j)):
				var adjCell = _fetch_cell(x + i, y + j);
				# 2 is the index of the plantType in the cell.
				if (adjCell[2] == cell[2]):
					likePlants += 1;
				if (adjCell[2] != 0):
					adjacentPlants += 1;
		
	return PlantDatabase.check_requirements(cell, likePlants, adjacentPlants);
	
func _step_time(randSeed) -> Array:
	# Not a single clue what growth array does, but presumably for the grid renderer?
	var growth = [];
	var waterRNG = RandomNumberGenerator.new();
	var sunRNG = RandomNumberGenerator.new();

	waterRNG.seed = randSeed;
	sunRNG.seed = randSeed;
	for i in range(options["gridSize"]):
		for j in range(options["gridSize"]):
			var index = _fetch_index(i, j);
			# 0 is water level, 1 is sun level, 2 is plant type, 3 is plant level

			# if plant can grow
			if(_check_plant_requirements(i, j)):
				# grow plant lol
				currentCells[index + 3] += 1;
				currentCells[index] -= PlantDatabase.get_requirements(currentCells[index + 2]).waterRequirement;
				growth.push_back(i);
				growth.push_back(j);

			# update water and sun levels
			currentCells[index] += waterRNG.randi_range(options.minWaterStep, options.maxWaterStep);

			currentCells[index + 1] = sunRNG.randi_range(options.minSunlight, options.maxSunlight);

	return growth;

func _plant_seed(x, y, plantType):
	var index = _fetch_index(x, y);
	currentCells[index + 2] = plantType;
	currentCells[index + 3] = 1;

func _harvest_plant(x, y):
	var index = _fetch_index(x, y);
	currentCells[index + 2] = 0;
	currentCells[index + 3] = 0;

func _unstep_time(waterSeed, sunSeed, actionInfo: Array):
	# create random generators based on given seeds
	var waterRNG = RandomNumberGenerator.new();
	waterRNG.seed = waterSeed;
	var sunRNG = RandomNumberGenerator.new();
	sunRNG.seed = sunSeed;

	for i in range(options["gridSize"]):
		for j in range(options["gridSize"]):
			var index = _fetch_index(i, j);
			
			currentCells[index] -= waterRNG.randi_range(options.minWaterStep, options.maxWaterStep);
			if(sunSeed == 0):
				currentCells[index + 1] = 0;	
			else:
				# no clue why you get a random sun value when you undo here. ?
				currentCells[index + 1] = sunRNG.randi_range(options.minSunlight, options.maxSunlight);

	for i in range(2, actionInfo.size(), 2):
		var index = _fetch_index(actionInfo[i], actionInfo[i + 1]);
		currentCells[index + 3] -=1;
		currentCells[index] += PlantDatabase.get_requirements(currentCells[index + 2]).waterRequirement;



# For unplanting and unharvesting, fetch cell used to return a value, but now when we change the 
# integers, we don't need to call any variant of SetCell.
func _unplant_seed(actionInfo: Array):
	var index = _fetch_index(actionInfo[1], actionInfo[2]);
	currentCells[index + 2] = 0;
	currentCells[index + 3] = 0;

func _unharvest_plant(actionInfo: Array):
	var index = _fetch_index(actionInfo[1], actionInfo[2]);
	var requirements = PlantDatabase.get_requirements(actionInfo[3]);

	currentCells[index + 2] = actionInfo[3];
	currentCells[index + 3] = requirements.maxGrowthLevel;
