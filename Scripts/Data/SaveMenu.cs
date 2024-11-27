using Godot;
using System;
using System.Net;

public partial class SaveMenu : MenuButton
{
	[Export] GridManager gridManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MenuButton saveMenu = this;
		PopupMenu popupMenu = saveMenu.GetPopup();
		popupMenu.Connect("id_pressed", new Callable(this, nameof(SaveToFile)));
	}

	private void SaveToFile(int fileID)
	{
		// id is defined inside the Godot editor
		// click on SaveMenu -> Items to see or make changes to id 
		switch (fileID)
		{
			case 1:
				gridManager.Save("File1");
				break;
			case 2:
				gridManager.Save("File2");
				break;
			case 3:
				gridManager.Save("File3");
				break;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
