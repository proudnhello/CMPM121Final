'''
using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public partial class Inventory : Node
{

	public static Inventory instance;

	[Export] public int numUniqueItems = 6;
	[Export] PackedScene itemSlotScene;
	[Export] public int numStartSeed1 = 3;

	[Export] public int numStartSeed2 = 3;

	[Export] public int numStartSeed3 = 8;

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

	public void WinGame() {
		GetTree().ChangeSceneToFile("res://Scenes/Victory.tscn");
	}

	public void checkWin() {
		if (items[(int)ItemType.PLANT1] >= 3 && items[(int)ItemType.PLANT2] >= 3 && items[(int)ItemType.PLANT3] >= 3) {
			WinGame();
		}
	}
	
	public void DisplayInventorySlot(int slotNum){
		int pixelsBetweenSlots = 215;
		int inventorySize = numUniqueItems;
		if(itemSlotSprites[slotNum] == null){
				Node2D slotNode = (Node2D)itemSlotScene.Instantiate();
				Sprite2D slotSprite = (Sprite2D)slotNode.GetNode("Slot");
				slotNode.Position = new Vector2(0, (slotNum - inventorySize / 2) * slotSprite.Texture.GetWidth() + slotNum * pixelsBetweenSlots);
			itemSlotSprites[slotNum] = slotNode;
			AddChild(itemSlotSprites[slotNum]);
		}

		Node2D slotScript = itemSlotSprites[slotNum];
		slotScript.Call("update_amount", items[slotNum]);
	}

	public void DisplayInventory(){
		for (int i = 0; i < numUniqueItems; i++){
			DisplayInventorySlot(i);
		}
	}

	public void InitializeInventory(){
		items.Add(numStartSeed1);
		items.Add(numStartSeed2);
		items.Add(numStartSeed3);
		for(int i = 0; i < 3; i++) {
			items.Add(0);
		}
	}

	public void ResetInventory(){
		items.Clear();
		InitializeInventory();
		DisplayInventory();
	}

	public void _on_harvest_plant_signal(int plantType, int number){
		GD.Print("Harvesting plant");
		ItemType item = (ItemType)plantType + 3;
		AddItem(item, number);
		checkWin();
	}

	public void _on_plant_seed_signal(int plantType, int number){
		GD.Print("Planting seed");
		ItemType item = (ItemType)plantType;
		AddItem(item, number);
		checkWin();
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		instance = this;
		itemSlotSprites = new Node2D[numUniqueItems];
		InitializeInventory();
		DisplayInventory();
		
	}

	public void AddItem(ItemType item, int amount){
		items[(int)item] += amount;
		Node2D slotScript = itemSlotSprites[(int)item];
		slotScript.Call("update_amount", items[(int)item]);
	}

	public bool RemoveItem(ItemType item, int amount){
		if(items[(int)item] >= amount){
			items[(int)item] -= amount;
			Node2D slotScript = itemSlotSprites[(int)item];
		slotScript.Call("update_amount", items[(int)item]);
			return true;
		}
		return false;
	}

	public int GetItemAmnt(ItemType item){
		return items[(int)item];
	}

	public void LoadFromMemory(int[][] actions) {
		
	}
}

'''

# Translated GDScript code

extends Node

@export var numUniqueItems : int = 6
@export var itemSlotScene : PackedScene = null
@export var numStartSeed1 : int = 3
@export var numStartSeed2 : int = 3
@export var numStartSeed3 : int = 8

enum ItemType {
    SEED1,
    SEED2,
    SEED3, 
    PLANT1,
    PLANT2,
    PLANT3
}

var items : Array = []
var itemSlotSprites : Array = []

func WinGame():
    get_tree().change_scene("res://Scenes/Victory.tscn")

func CheckWin():
    if items[ItemType.PLANT1] >= 3 and items[ItemType.PLANT2] >= 3 and items[ItemType.PLANT3] >= 3:
        WinGame()

func DisplayInventorySlot(slotNum):
    var pixelsBetweenSlots = 215
    var inventorySize = numUniqueItems
    if itemSlotSprites[slotNum] == null:
        var slotNode = itemSlotScene.instantiate() as Node2D
        var slotSprite = slotNode.get_node("Slot") as Sprite2D
        slotNode.position = Vector2(0, (slotNum - inventorySize / 2.0) * slotSprite.texture.get_width() + slotNum * pixelsBetweenSlots)
        itemSlotSprites[slotNum] = slotNode
        add_child(itemSlotSprites[slotNum])

    var slotScript = itemSlotSprites[slotNum]
    slotScript.update_amount(items[slotNum])

func DisplayInventory():
    if itemSlotScene == null:
        return
    for i in range(numUniqueItems):
        DisplayInventorySlot(i)

func InitializeInventory():
    items.append(numStartSeed1)
    items.append(numStartSeed2)
    items.append(numStartSeed3)
    for i in range(3):
        items.append(0)

func ResetInventory():
    items.clear()
    InitializeInventory()
    DisplayInventory()

func _on_harvest_plant_signal(plantType, number):
    print("Harvesting plant")
    var item = plantType + 3
    AddItem(item, number)
    CheckWin()

func _on_plant_seed_signal(plantType, number):
    print("Planting seed")
    var item = plantType
    AddItem(item, number)
    CheckWin()

func _ready():
    print(itemSlotScene)
    itemSlotSprites = []
    for i in range(numUniqueItems):
        itemSlotSprites.append(null)
    InitializeInventory()
    DisplayInventory()

func AddItem(item, amount):
    items[item] += amount
    var slotScript = itemSlotSprites[item]
    slotScript.call("update_amount", items[item])

func RemoveItem(item, amount):
    if items[item] >= amount:
        items[item] -= amount
        var slotScript = itemSlotSprites[item]
        slotScript.call("update_amount", items[item])
        return true
    return false

func GetItemAmnt(item):
    return items[item]
