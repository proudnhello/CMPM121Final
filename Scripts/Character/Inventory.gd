# Translated GDScript code

extends Node

@export var numUniqueItems : int = 6
@export var itemSlotScene : PackedScene = null
@export var numStartSeed1 : int = 3
@export var numStartSeed2 : int = 3
@export var numStartSeed3 : int = 8

# Our DSL for plant types and growth requirements

var PlantTypes: Array = []

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
	get_tree().change_scene_to_file("res://Scenes/Victory.tscn")

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
	for i in range(PlantTypes.size()):
		items.append(PlantTypes[i]["startSeeds"])
	for i in range(PlantTypes.size()):
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
	PlantTypes.append({
		"plantName": "Plant1", 
		"waterRequirement": 2, 
		"sunRequirement": 8, 
		"maxGrowthLevel": 3, 
		"minLikePlants": 0, 
		"maxLikePlants": 4,
		"minAdjPlants": 0, 
		"maxAdjPlants": 4, 
		"startSeeds": 3})

	PlantTypes.append({
		"plantName": "Plant2", 
		"waterRequirement": 10, 
		"sunRequirement": 2, 
		"maxGrowthLevel": 3, 
		"minLikePlants": 0, 
		"maxLikePlants": 2, 
		"minAdjPlants": 0, 
		"maxAdjPlants": 2, 
		"startSeeds": 3})

	PlantTypes.append({
		"plantName": "Plant3", 
		"waterRequirement": 2, 
		"sunRequirement": 2, 
		"maxGrowthLevel": 3, 
		"minLikePlants": 4, 
		"maxLikePlants": 10, 
		"minAdjPlants": 4, 
		"maxAdjPlants": 10, 
		"startSeeds": 8})

	itemSlotSprites = []
	for i in range(numUniqueItems):
		itemSlotSprites.append(null)
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
