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

public void DisplayInventory(){
		foreach (int itemIdx in Enum.GetValues<ItemType>()){
			UpdateItemSlot itemSlot = GetNode<UpdateItemSlot>("ItemSlots/ItemSlot" + itemIdx);
			GD.Print(items[itemIdx]);
		}
}

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
		DisplayInventory();
	}

	public void AddItem(ItemType item, int amount){
		items[(int)item] += amount;
	}

	public void RemoveItem(ItemType item, int amount){
		if(items[(int)item] >= amount){
			items[(int)item] -= amount;
		}
		return;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// placeholders until other functions are implemented
		// if(Input.IsActionJustPressed("1")){
		// 	RemoveItem(ItemType.SEED1, 1);
		// }
		// if(Input.IsActionJustPressed("2")){
		// 	RemoveItem(ItemType.SEED2, 1);
		// }
		// if(Input.IsActionJustPressed("3")){
		// 	RemoveItem(ItemType.SEED3, 1);
		// }
		// if(Input.IsActionJustPressed("4")){
		// 	AddItem(ItemType.PLANT1, 1);
		// }
		// if(Input.IsActionJustPressed("5")){
		// 	AddItem(ItemType.PLANT2, 1);
		// }
		// if(Input.IsActionJustPressed("6")){
		// 	AddItem(ItemType.PLANT3, 1);
		// }
	}
}
