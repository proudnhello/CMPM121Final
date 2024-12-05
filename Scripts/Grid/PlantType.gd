extends Resource

@export var plantName: String
@export var startingSeedCount: int

@export var water_requirement: int
@export var sun_requirement: int
@export var max_growth_level: int
@export var min_like_plants: int
@export var max_like_plants: int
@export var min_adj_plants: int
@export var max_adj_plants: int

func check_simple_requirements(cell):
    if cell[0] < water_requirement or cell[1] < sun_requirement or cell[3] >= max_growth_level:
        return false
    return true