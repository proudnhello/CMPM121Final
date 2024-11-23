using Godot;
using System;

public partial class UpdateItemSlot : Node2D {
   [Export] Label amountLabel; 

   public void UpdateAmount(int newAmount) {
	  amountLabel.Text = "" + newAmount;
   }   

}
