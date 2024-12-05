# Translated GDScript code

extends Node

@export var itemSlotScene : PackedScene
@export	var pixelsBetweenSlots: float = 215


# Our DSL for plant types and growth requirements

var PlantInfo: Array;

var items : Array
var itemSlotSprites : Array

func WinGame():
	get_tree().change_scene_to_file("res://Scenes/Victory.tscn")

func CheckWin():
	print( GameData.itemData["game settings"]["min plants"])
	for i in range(PlantInfo.size()):
		if (items[i + PlantInfo.size()]) < GameData.itemData["game settings"]["min plants"]: return
	WinGame()

func DisplayInventorySlot(slotNum):		
	var slotScript = itemSlotSprites[slotNum]
	slotScript.update_amount(items[slotNum])

func DisplayInventory():
	if itemSlotScene == null:
		return
	for i in range(items.size()):
		DisplayInventorySlot(i)

func InitializeInventory():
	for item in PlantInfo:
		items.append(item.startSeeds)
	for i in range(PlantInfo.size()):
		items.append(0)
	if (itemSlotScene == null): return;
		

func ResetInventory():
	items.clear()
	InitializeInventory()
	DisplayInventory()

func _on_harvest_plant_signal(plantType, number):
	print("Harvesting plant")
	var item = plantType + PlantInfo.size()
	AddItem(item, number)
	CheckWin()

func _on_plant_seed_signal(plantType, number):
	print("Planting seed")
	var item = plantType
	AddItem(item, number)
	CheckWin()

func _ready():
	PlantInfo = PlantDatabase.retreivePlants();
	if (itemSlotScene == null): return;
	items = []
	itemSlotSprites = []
	for i in range(PlantInfo.size()*2):
		var slotNode = itemSlotScene.instantiate() as Node2D
		var slotSprite = slotNode.get_node("Slot") as Sprite2D
		slotNode.position = Vector2(0, (i - items.size() / 2.0) * slotSprite.texture.get_width() + i * pixelsBetweenSlots)
		itemSlotSprites.append(slotNode);
		add_child(itemSlotSprites[i])
	InitializeInventory()
	DisplayInventory()

func AddItem(item, amount):
	items[item] += amount
	var slotScript = itemSlotSprites[item]
	slotScript.update_amount(items[item])

func RemoveItem(item, amount):
	if items[item] >= amount:
		items[item] -= amount
		var slotScript = itemSlotSprites[item]
		slotScript.update_amount(items[item])
		return true
	return false

func GetItemAmnt(item):
	return items[item]
