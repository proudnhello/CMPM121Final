using Godot;
using System;

public struct Cell {
	public int waterLevel;
	public int sunLevel;
	public int plantType;
	public int plantLevel;
}

public class Grid {
	public Cell[] currentCells;
	Cell[] swapCells;
	int dimensions;
	
	public Grid(int _dimensions) {
		dimensions = _dimensions;
		currentCells = new Cell[dimensions * dimensions];
		swapCells = new Cell[dimensions * dimensions];
	}

	public Cell[] GetCells() {
		return currentCells;
	}

	public void StepTime(int maxWaterLevelIncrease, int maxSunLevel) {
		
		// iterate through currentCells and store changes in swapCells, then swap pointers;
		for (int i = 0; i < currentCells.Length; i++) {
			int x = i % dimensions;
			int y = Mathf.FloorToInt(i / dimensions);
			
			// Increase water level by a random amount
			swapCells[i].waterLevel = currentCells[i].waterLevel + Mathf.FloorToInt(GD.RandRange(0, maxWaterLevelIncrease));

			// Set sun level to a random amount
			swapCells[i].sunLevel = Mathf.FloorToInt(GD.RandRange(0, maxSunLevel));
		}

		(swapCells, currentCells) = (currentCells, swapCells);
	}
}

public partial class GridManager : Node
{
	[Export(PropertyHint.Range, "1, 200")] public int gridDimensions;
	[Export] public int maxWaterLevelIncrease;
	[Export] public int maxSunLevel;

	public Grid grid;

	public override void _Ready()
	{
		grid = new Grid(gridDimensions);
		GD.Randomize();
	}

	public void ProgressTime() {
		// Progress time by one step
		grid.StepTime(maxWaterLevelIncrease, maxSunLevel);
		RenderGrid();
	}

	public void RenderGrid()
	{ 
		for (int i = 0; i < grid.currentCells.Length; i++) {
			int x = i % gridDimensions;
			int y = Mathf.FloorToInt(i / gridDimensions);
			Cell cell = grid.currentCells[i];
			GD.Print($"Cell at {x}, {y} has water level {cell.waterLevel} and sun level {cell.sunLevel}");
		}
	}
}
