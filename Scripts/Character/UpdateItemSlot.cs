using Godot;
using System;

public partial class UpdateItemSlot : Node2D {
   [ExportAttribute] Label amount; 

   public void UpdateAmount(int newAmount) {
      amount.Text = "" + newAmount;
   }   
}