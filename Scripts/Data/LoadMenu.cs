using Godot;
using System;

public partial class LoadMenu : MenuButton
{
	[Export] GridManager gridManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		MenuButton loadMenu = this;
		PopupMenu popupMenu = loadMenu.GetPopup();
		popupMenu.Connect("id_pressed", new Callable(this, nameof(LoadFromFile)));
	}

	private void LoadFromFile(int fileID)
	{
		// id is defined inside the Godot editor
		// click on SaveMenu -> Items to see or make changes to id 
		switch (fileID)
		{
			case 1:
				gridManager.Load("File1");
				break;
			case 2:
				gridManager.Load("File2");
				break;
			case 3:
				gridManager.Load("File3");
				break;
		}
	}
}
