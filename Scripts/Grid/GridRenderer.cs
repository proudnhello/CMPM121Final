using Godot;

public class GridRenderer 
{
    GridOptions options;
    Grid grid;
    Node parentNode;
    Node2D[][] gridSprites;
    PackedScene cellVisuals;

    public GridRenderer(Grid grid, GridOptions options, Node parentNode) {
        this.grid = grid;
        this.options = options;
        this.parentNode = parentNode; 
        gridSprites = new Node2D[options.gridDimensions][];
		for (int i = 0; i < options.gridDimensions; i++) {
			gridSprites[i] = new Node2D[options.gridDimensions];
		}
        cellVisuals = ResourceLoader.Load<PackedScene>("res://Scenes/Cell.tscn");
        RenderGrid();
    }

    public int GetCellSize() {
        Sprite2D cellSprite = (Sprite2D)cellVisuals.Instantiate().GetNode("Background");
		int size = Mathf.FloorToInt(cellSprite.Texture.GetWidth() * cellSprite.Scale[0]);
		cellSprite.Free();
        return size;
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
			Node2D cellNode = (Node2D)cellVisuals.Instantiate();
			// Get the sprite of the cell, so we know the size of it, so we can position it correctly
			Sprite2D cellSprite = cellNode.GetNode<Sprite2D>("Background");
			cellNode.Position = new Vector2(
				(x - options.gridDimensions / 2) * cellSprite.Texture.GetWidth() * cellSprite.Scale[0],
				(y - options.gridDimensions / 2) * cellSprite.Texture.GetWidth() * cellSprite.Scale[1]
			);
			gridSprites[x][y] = cellNode;
			parentNode.AddChild(gridSprites[x][y]);
		}

		// Set the cell's levels
		UpdateCell cellScript = (UpdateCell)gridSprites[x][y];
		cellScript.UpdateLabels(cell);
	}
}