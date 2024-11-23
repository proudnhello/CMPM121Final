using Godot;

[GlobalClass]
public partial class PlantGrowthRequirement : Resource
{
    [Export] public int waterRequirement;
    [Export] public int sunRequirement;
    [Export] public int maxGrowthLevel;
    [Export] public int minLikePlants;
    [Export] public int maxLikePlants;
    [Export] public int minAdjPlants;
    [Export] public int maxAdjPlants;

    public bool checkSimpleRequirements(Cell cell) {
        if (cell.waterLevel < waterRequirement || cell.sunLevel < sunRequirement || cell.plantLevel >= maxGrowthLevel) {
            return false;
        }
        return true;
    }
}