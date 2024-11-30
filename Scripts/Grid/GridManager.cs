using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

public struct Cell {
	public int waterLevel;
	public int sunLevel;
	public int plantType;
	public int plantLevel;
}

public class Grid {
	public Cell[] currentCells;
	Cell[] swapCells;
	GridOptions op;
	Node inventory;

	
	public Grid(GridOptions _op, Node inventory) {
		op = _op;
		currentCells = new Cell[op.gridDimensions * op.gridDimensions];
		swapCells = new Cell[op.gridDimensions * op.gridDimensions];
		this.inventory = inventory;
	}

	// Fetch the cell at (x, y) in the grid
	public Cell FetchCell(int x, int y) {
		if (x < 0 || x >= op.gridDimensions || y < 0 || y >= op.gridDimensions) {
			Cell invalidCell = new Cell();
			// -1000 is an invalid water level
			invalidCell.waterLevel = -1000;
			return invalidCell;
		}
		return currentCells[y * op.gridDimensions + x];
	}

	public void SetCell(int x, int y, Cell cell) {
		currentCells[y * op.gridDimensions + x] = cell;
	}

	public int FetchIndex(int x, int y){
		return y * op.gridDimensions + x;
	}

	public bool CanPlant(int x, int y, int plantType) {
		if (FetchCell(x, y).plantType != 0 || (int)inventory.Call("GetItemAmnt", plantType - 1) <= 0) return false;
		else return true;
	}

	public bool CanHarvest(int x, int y) {
		if (FetchCell(x, y).plantType == 0 || FetchCell(x, y).plantLevel < 3) return false;
		else return true;
	}

	public void ClearBoard() {
		for (int i = 0; i < currentCells.Length; i++) {
			currentCells[i].waterLevel = 0;  currentCells[i].sunLevel = 0; currentCells[i].plantType = 0; currentCells[i].plantLevel = 0;
		}
	}

		// Check if the cell is valid
	bool IsCellValid(Cell cell){
		return cell.waterLevel != -1000;
	}

	// Checks if the plant in the cell can grow
	bool CheckPlantRequirements(Cell cell, int x, int y) {
		PlantGrowthRequirement requirements = op.GetPlantRequirements(cell.plantType);
		// Check the simple requirements that don't require other cells
		if (!requirements.checkSimpleRequirements(cell)) {
			return false;
		}

		// Check the requirements that require other cells
		int adjacentPlants = 0;
		int likePlants = 0;
		// Check the 8 adjacent cells 
		for (int dx = -1; dx <= 1; dx++) {
			for (int dy = -1; dy <= 1; dy++) {
				if (dx == 0 && dy == 0) continue; // Skip the current cell
				Cell adjacentCell = FetchCell(x + dx, y + dy);
				if (!IsCellValid(adjacentCell)) {
					continue;
				}
				if (adjacentCell.plantType != 0){
					adjacentPlants++;
				}
				if (adjacentCell.plantType == cell.plantType) {
					likePlants++;
				}
			}
		}

		// Check if the number of adjacent plants and like plants meet the requirements
		if (adjacentPlants < requirements.minAdjPlants || 
		likePlants < requirements.minLikePlants || 
		likePlants > requirements.maxLikePlants || 
		adjacentPlants > requirements.maxAdjPlants) {
			return false;
		}
		return true;
	}

	// Returns a list of all the cells that grew a plant
	public int[] StepTime(int seed) {
		// The rng seperation is for when we need to undo actions, the sun will need to use the previous time step's seed, while the water will use the current time step's seed
		List<int> growth = new List<int>();
		// So we can't just use one rng for both
		RandomNumberGenerator waterRNG = new RandomNumberGenerator();
		RandomNumberGenerator sunRNG = new RandomNumberGenerator();

		waterRNG.Seed = (ulong)seed;
		sunRNG.Seed = (ulong)seed;
		// iterate through currentCells and store changes in swapCells, then swap pointers;
		for (int i = 0; i < currentCells.Length; i++) {
			int x = i % op.gridDimensions;
			int y = Mathf.FloorToInt(i / op.gridDimensions);
			
			// Increase water level by a random amount
			swapCells[i].waterLevel = currentCells[i].waterLevel + waterRNG.RandiRange(0, op.maxWaterLevelIncrease);

			// Set sun level to a random amount
			swapCells[i].sunLevel = sunRNG.RandiRange(0, op.maxSunLevel);

			// If the cell has a plant, save the plant type across
			swapCells[i].plantType = currentCells[i].plantType;

			// If the cell has a plant, check if it can grow
			if (currentCells[i].plantType == 0){
				swapCells[i].plantLevel = 0;
				continue;
			}

			// If the plant can grow, grow it
			if (CheckPlantRequirements(currentCells[i], x, y)) {
				swapCells[i].plantLevel = currentCells[i].plantLevel + 1;
				swapCells[i].waterLevel -= op.GetPlantRequirements(swapCells[i].plantType).waterRequirement;
				growth.Add(x);
				growth.Add(y);
			}else{
				swapCells[i].plantLevel = currentCells[i].plantLevel;
			}
		}

		(swapCells, currentCells) = (currentCells, swapCells);
		return growth.ToArray();
	}

	
	public void PlantSeed(int x, int y, int plantType) {
		int index = y * op.gridDimensions + x;
		currentCells[index].plantType = plantType;
		currentCells[index].plantLevel = 1;
	}

