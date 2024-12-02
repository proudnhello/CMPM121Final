class_name PlantDatabase extends Node

###### INTERNAL DSL FOR ALL PLANTS

## Special Requirement Functions
static func lily(info) -> bool: 
    var cell = info[0];
    var sunRequirement;
    if (cell[3] == 1): sunRequirement = 2;
    elif (cell[3] == 2): sunRequirement = 4;
    else: sunRequirement = 8;

    if cell[1] < sunRequirement:
        return false
    return true

## Plant Types
static var Plants: Array = [
    {"plantName": "Lily", 
        "waterRequirement": 2, 
        "sunRequirement": 2, 
        "maxGrowthLevel": 3, 
        "minAdjPlants": 0, 
        "maxAdjPlants": 4, 
        "startSeeds": 3,
        "specialCheck": Callable(lily)
    },
    {"plantName": "Sunflower",
        "waterRequirement": 10,
        "sunRequirement": 2,
        "maxGrowthLevel": 3, 
        "minLikePlants": 1, 
        "maxLikePlants": 4, 
        "startSeeds": 3},  
    {"plantName": "Rafflesia",
        "waterRequirement": 2,
        "sunRequirement": 2,
        "maxGrowthLevel": 3,
        "minLikePlants": 4,
        "maxLikePlants": 10,
        "minAdjPlants": 4, 
        "maxAdjPlants": 10, 
        "startSeeds": 8}        
    ]
##############################

static var NumberOfPlantTypes: int = 3;

static func retreivePlants() -> Array:
    for plant in Plants:
        if (!plant.has("waterRequirement")): plant.waterRequirement = 0;
        if (!plant.has("sunRequirement")): plant.sunRequirement = 0;
        if (!plant.has("maxGrowthLevel")): plant.maxGrowthLevel = 3;
        if (!plant.has("minLikePlants")): plant.minLikePlants = 0;
        if (!plant.has("maxLikePlants")): plant.maxLikePlants = 8;
        if (!plant.has("minAdjPlants")): plant.minAdjPlants = 0;
        if (!plant.has("maxAdjPlants")): plant.maxAdjPlants = 8;
        if (!plant.has("startSeeds")): plant.startSeeds = 0;
        if (!plant.has("specialCheck")): plant.specialCheck = null;
    NumberOfPlantTypes = Plants.size();
    return Plants;


static func check_condition_requirements(cell) -> bool:
    var plant = retreivePlants()[cell[2]-1];

    if cell[0] < plant.waterRequirement or cell[1] < plant.sunRequirement or cell[3] >= plant.maxGrowthLevel:
        return false
    return true

static func check_neighbor_requirements(cell, like_neighbors, total_neighbors):
    var plant = retreivePlants()[cell[2]-1];

    if (total_neighbors < plant.minAdjPlants || total_neighbors > plant.maxAdjPlants ||
        like_neighbors < plant.minLikePlants || like_neighbors > plant.maxLikePlants):
            return false;
    return true;

static func check_special_requirements(cell, likePlants, adjacentPlants) -> bool:
    var plant = retreivePlants()[cell[2]-1];
    if (plant.specialCheck != null): return plant.specialCheck.call([cell, likePlants, adjacentPlants]);
    return true;

static func get_requirements(type) -> Dictionary:
    return retreivePlants()[type-1];
