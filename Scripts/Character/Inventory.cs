using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Inventory : Node
{
	[Export] public int maxItems;
	[Export] public int startingSeedCount;
	public enum ItemType {
		SEED1,
		SEED2,
		SEED3, 
		PLANT1,
		PLANT2,
		PLANT3
	}
	public List<int> items = new List<int>();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (int itemIdx in Enum.GetValues<ItemType>()){
			if(itemIdx < 3){
				items.Add(startingSeedCount);
			} else {
				items.Add(0);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
