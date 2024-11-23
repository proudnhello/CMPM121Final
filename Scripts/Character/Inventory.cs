using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Inventory : Node
{
	[Export] public int numUniqueItems = 6;
	[Export] PackedScene itemSlotScene;
	[Export] public int startingSeedCount = 3;
	public enum ItemType {
		SEED1,
		SEED2,
		SEED3, 
		PLANT1,
		PLANT2,
		PLANT3
	}
	public List<int> items = new List<int>();
	public Node2D[] itemSlotSprites;
	
	public void DisplayInventorySlot(int slotNum){
		int inventorySize = numUniqueItems;
		if(itemSlotSprites[slotNum] == null){
				Node2D slotNode = (Node2D)itemSlotScene.Instantiate();
				Sprite2D slotSprite = (Sprite2D)slotNode.GetNode("Slot");
				// --------- DOESNT POSITION CORRECTLY ------------
				slotSprite.Position = new Vector2(
					0, 
					(slotNum - inventorySize / 2) * slotSprite.Texture.GetWidth() 
				);
				// ---------------------------------------------------------------------------------
			itemSlotSprites[slotNum] = slotNode;
			AddChild(itemSlotSprites[slotNum]);
		}

		UpdateItemSlot slotScript = (UpdateItemSlot)itemSlotSprites[slotNum];
		slotScript.UpdateAmount(items[slotNum]);
	}

	public void DisplayInventory(){
		for (int i = 0; i < numUniqueItems; i++){
			DisplayInventorySlot(i);
		}
	}

	public void InitializeInventory(){
		foreach (int itemIdx in Enum.GetValues<ItemType>()){
			if(itemIdx < 3){
				items.Add(startingSeedCount);
			} else {
				items.Add(0);
			}
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		itemSlotSprites = new Node2D[numUniqueItems];
		InitializeInventory();
		DisplayInventory();
	}

	public void AddItem(ItemType item, int amount){
		items[(int)item] += amount;
		UpdateItemSlot slotScript = (UpdateItemSlot)itemSlotSprites[(int)item];
		slotScript.UpdateAmount(items[(int)item]);
	}

	public void RemoveItem(ItemType item, int amount){
		if(items[(int)item] >= amount){
			items[(int)item] -= amount;
			UpdateItemSlot slotScript = (UpdateItemSlot)itemSlotSprites[(int)item];
			slotScript.UpdateAmount(items[(int)item]);
		}
		return;
	}

	public int GetItemAmnt(ItemType item){
		return items[(int)item];
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}
}
