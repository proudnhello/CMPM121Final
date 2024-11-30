extends Resource

@export var gridDimensions: int = 1
@export var maxWaterLevelIncrease: int
@export var maxSunLevel: int
@export var numberOfPlantTypes: int
@export var plantRequirements: Array[Resource] = []
@export var maxWater: int = 25

func get_plant_requirements(plant_type: int) -> Resource:
	print(plant_type)
	return plantRequirements[plant_type - 1]