	public void HarvestPlant(int x, int y) {
		int index = y * op.gridDimensions + x;
		currentCells[index].plantType = 0;
		currentCells[index].plantLevel = 0;
		swapCells[index].plantType = 0;
		swapCells[index].plantLevel = 0;
	}

	public void UnStepTime(int waterSeed, int sunSeed, int[] actionInfo) {
		// Action info will contain a list of all of the cells that grew a plant, so we need to un-grow them and add the water back
		for (int i = 2; i < actionInfo.Length; i+=2) {
			int x = actionInfo[i];
			int y = actionInfo[i+1];
			int index = FetchIndex(x, y);

			currentCells[index].plantLevel--;
			currentCells[index].waterLevel += op.GetPlantRequirements(currentCells[index].plantType).waterRequirement;
		}

		RandomNumberGenerator waterRNG = new RandomNumberGenerator();
		waterRNG.Seed = (ulong)waterSeed;
		RandomNumberGenerator sunRNG = new RandomNumberGenerator();
		sunRNG.Seed = (ulong)sunSeed;
		// iterate through currentCells and store changes in swapCells, then swap pointers;
		for (int i = 0; i < currentCells.Length; i++) {
			int x = i % op.gridDimensions;
			int y = Mathf.FloorToInt(i / op.gridDimensions);
			
			// Decreace water level by the same random amount
			swapCells[i].waterLevel = currentCells[i].waterLevel - waterRNG.RandiRange(0, op.maxWaterLevelIncrease);

			// If the sun seed is 0, that means we've reached the start of the game, so we need to set the sun level to 0
			if (sunSeed == 0){
				swapCells[i].sunLevel = 0;
			}else{
				swapCells[i].sunLevel = sunRNG.RandiRange(0, op.maxSunLevel);
			}

			// If the cell has a plant, save the plant type across
			swapCells[i].plantType = currentCells[i].plantType;
			swapCells[i].plantLevel = currentCells[i].plantLevel;
		}

		(swapCells, currentCells) = (currentCells, swapCells);
	}

	public void UnPlantSeed(int[] actionInfo) {
		int x = actionInfo[1];
		int y = actionInfo[2];
		Cell cell = FetchCell(x, y);
		
		cell.plantType = 0;
		cell.plantLevel = 0;

		SetCell(x, y, cell);
	}
	
	public void UnHarvestPlant(int[] actionInfo) {
		int x = actionInfo[1];
		int y = actionInfo[2];
		Cell cell = FetchCell(x, y);
		PlantGrowthRequirement requirements = op.GetPlantRequirements(actionInfo[3]);
		
		cell.plantType = actionInfo[3];
		cell.plantLevel = requirements.maxGrowthLevel;

		SetCell(x, y, cell);
	}

	// Grows the plant in the cell, returns the new plant level
	Cell GrowPlant(Cell cell) {
		cell.plantLevel++;
		cell.waterLevel -= op.GetPlantRequirements(cell.plantType).waterRequirement;
		return cell;
	}

}

public partial class GridManager : Node
{
	[Export] GridOptions options;
	
	[Export] Node player;

	[Export] PackedScene cellScene;

	[Export] public int numberOfPlantTypes = 3;

	[Export] Node2D inventory;

	public Grid grid;
	[Export] public Node actionTracker;

	public GridRenderer gridRenderer;
	
	public int baseSeed;

	public override void _Ready()
	{
		grid = new Grid(options, inventory);
		gridRenderer = new(grid, options, this);

		GD.Seed((uint)actionTracker.Call("get_seed"));
		baseSeed = (int)actionTracker.Call("get_seed");

		// Set the player's movement distance		
		player.Call("init", gridRenderer.GetCellSize(), options.gridDimensions-1, 
			new Vector2(Mathf.FloorToInt(options.gridDimensions/2), Mathf.FloorToInt(options.gridDimensions/2)));

		// this is already GD script, but commented out until everything else gets translated
		//if(SceneSwitcher.filepath != null):
		//	print("save file: ", SceneSwitcher.filepath);
		//	Load(SceneSwitcher.filepath);
	}

	
	int[] StepTime(int seed) {
		int[] grownPlants = grid.StepTime(seed);
		gridRenderer.RenderGrid();
		return grownPlants;
	}

	
	void PlantSeed(int x, int y, int plantType) {
		EmitSignal("PlantSeedSignal", plantType - 1, -1);
		grid.PlantSeed(x, y, plantType);
		gridRenderer.RenderCell(x, y);
	}

	
	void HarvestPlant(int x, int y) {
		EmitSignal("HarvestPlantSignal", grid.FetchCell(x, y).plantType - 1, 1);
		grid.HarvestPlant(x, y);
		gridRenderer.RenderCell(x, y);
	}


