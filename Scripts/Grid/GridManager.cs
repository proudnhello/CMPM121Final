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

	public void Iterate() {
		
		// iterate through currentCells and store changes in swapCells, then swap pointers;
		for (int i = 0; i < currentCells.Length; i++) {
			int x = i % dimensions;
			int y = Mathf.FloorToInt(i / dimensions);

			swapCells[i].waterLevel = currentCells[i].waterLevel + 1;
		}

        (swapCells, currentCells) = (currentCells, swapCells);
    }
}

public partial class GridManager : Node
{
	[Export(PropertyHint.Range, "1, 200")] public int gridDimensions;

	public GridRenderer GridRenderer;
	public Grid grid;

	public override void _Ready()
	{
		grid = new Grid(gridDimensions);
		GridRenderer = GetNode<GridRenderer>("/root/Node2D/GridManager/GridRenderer");
	}

	public void ProgressTime() {
		grid.Iterate();
		GridRenderer.RenderGrid(grid.GetCells());
	}
}

