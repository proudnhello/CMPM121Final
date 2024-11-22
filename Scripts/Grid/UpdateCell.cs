using Godot;
using System;

public partial class UpdateCell : Node2D
{
	[Export] Label waterLevelLabel;
	[Export] Label sunLevelLabel;
	[Export] Label plantTypeLabel;

	public void UpdateLabels(Cell cell) {
		waterLevelLabel.Text = "" + cell.waterLevel;
		sunLevelLabel.Text = "" + cell.sunLevel;
		plantTypeLabel.Text = "" + cell.plantType;
	}
}
