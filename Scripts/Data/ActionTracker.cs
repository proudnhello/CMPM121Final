using Godot;
using System;
using System.Collections.Generic;


public class ActionTracker 
{
    Stack<int[]> actions;
    Stack<int[]> redoActions;

    int timeProgressCounter = 0;

    public ActionTracker() {
        // Load from memory first

        actions = new();
        redoActions = new();
    }

    public void StepTime() {
        actions.Push(new int[]{0, timeProgressCounter});
        GD.Seed((ulong)timeProgressCounter);
        timeProgressCounter++;
        redoActions.Clear();
        GD.Print(actions.Count);
    }

    public void PlantSeed(int x, int y, int plantType) {
        actions.Push(new int[]{1, x, y, plantType});
        redoActions.Clear();
        GD.Print(actions.Count);

    }

    public void HarvestPlant(int x, int y, int plantType) {
        actions.Push(new int[]{2, x, y, plantType});
        redoActions.Clear();
        GD.Print(actions.Count);
    }

    public int[] UndoAction() {
        if (actions.Count == 0) return null;
        int[] action = actions.Pop();
        GD.Print(action.Stringify());
        redoActions.Push(action);
        return action;
    }

    public int[] RedoAction() {
        if (redoActions.Count == 0) return null;
        int[] action = redoActions.Pop();
        GD.Print(action.Stringify());
        actions.Push(action);
        return action;
    }
}