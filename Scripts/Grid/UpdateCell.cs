using Godot;
using System;

public partial class UpdateCell : Node2D
{
	[Export] Label waterLevelLabel;
	[Export] Label sunLevelLabel;
	[Export] Label plantTypeLabel;

	public void UpdateWaterLevel(int waterLevel) {
		waterLevelLabel.Text = "" + waterLevel;
	}

	public void UpdateSunLevel(int sunLevel) {
		sunLevelLabel.Text = "" + sunLevel;
	}

	public void UpdatePlantType(int plantType) {
		plantTypeLabel.Text = "" + plantType;
	}
}
