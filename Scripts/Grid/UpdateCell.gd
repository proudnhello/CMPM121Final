'''
using Godot;
using System;

public partial class UpdateCell : Node2D
{
	[Export] Label waterLevelLabel;
	[Export] Label sunLevelLabel;
	[Export] Label plantTypeLabel;
	[Export] Label growthLevelLabel;

	public void UpdateLabels(Cell cell) {
		waterLevelLabel.Text = "" + cell.waterLevel;
		sunLevelLabel.Text = "" + cell.sunLevel;
		plantTypeLabel.Text = "" + cell.plantType;
		growthLevelLabel.Text = "" + cell.plantLevel;
	}
}
'''
extends Node2D

@export var water_level_label: Label
@export var sun_level_label: Label
@export var plant_type_label: Label
@export var growth_level_label: Label

func update_labels(cell_array):
	water_level_label.text = str(cell_array[0])
	sun_level_label.text = str(cell_array[1])
	plant_type_label.text = str(cell_array[2])
	growth_level_label.text = str(cell_array[3])
