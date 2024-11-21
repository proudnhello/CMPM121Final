using Godot;
using System;

public struct Cell {
	int waterLevel;
	int sunLevel;
	int plantType;
	int plantLevel;
}

public partial class GridManager : Node
{
	[Export(PropertyHint.Range, "1, 200")] public int gridDimensions;

	public Cell[] cells;
	public GridRenderer GridRenderer;

	public override void _Ready()
	{
		cells = new Cell[gridDimensions * gridDimensions];
		GridRenderer = GetNode<GridRenderer>("/root/Node2D/GridManager/GridRenderer");
	}

	public void ProgressTime() {
		GridRenderer.RenderGrid(cells);
	}
}

