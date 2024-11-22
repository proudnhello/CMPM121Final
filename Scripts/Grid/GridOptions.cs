using Godot;

public partial class GridOptions : Resource {
	[Export(PropertyHint.Range, "1, 200")] public int gridDimensions;
	[Export] public int maxWaterLevelIncrease;
	[Export] public int maxSunLevel;
}