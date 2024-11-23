using Godot;

public partial class GridOptions : Resource {
	[Export(PropertyHint.Range, "1, 200")] public int gridDimensions;
	[Export] public int maxWaterLevelIncrease;
	[Export] public int maxSunLevel;

	[Export] public int numberOfPlantTypes;
	[Export] PlantGrowthRequirement[] plantRequirements;

	// Because we made 0 represent no plant, we need to subtract 1 from the plant type to get the correct index
	// In hindsight, this was stupid
	public PlantGrowthRequirement GetPlantRequirements(int plantType) {
		return plantRequirements[plantType - 1];
	}
}