class_name InputManager extends Node

@export var character: Character;
var seedActionSelected: int;

func select_seed(seedIndex: int):
    seedActionSelected = seedIndex;

func press_plant():
    print(seedActionSelected);
    character.plant_seed(seedActionSelected)

func press_harvest():
    character.harvest_plant()

func press_up(): character.move_position(0, -1)
func press_left(): character.move_position(-1, 0)
func press_down(): character.move_position(0, 1)
func press_right(): character.move_position(1, 0)