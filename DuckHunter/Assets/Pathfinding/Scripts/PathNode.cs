/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {

    private Grid<PathNode> grid;
    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public bool isWalkable;
    public PathNode cameFromNode;
    public double prob;
    public double util;
    public bool hasPlayer;
    public bool hasDuck;
    public bool canBeFilled;

    public PathNode(Grid<PathNode> grid, int x, int y) {
        this.grid = grid;
        this.x = x;
        this.y = y;
        prob = -0.4;
        util = 0;
        isWalkable = true;
        hasPlayer = false;
        hasDuck = false;
        canBeFilled = true;
        if(x == 0 && y == 0)
            canBeFilled = false;        
    }

    public void CalculateFCost() {
        fCost = gCost + hCost;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.isWalkable = isWalkable;
        if (this.isWalkable == false)
            canBeFilled = false;
        else
            canBeFilled = true;
        grid.TriggerGridObjectChanged(x, y);
    }

    public override string ToString() {
        return x + "," + y;
    }

    public void SetProb(double newProb)
    {
        prob = newProb;
    }

    public bool GetCanBeFilled()
    {
        return canBeFilled;
    }

    public void SetCanBeFilled(bool newb)
    {
        canBeFilled = newb;
    }

    public double GetProb()
    {
        return prob;
    }
}
