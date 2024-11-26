using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public class ActionTracker 
{
    Stack<int[]> actions;
    Stack<int[]> redoActions;

    int seed;
    int programCounter;

    public ActionTracker() {
        // Load from memory first
        actions = new();
        redoActions = new();
        seed = (int)Time.GetTicksMsec();
        // We use 0 to represent no seed, so we need to make sure the seed is not 0
        if (seed == 0) seed = 1;
        actions.Push(new int[]{-1, seed}); // -1 tells the loader what seed to use
        programCounter = 0;
    }

    public ActionTracker(int seed) {
        actions = new();
        redoActions = new();
        this.seed = seed;
        actions.Push(new int[]{-1, seed});
        programCounter = 0;
    }

    public int GetSeed() {
        return seed;
    }

    public int GetNextSeed() {
        int stepSeed = seed + programCounter;
        programCounter++;
        return stepSeed;
    }

    public void StepTime(int stepSeed, int[] grownPlants) {
        int[] typeSeed = new int[]{0, stepSeed};
        if(grownPlants.Length != 0) {
            GD.Print("Grown plants: " + grownPlants.Stringify());
        }
        actions.Push(typeSeed.Concat(grownPlants).ToArray());
        redoActions.Clear();
    }

    public void UnStepTime() {
        programCounter--;
    }

    public void PlantSeed(int x, int y, int plantType) {
        actions.Push(new int[]{1, x, y, plantType});
        redoActions.Clear();

    }

    public void HarvestPlant(int x, int y, int plantType) {
        actions.Push(new int[]{2, x, y, plantType});
        redoActions.Clear();
    }

    public int[] UndoAction() {
        // For save purposes, action 0 will contain the seed, so we need to make sure we don't undo that
        if (actions.Count <= 1) return null;
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