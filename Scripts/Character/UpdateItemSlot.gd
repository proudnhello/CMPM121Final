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

func update_amount(new_amount: int) -> void:
    amount_label.text = str(new_amount)