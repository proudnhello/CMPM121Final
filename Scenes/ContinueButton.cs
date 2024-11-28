using Godot;
using System;

public partial class ContinueButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_pressed(){
		if(FileAccess.FileExists("user://AutoSave.save")) {
			SceneSwitcher.Instance.StartSceneFromFile("AutoSave");
		}
		else {
			GD.Print("No save file found.");
		}
	}
}
