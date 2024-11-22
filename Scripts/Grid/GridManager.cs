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

	// Fetch the cell at (x, y) in the grid
	public Cell FetchCell(int x, int y) {
		return currentCells[y * dimensions + x];
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
	
	[Export] Node2D player;

	[Export] PackedScene cellScene;

	public Grid grid;
	
	Node2D[][] gridSprites;

	public override void _Ready()
	{
		grid = new Grid(gridDimensions);
		gridSprites = new Node2D[gridDimensions][];
		for (int i = 0; i < gridDimensions; i++) {
			gridSprites[i] = new Node2D[gridDimensions];
		}
		GD.Randomize();
		RenderGrid();

		// Set the player's movement distance
		CharacterMovement playerMovement = (CharacterMovement)player;
		// Get the sprite of the cell, so we know the size of it, so we can position it correctly
		// I don't like that we have to make a new cellSprite just to get the size of the cell, but idk how else to
		Sprite2D cellSprite = (Sprite2D)cellScene.Instantiate().GetNode("Background");
		playerMovement.SetMovementDistance(Mathf.FloorToInt(cellSprite.Texture.GetWidth() * cellSprite.Scale[0]));
		cellSprite.Free();
	}

	public void ProgressTime() {
		// Progress time by one step
		grid.StepTime(maxWaterLevelIncrease, maxSunLevel);
		RenderGrid();
	}

	public void RenderGrid()
	{ 
		// Iterate through the grid and render the cells
		for (int i = 0; i < gridDimensions; i++){
			for (int j = 0; j < gridDimensions; j++){
				Cell cell = grid.FetchCell(i, j);

				// If the cell is null, create a new cell
				if (gridSprites[i][j] == null) {
					Node2D cellNode = (Node2D)cellScene.Instantiate();
					// Get the sprite of the cell, so we know the size of it, so we can position it correctly
					Sprite2D cellSprite = cellNode.GetNode<Sprite2D>("Background");
					cellNode.Position = new Vector2(
						(i - gridDimensions / 2) * cellSprite.Texture.GetWidth() * cellSprite.Scale[0],
						(j - gridDimensions / 2) * cellSprite.Texture.GetWidth() * cellSprite.Scale[1]
					);
					gridSprites[i][j] = cellNode;
					AddChild(gridSprites[i][j]);
				}

				// Set the cell's levels
				UpdateCell cellScript = (UpdateCell)gridSprites[i][j];
				cellScript.UpdateWaterLevel(cell.waterLevel);
				cellScript.UpdateSunLevel(cell.sunLevel);
			}
		}
	}
}
