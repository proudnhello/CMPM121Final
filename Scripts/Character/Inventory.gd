# Translated GDScript code

extends Node

@export var itemSlotScene : PackedScene
@export	var pixelsBetweenSlots: float = 215
@export var inputManager: InputManager


# Our DSL for plant types and growth requirements

var PlantInfo: Array;

var items : Array
var itemSlotSprites : Array

func WinGame():
	get_tree().change_scene_to_file("res://Scenes/Victory.tscn")

func CheckWin():
	for i in range(PlantInfo.size()):
		if (items[i + PlantInfo.size()]) < GameData.itemData["game settings"]["min plants"]: return
	WinGame()

func DisplayInventorySlot(slotNum):		
	var slotScript = itemSlotSprites[slotNum]
	slotScript.update_amount(items[slotNum], slotNum >= PlantInfo.size())
	slotScript.update_selection(slotNum == currentSeed);

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
	var item = plantType + PlantInfo.size()
	AddItem(item, number)
	CheckWin()

func _on_plant_seed_signal(plantType, number):
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
		if (i < PlantInfo.size()): 
			slotNode.position = Vector2(0, (i - items.size() / 2.0) * slotSprite.texture.get_width() + i * pixelsBetweenSlots)
			slotSprite.self_modulate = Color.hex(0x5b3400ff)
			var touchButton = slotNode.get_node("Button") as Button
			touchButton.button_down.connect(SelectSeed.bind(i))
		else:
			var effectiveIndex = i - PlantInfo.size()
			slotNode.position = Vector2(slotSprite.texture.get_width() + pixelsBetweenSlots, (effectiveIndex - items.size() / 2) * slotSprite.texture.get_width() + effectiveIndex * pixelsBetweenSlots)
		itemSlotSprites.append(slotNode);
		add_child(itemSlotSprites[i])
	InitializeInventory()
	SelectSeed(0)
	DisplayInventory()

func AddItem(item, amount):
	items[item] += amount
	DisplayInventorySlot(item)

func RemoveItem(item, amount):
	if items[item] >= amount:
		items[item] -= amount
		DisplayInventorySlot(item)
		return true
	return false

func GetItemAmnt(item):
	return items[item]

var currentSeed: int = 0;
func SelectSeed(index: int):
	print("selected seed ", index)
	inputManager.select_seed(index+1);
	currentSeed = index;
	DisplayInventory()