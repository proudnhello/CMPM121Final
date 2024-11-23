using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

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
	
	public Grid(GridOptions _op) {
		op = _op;
		currentCells = new Cell[op.gridDimensions * op.gridDimensions];
		swapCells = new Cell[op.gridDimensions * op.gridDimensions];
	}

	public Cell[] GetCells() {
		return currentCells;
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

	// Check if the cell is valid
	public bool checkCellValidity(Cell cell){
		return cell.waterLevel != -1000;
	}

	public void StepTime() {

		// iterate through currentCells and store changes in swapCells, then swap pointers;
		for (int i = 0; i < currentCells.Length; i++) {
			int x = i % op.gridDimensions;
			int y = Mathf.FloorToInt(i / op.gridDimensions);
			
			// Increase water level by a random amount
			swapCells[i].waterLevel = currentCells[i].waterLevel + Mathf.FloorToInt(GD.RandRange(0, op.maxWaterLevelIncrease));

			// Set sun level to a random amount
			swapCells[i].sunLevel = Mathf.FloorToInt(GD.RandRange(0, op.maxSunLevel));

			// If the cell has a plant, save the plant type across
			swapCells[i].plantType = currentCells[i].plantType;

			// If the cell has a plant, check if it can grow
			if (currentCells[i].plantType == 0){
				swapCells[i].plantLevel = 0;
				continue;
			}

			// 
			if (CheckPlantRequirements(currentCells[i], x, y)) {
				swapCells[i].plantLevel = GrowPlant(currentCells[i]);
			}else{
				swapCells[i].plantLevel = currentCells[i].plantLevel;
			}
		}

		(swapCells, currentCells) = (currentCells, swapCells);
	}

	// Grows the plant in the cell, returns the new plant level
	public int GrowPlant(Cell cell) {
		return cell.plantLevel + 1;
	}

	// Checks if the plant in the cell can grow
	public bool CheckPlantRequirements(Cell cell, int x, int y) {
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
				if (!checkCellValidity(adjacentCell)) {
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

	public void PlantSeed(int x, int y, int plantType) {
		int index = y * op.gridDimensions + x;
		currentCells[index].plantType = plantType;
		currentCells[index].plantLevel = 1;
	}

	public void HarvestPlant(int x, int y) {
		int index = y * op.gridDimensions + x;
		currentCells[index].plantType = 0;
		currentCells[index].plantLevel = 0;
	}
}

public partial class GridManager : Node
{
	[Export] GridOptions options;
	
	[Export] Node2D player;

	[Export] PackedScene cellScene;

	[Export] public int numberOfPlantTypes = 3;

	public Grid grid;
	
	Node2D[][] gridSprites;

	public override void _Ready()
	{
		grid = new Grid(options);
		gridSprites = new Node2D[options.gridDimensions][];
		for (int i = 0; i < options.gridDimensions; i++) {
			gridSprites[i] = new Node2D[options.gridDimensions];
		}
		GD.Randomize();
		RenderGrid();

		// Set the player's movement distance
		CharacterMovement playerMovement = (CharacterMovement)player;
		// Get the sprite of the cell, so we know the size of it, so we can position it correctly
		// I don't like that we have to make a new cellSprite just to get the size of the cell, but idk how else to
		
		Sprite2D cellSprite = (Sprite2D)cellScene.Instantiate().GetNode("Background");
		playerMovement.Init(Mathf.FloorToInt(cellSprite.Texture.GetWidth() * cellSprite.Scale[0]), 
							options.gridDimensions-1, 
							new Vector2(Mathf.FloorToInt(options.gridDimensions/2), Mathf.FloorToInt(options.gridDimensions/2)));
		cellSprite.Free();
	}

	public void ProgressTime() {
		// Progress time by one step
		grid.StepTime();
		RenderGrid();
	}

	public void RenderGrid()
	{ 
		// Iterate through the grid and render the cells
		for (int i = 0; i < options.gridDimensions; i++){
			for (int j = 0; j < options.gridDimensions; j++){
				RenderCell(i, j);
			}
		}
	}

	public void RenderCell(int x, int y) {
		Cell cell = grid.FetchCell(x, y);
		// If the cell is null, create a new cell
		if (gridSprites[x][y] == null) {
			Node2D cellNode = (Node2D)cellScene.Instantiate();
			// Get the sprite of the cell, so we know the size of it, so we can position it correctly
			Sprite2D cellSprite = cellNode.GetNode<Sprite2D>("Background");
			cellNode.Position = new Vector2(
				(x - options.gridDimensions / 2) * cellSprite.Texture.GetWidth() * cellSprite.Scale[0],
				(y - options.gridDimensions / 2) * cellSprite.Texture.GetWidth() * cellSprite.Scale[1]
			);
			gridSprites[x][y] = cellNode;
			AddChild(gridSprites[x][y]);
		}

		// Set the cell's levels
		UpdateCell cellScript = (UpdateCell)gridSprites[x][y];
		cellScript.UpdateLabels(cell);
	}

	public void PlantSeed(int x, int y, int plantType) {
		if (grid.FetchCell(x, y).plantType != 0) return;
		grid.PlantSeed(x, y, plantType);
		GD.Print("Planting " + grid.FetchCell(x, y).plantType + " at (" + x + ", " + y + ")");
		RenderCell(x, y);
	}

	public void HarvestPlant(int x, int y) {
		if (grid.FetchCell(x, y).plantType == 0) return;
		grid.HarvestPlant(x, y);
		GD.Print("Harvesting at (" + x + ", " + y + ")");
		RenderCell(x, y);
	}
}
