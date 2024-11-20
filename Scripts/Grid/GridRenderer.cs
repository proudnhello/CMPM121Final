using Godot;
using System;

public partial class GridRenderer : Node2D
{
	Camera2D camera2D;
	[Export] float gridSize;
	[Export] float gridCellSize;
	public override void _Ready()
	{
		camera2D = (Camera2D)GetNode("Camera2D");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Draw()
	{
		// for (int i = 0; i < gridSize; i++) {
		// 	DrawLine(new Vector2(i*(1/gridSize), -gridSize), new Vector2(i*(1/gridSize),gridSize), Color.Color8(0, 0, 0, 1), 5);
		// }
		// for (int i = 0; i < gridSize; i++) {
		// 	DrawLine(new Vector2(-gridSize, i*(1/gridSize)), new Vector2(gridSize, i*(1/gridSize)), Color.Color8(0, 0, 0, 1), 5);
		// }
	}
}