	[Signal]
	public delegate void PlantSeedSignalEventHandler(int plantType, int number);

	public void TryPlantSeed(int x, int y, int plantType) {
		if (!grid.CanPlant(x, y, plantType)) return;
		actionTracker.Call("plant_seed", x, y, plantType);
		PlantSeed(x, y, plantType);	
	}

	[Signal]
	public delegate void HarvestPlantSignalEventHandler(int plantType, int number);

	public void TryHarvestPlant(int x, int y) {
		if (!grid.CanHarvest(x, y)) return;
		Cell cell = grid.FetchCell(x, y);
		actionTracker.Call("harvest_plant", x, y, cell.plantType);
		HarvestPlant(x, y);
	}

	
	public void ProgressTimeButton() {
		 // Progress time by one step
		int timeStepSeed = (int)actionTracker.Call("get_next_seed");
		int[] grownPlants = StepTime(timeStepSeed);
		actionTracker.Call("step_time", timeStepSeed, grownPlants);
	}

	public void UnPlantSeed(int[] actionInfo) {
		grid.UnPlantSeed(actionInfo);
		EmitSignal("PlantSeedSignal", actionInfo[3] - 1, 1);
		gridRenderer.RenderCell(actionInfo[1], actionInfo[2]);
	}

	public void UnHarvestPlant(int[] actionInfo) {
		grid.UnHarvestPlant(actionInfo);
		EmitSignal("HarvestPlantSignal", actionInfo[3] - 1, -1);
		gridRenderer.RenderCell(actionInfo[1], actionInfo[2]);
	}

	public void UnStepTime(int[] actionInfo) {
		int waterSeed = actionInfo[1];
		int sunSeed = actionInfo[1] - 1;
		if (sunSeed < (int)actionTracker.Call("get_seed")) sunSeed = 0;
		grid.UnStepTime(waterSeed, sunSeed, actionInfo);
		actionTracker.Call("un_step_time");
		gridRenderer.RenderGrid();
	}

	public void UndoActionButton() {
		Godot.Collections.Array variants = (Godot.Collections.Array)actionTracker.Call("undo_action");
		int[] actionInfo = ConvertGodotArrayToArray(variants);
		GD.Print("Action info: ", actionInfo.Stringify());
		if (actionInfo == null || actionInfo.Length == 0) return;

		if (actionInfo[0] == 0) {
			UnStepTime(actionInfo);
		} else if (actionInfo[0] == 1) {
			UnPlantSeed(actionInfo);
		} else if (actionInfo[0] == 2) {
			UnHarvestPlant(actionInfo);
		}
	}

	public void RedoActionButton() {
		Godot.Collections.Array variants = (Godot.Collections.Array)actionTracker.Call("redo_action");
		int[] actionInfo = ConvertGodotArrayToArray(variants);
		if (actionInfo == null || actionInfo.Length == 0) return;

		if (actionInfo[0] == 0) {
			StepTime(actionInfo[1]);
		} else if (actionInfo[0] == 1) {
			PlantSeed(actionInfo[1], actionInfo[2], actionInfo[3]);
		} else if (actionInfo[0] == 2) {
			HarvestPlant(actionInfo[1], actionInfo[2]);
		}
	}

	public void Save(string saveName) {
		actionTracker.Call("save",saveName);
	}

	public void Load(string saveName) {
		GD.Print("Loading save file: ", saveName);
		Godot.Collections.Array actions = (Godot.Collections.Array)actionTracker.Call("load", saveName);
		int[][] actionsArray = new int[actions.Count][];
		for (int i = 0; i < actions.Count; i++) {
			actionsArray[i] = ConvertGodotArrayToArray((Godot.Collections.Array)actions[i]);
		}

		if (actions == null) return;
		GetTree().Paused = true;
		grid.ClearBoard();
		inventory.Call("ResetInventory");
		foreach (var actionInfo in actionsArray) {
			GD.Print("Loaded action info: ", actionInfo.Stringify());
			if (actionInfo[0] == 0) {
				StepTime(actionInfo[1]);
				GD.Print("Loaded step time");
			} else if (actionInfo[0] == 1) {
				PlantSeed(actionInfo[1], actionInfo[2], actionInfo[3]);
				GD.Print("Loaded plant seed at ", actionInfo[1], " ",actionInfo[2]);
			} else if (actionInfo[0] == 2) {
				HarvestPlant(actionInfo[1], actionInfo[2]);
				GD.Print("Loaded harvest plant at ", actionInfo[1], " ",actionInfo[2]);
			}
			GD.Print("\n");
		}
		GetTree().Paused = false;
		gridRenderer.RenderGrid();
	}

	public int[] ConvertGodotArrayToArray(Godot.Collections.Array godotArray) {
		int[] array = new int[godotArray.Count];
		try{
			for (int i = 0; i < godotArray.Count; i++) {
				array[i] = (int)godotArray[i];
			}
		}catch(Exception e){
			return null;
		}
		return array;
	}
}
