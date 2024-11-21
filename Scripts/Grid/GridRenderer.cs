using Godot;
using System;

public partial class GridRenderer : Node2D
{
	[Export] float gridCellSize;

    public void RenderGrid(Cell[] cells)
    {
        GD.Print(cells.Length);
    }

}
