using Godot;
using System;
using System.Collections.Generic;


public class ActionTracker 
{
    Stack<int[]> actions;
    Stack<int[]> redoActions;

    int seed;

    public ActionTracker() {
        // Load from memory first
        actions = new();
        redoActions = new();
        seed = (int)Time.GetTicksMsec();
        actions.Push(new int[]{-1, seed}); // -1 tells the loader what seed to use
    }

    public ActionTracker(int seed) {
        actions = new();
        redoActions = new();
        this.seed = seed;
        actions.Push(new int[]{-1, seed});
    }

    public int GetSeed() {
        return seed;
    }

    public void StepTime(int seed) {
        actions.Push(new int[]{0, seed});
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