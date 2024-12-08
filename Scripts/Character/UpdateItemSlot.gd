'''
using Godot;
using System;

public partial class UpdateItemSlot : Node2D {
   [Export] Label amountLabel; 

   public void UpdateAmount(int newAmount) {
	  amountLabel.Text = "" + newAmount;
   }   

}

'''

extends Node2D

@export var amount_label: Label
@export var selected_border: CanvasItem;

func update_amount(new_amount: int, is_plant: bool) -> void:
    amount_label.text = str(new_amount);
    if (is_plant): amount_label.text += "/" + str(GameData.itemData["game settings"]["min plants"])

func update_selection(isSelected: bool):
   selected_border.visible = isSelected;
      