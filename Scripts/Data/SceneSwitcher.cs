using Godot;
using System;
using System.IO; 

public partial class SceneSwitcher : Node
{

	public static SceneSwitcher Instance { get; private set; }

	public string filepath = null;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	public void StartSceneFromScratch() {
		this.filepath = null;
		string absolutePath = ProjectSettings.GlobalizePath("user://AutoSave.save");
		try
        {
            File.Delete(absolutePath);
		} catch (IOException ex)
        {
            GD.PrintErr($"Failed to delete file: {ex.Message}");
        }
		GetTree().ChangeSceneToFile("res://Scenes/Gameplay.tscn");
	}


	public void StartSceneFromFile(string filepath) {
		this.filepath = filepath;
		GetTree().ChangeSceneToFile("res://Scenes/Gameplay.tscn");
	}
}
