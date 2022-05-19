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
using System;

public class Pathfinding {

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    private Grid<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    private PathNode duckpos;
    private PathNode characterPosition;
    int turn;

    public Pathfinding(int width, int height) {
        Instance = this;
        grid = new Grid<PathNode>(width, height, 10f, Vector3.zero, (Grid<PathNode> g, int x, int y) => new PathNode(g, x, y));
        duckpos = GetNode(0, 5);
        Debug.Log(duckpos);
        characterPosition = GetNode(0, 0);
        turn = 0;
    }

    public Grid<PathNode> GetGrid() {
        return grid;
    }

    public List<Vector3> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition) {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);
        Debug.Log("yass");

        List<PathNode> path = FindPath(startX, startY, endX, endY);
        if (path == null) {
            return null;
        } else {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path) {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY) {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);
        Debug.Log("we are in a*!");
        Debug.Log(startNode.ToString());

        if (startNode == null || endNode == null) {
            // Invalid Path
            return null;
        }

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < grid.GetWidth(); x++) {
            for (int y = 0; y < grid.GetHeight(); y++) {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = 99999999;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();
        
        PathfindingDebugStepVisual.Instance.ClearSnapshots();
        PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, startNode, openList, closedList);

        while (openList.Count > 0) {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode) {
                // Reached final node
                PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, currentNode, openList, closedList);
                PathfindingDebugStepVisual.Instance.TakeSnapshotFinalPath(grid, CalculatePath(endNode));
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if (closedList.Contains(neighbourNode)) continue;
                if (!neighbourNode.isWalkable) {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode)) {
                        openList.Add(neighbourNode);
                    }
                }
                PathfindingDebugStepVisual.Instance.TakeSnapshot(grid, currentNode, openList, closedList);
            }
        }

        // Out of nodes on the openList
        return null;
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbourList = new List<PathNode>();
        if (currentNode.x + 1 < grid.GetWidth())
        {
            // Right
            neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y));
            /*
            // Right Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            // Right Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
            */
        }

        if (currentNode.x - 1 >= 0) {
            // Left
            neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y));
            /*
            // Left Down
            if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            // Left Up
            if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
            */
        }
       
        // Up
        if (currentNode.y + 1 < grid.GetHeight()) neighbourList.Add(GetNode(currentNode.x, currentNode.y + 1));
        // Down
        if (currentNode.y - 1 >= 0) neighbourList.Add(GetNode(currentNode.x, currentNode.y - 1));

        return neighbourList;
    }

    public double GetRewardFunction(PathNode state)
    {
        return state.GetProb();
    }

    public bool[] CheckLeftRight(int mode, PathNode currentNode)
    {
        bool[] arr = new bool[2];
        if (mode == 0)
        {
            PathNode rightNode = GetNode(currentNode.x + 1, currentNode.y);
            PathNode leftNode = GetNode(currentNode.x - 1, currentNode.y);
            if (rightNode != null && leftNode != null)
            {
                bool right = rightNode.isWalkable;
                bool left = leftNode.isWalkable;
                arr[0] = right;
                arr[1] = left;
            }
        } 
        else
        {
            PathNode topNode = GetNode(currentNode.x, currentNode.y + 1);
            PathNode bottomNode = GetNode(currentNode.x, currentNode.y - 1);
            if (topNode != null && bottomNode != null)
            {
                bool top = topNode.isWalkable;
                bool bottom = bottomNode.isWalkable;
                arr[0] = top;
                arr[1] = bottom;
            }
        }
        return arr;
    }

    public bool[] CheckOut(int mode, PathNode currentNode)
    {
        bool[] arr = new bool[2];
        if (mode == 0)
        {
            PathNode rightNode = GetNode(currentNode.x + 1, currentNode.y);
            PathNode leftNode = GetNode(currentNode.x - 1, currentNode.y);
            if (rightNode != null && leftNode != null)
            {
                bool right = rightNode.isWalkable;
                bool left = leftNode.isWalkable;
                arr[0] = right;
                arr[1] = left;
            }
            else
            {
                arr[0] = true;
                arr[1] = true;
                if (currentNode.x + 1 >= grid.GetWidth())
                {
                    arr[0] = false;
                }
                if (currentNode.x - 1 < 0)
                {
                    arr[1] = false;
                }
            }
        }
        else
        {
            PathNode topNode = GetNode(currentNode.x, currentNode.y + 1);
            PathNode bottomNode = GetNode(currentNode.x, currentNode.y - 1);
            if (topNode != null && bottomNode != null)
            {
                bool top = topNode.isWalkable;
                bool bottom = bottomNode.isWalkable;
                arr[0] = top;
                arr[1] = bottom;
            } else {
                arr[0] = true;
                arr[1] = true;
                if (currentNode.y + 1 >= grid.GetHeight())
                {
                    arr[0] = false;
                } 
                if (currentNode.y - 1< 0)
                {
                    arr[1] = false;
                }
            }
        }
        return arr;
    }

    public List<Tuple<PathNode, PathNode, int, int, double>> GetTransitionFunction(PathNode currentNode) // 0 left 1 right 2 up 3 down 5 no move
    {
        double randomRate = 0.2;
        List<Tuple<PathNode, PathNode, int, int, double>> transitions = new List<Tuple<PathNode, PathNode, int, int, double>>();
        for(int r = 0; r < grid.GetWidth(); r++)
        {
            for(int c = 0; c < grid.GetHeight(); c++)
            {
                Debug.Log("r " + currentNode.x + " c " + currentNode.y);
                currentNode = GetNode(0 + r, 0 + c);
                
                if (currentNode.isWalkable)
                {
                    List<Tuple<PathNode, int>> neighbourList = new List<Tuple<PathNode, int>>();
                    List<Tuple<PathNode, int>> negList = new List<Tuple<PathNode, int>>();

                    if (currentNode.x + 1 < grid.GetWidth())
                    {// Right
                        neighbourList.Add(new Tuple<PathNode, int>(GetNode(currentNode.x + 1, currentNode.y), 1));
                    }
                    else { 
                        negList.Add(new Tuple<PathNode, int>(GetNode(currentNode.x + 1, currentNode.y), 1));
                    }
                    if (currentNode.x - 1 >= 0)
                    {// Left
                        neighbourList.Add(new Tuple<PathNode, int>(GetNode(currentNode.x - 1, currentNode.y), 0));
                    }
                    else 
                    {
                        negList.Add(new Tuple<PathNode, int>(GetNode(currentNode.x - 1, currentNode.y), 0));
                    }
                    if (currentNode.y + 1 < grid.GetHeight())
                    { // Up
                        neighbourList.Add(new Tuple<PathNode, int>(GetNode(currentNode.x, currentNode.y + 1), 2));
                    }
                    else {
                        negList.Add(new Tuple<PathNode, int>(GetNode(currentNode.x, currentNode.y + 1), 2));
                    }
                    if (currentNode.y - 1 >= 0)
                    {// Down
                        neighbourList.Add(new Tuple<PathNode, int>(GetNode(currentNode.x, currentNode.y - 1), 3));
                    }
                    else { 
                        negList.Add(new Tuple<PathNode, int>(GetNode(currentNode.x, currentNode.y - 1), 3));
                    }

                    int index = 0;
                    foreach (Tuple<PathNode, int> neighbourNode in neighbourList)
                    {
                        PathNode nb = neighbourNode.Item1;
                        int pos = neighbourNode.Item2;
                        int toPos = pos;


                        if (!nb.isWalkable)
                        {
                            if (pos == 1 || pos == 0)
                            {
                                bool[] arr = CheckLeftRight(1, currentNode);
                                if (!arr[0] && !arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1.0);
                                    transitions.Add(wallTuple);
                                }
                                else if (!arr[0] && arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y - 1), toPos, 3, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                }
                                else if (arr[0] && !arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y + 1), toPos, 2, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                }
                                else
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y - 1), toPos, 3, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y + 1), toPos, 2, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                    transitions.Add(wallTuple3);
                                }
                            }
                            else
                            {
                                bool[] arr = CheckLeftRight(0, currentNode);
                                if (!arr[0] && !arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1.0);
                                    transitions.Add(wallTuple);
                                }
                                else if (!arr[0] && arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x - 1, currentNode.y), toPos, 0, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                }
                                else if (arr[0] && !arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x + 1, currentNode.y), toPos, 1, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                }
                                else
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x - 1, currentNode.y), toPos, 0, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x + 1, currentNode.y), toPos, 1, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                    transitions.Add(wallTuple3);
                                }
                            }
                        }
                        else
                        {
                            if (pos == 1 || pos == 0)
                            {
                                bool[] arr = CheckLeftRight(1, currentNode);
                                if (!arr[0] && !arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, nb, toPos, pos, 1 - randomRate);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                }
                                else if (!arr[0] && arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, nb, toPos, pos, 1 - randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y - 1), toPos, 3, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                    transitions.Add(wallTuple3);
                                }
                                else if (arr[0] && !arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y + 1), toPos, 2, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, nb, toPos, pos, 1 - randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                    transitions.Add(wallTuple3);
                                }
                                else
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, nb, toPos, pos, 1 - randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y - 1), toPos, 3, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y + 1), toPos, 2, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                    transitions.Add(wallTuple3);
                                }
                            }
                            else
                            {
                                bool[] arr = CheckLeftRight(0, currentNode);
                                if (!arr[0] && !arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, nb, toPos, pos, 1 - randomRate);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                }
                                else if (!arr[0] && arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, nb, toPos, pos, 1 - randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x - 1, currentNode.y), toPos, 0, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                    transitions.Add(wallTuple3);
                                }
                                else if (arr[0] && !arr[1])
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x + 1, currentNode.y), toPos, 1, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, nb, toPos, pos, 1 - randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                    transitions.Add(wallTuple3);
                                }
                                else
                                {
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, nb, toPos, pos, 1 - randomRate);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x - 1, currentNode.y), toPos, 0, randomRate / 2);
                                    Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x + 1, currentNode.y), toPos, 1, randomRate / 2);
                                    transitions.Add(wallTuple);
                                    transitions.Add(wallTuple2);
                                    transitions.Add(wallTuple3);
                                }
                            }
                        }
                        index++;
                    }

                    index = 0;
                    foreach (Tuple<PathNode, int> neighbourNode in negList)
                    {
                        PathNode nb = neighbourNode.Item1;
                        int pos = neighbourNode.Item2;
                        int toPos = pos;

                        if (pos == 1 || pos == 0)
                        {
                            bool[] arr = CheckOut(1, currentNode);
                            if (!arr[0] && !arr[1])
                            {
                                Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1.0);
                                transitions.Add(wallTuple);
                            }
                            else if (!arr[0] && arr[1])
                            {
                                Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate / 2);
                                Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y - 1), toPos, 3, randomRate / 2);
                                transitions.Add(wallTuple);
                                transitions.Add(wallTuple2);
                            }
                            else if (arr[0] && !arr[1])
                            {
                                Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y + 1), toPos, 2, randomRate / 2);
                                Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate / 2);
                                transitions.Add(wallTuple);
                                transitions.Add(wallTuple2);
                            }
                            else
                            {
                                Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate);
                                Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y - 1), toPos, 3, randomRate / 2);
                                Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x, currentNode.y + 1), toPos, 2, randomRate / 2);
                                transitions.Add(wallTuple);
                                transitions.Add(wallTuple2);
                                transitions.Add(wallTuple3);
                            }
                        }
                        else
                        {
                            bool[] arr = CheckOut(0, currentNode);
                            if (!arr[0] && !arr[1])
                            {
                                Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1.0);
                                transitions.Add(wallTuple);
                            }
                            else if (!arr[0] && arr[1])
                            {
                                Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate / 2);
                                Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x - 1, currentNode.y), toPos, 0, randomRate / 2);
                                transitions.Add(wallTuple);
                                transitions.Add(wallTuple2);
                            }
                            else if (arr[0] && !arr[1])
                            {
                                Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x + 1, currentNode.y), toPos, 1, randomRate / 2);
                                Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate / 2);
                                transitions.Add(wallTuple);
                                transitions.Add(wallTuple2);
                            }
                            else
                            {
                                Tuple<PathNode, PathNode, int, int, double> wallTuple = new Tuple<PathNode, PathNode, int, int, double>(currentNode, currentNode, toPos, 5, 1 - randomRate);
                                Tuple<PathNode, PathNode, int, int, double> wallTuple2 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x - 1, currentNode.y), toPos, 0, randomRate / 2);
                                Tuple<PathNode, PathNode, int, int, double> wallTuple3 = new Tuple<PathNode, PathNode, int, int, double>(currentNode, GetNode(currentNode.x + 1, currentNode.y), toPos, 1, randomRate / 2);
                                transitions.Add(wallTuple);
                                transitions.Add(wallTuple2);
                                transitions.Add(wallTuple3);
                            }

                        }
                        index++;
                    }
                }
            }
        }
        return transitions;
    }

    public void MDP(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);
        PathNode endNode = grid.GetGridObject(endX, endY);
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode asd = grid.GetGridObject(3, 1);
        endNode.SetProb(3.0);
        asd.SetProb(-5.0);
        List<Tuple<PathNode, PathNode, int, int, double>> transitions = GetTransitionFunction(startNode);
        PolicyIteration poly = new PolicyIteration(grid.GetHeight(), grid.GetWidth(), transitions, 0.5, null, grid, startNode);
        poly.train();
    }

    public PathNode GetNode(int x, int y) {
        return grid.GetGridObject(x, y);
    }

    private List<PathNode> CalculatePath(PathNode endNode) {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null) {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();
        return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b) {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList) {
        PathNode lowestFCostNode = pathNodeList[0];
        for (int i = 1; i < pathNodeList.Count; i++) {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost) {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }

    public List<Vector3> SearchDeep(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        Debug.Log("SEARCHING IN THE DEEP");
        List<PathNode> path = SearchDeep(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    public List<PathNode> SearchDeep(int startX, int startY, int endX, int endY)
    {
        Debug.Log("SEARCHING IN THE DEEP");
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null)
        {
            // Invalid Path
            return null;
        }

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.cameFromNode = null;
            }
        }

        openList = new List<PathNode>();
        closedList = new List<PathNode>();
        PathNode currentNode = startNode;
        closedList.Add(startNode);
        
        foreach (PathNode neighbourNode in GetNeighbourList(startNode)){
            List<PathNode> path = SearchDeep(neighbourNode, endNode, 0);
            if (path != null)
                return path;
        }
        return null;
    }
    public List<PathNode> SearchDeep(PathNode node, PathNode endNode, int n)
    {    
        //Debug.Log("SEARCHING IN THE DEEP");
        bool flag = false;

        if(node == endNode)
        {
            Debug.Log("Found the correct path");
            return CalculatePath(endNode);
        }

        openList.Add(node);
        foreach (PathNode neighbourNode in GetNeighbourList(node))
        {
            //openList.Remove(node);
            if (!openList.Contains(neighbourNode) && neighbourNode.isWalkable && !closedList.Contains(neighbourNode))
            {
                flag = true;
                neighbourNode.cameFromNode = node;
                
                openList.Add(neighbourNode);
                Debug.Log("Moving on to " + neighbourNode + "n = " + n );
                return SearchDeep(neighbourNode, endNode, ++n);
            }
            else if(!closedList.Contains(neighbourNode) && !neighbourNode.isWalkable)
            {
                closedList.Add(neighbourNode);
            }
        }
        if (!flag)
        {
            
            closedList.Add(node);
            Debug.Log("No go on this tile");
        }
          
        return null;
    }


    public List<Vector3> BreadthFirstSearch(Vector3 startWorldPosition, Vector3 endWorldPosition)
    {
        grid.GetXY(startWorldPosition, out int startX, out int startY);
        grid.GetXY(endWorldPosition, out int endX, out int endY);

        Debug.Log("BREADTH FIRST SEARCH");
        List<PathNode> path = BreadthFirstSearch(startX, startY, endX, endY);
        if (path == null)
        {
            return null;
        }
        else
        {
            List<Vector3> vectorPath = new List<Vector3>();
            foreach (PathNode pathNode in path)
            {
                vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
            }
            return vectorPath;
        }
    }

    public List<PathNode> BreadthFirstSearch(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null)
        {
            // Invalid Path
            return null;
        }

        openList = new List<PathNode>();
        closedList = new List<PathNode>();
        PathNode currentNode = startNode;
        closedList.Add(startNode);

        bool[,] vis = new bool[grid.GetWidth(), grid.GetHeight()];

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.cameFromNode = null;
            }
        }

        int n = 0;
        openList.Add(startNode);
        while(openList.Count != 0   )
        {
            if(openList[0] == endNode)
            {
                return CalculatePath(endNode);
            }
            currentNode = openList[0];
            openList.RemoveAt(0);
            closedList.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode))
            {
                if( neighbourNode.isWalkable && !openList.Contains(neighbourNode) && !closedList.Contains(neighbourNode))
                {
                    openList.Add(neighbourNode);
                    neighbourNode.cameFromNode = currentNode;
                    
                }
            }
            n++;
            Debug.Log(n);
        }
        return null;
    }

    private int ManhattanDistance(PathNode a, PathNode b)
    {
        int xDistance = Mathf.Abs(a.x - b.x);
        int yDistance = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDistance - yDistance);
        return xDistance + yDistance;
    }

    public int NearestWallDistance(PathNode start)
    {
        int dist = int.MaxValue;
        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                if (i != start.x && j != start.y && !grid.GetGridObject(i, j).isWalkable)
                {
                    int temp = ManhattanDistance(start, grid.GetGridObject(i, j));
                    dist = Math.Min(temp, dist);
                    //
                }

            }
        }
        return dist;
    }


    public int MiniMax(bool isMax, PathNode initNode, PathNode currNode, int height, int timer)
    {
        PathNode temp = currNode;
        if (duckpos == characterPosition)
        {
            return getScore();
        }
        if (height == 10)
        {
            return getScore();
        }
        /*
        if (isNoDucks())
        {
            Debug.Log("problem");
            return getScore();
        }
        */
        //TIMER HAS ENDED
        if (timer == 0)
        {
            return -1000;
        }

        if (isMax)
        {
            List<int> possibleMoves = new List<int>();
            foreach (PathNode neighbour in GetNeighbourList(currNode))
            {
                currNode = neighbour;
                possibleMoves.Add(MiniMax(!isMax, initNode, currNode, height + 1, timer - 1));
                currNode = temp;
            }
            int max = -1000;
            Debug.Log("possible moves for max are " + possibleMoves + " /THE END");
            foreach (int value in possibleMoves)
            {
                if (value > max)
                {
                    max = value;
                }
            }
            return max;
        }
        else
        {
            //return Math.Min(MiniMax(depth + 1, nodeIndex * 2, true, scores, h), MiniMax(depth + 1, nodeIndex * 2 + 1, true, scores, h));
            List<int> possibleMoves = new List<int>();
            foreach (PathNode neighbour in GetNeighbourList(currNode))
            {
                currNode = neighbour;
                possibleMoves.Add(MiniMax(isMax, initNode, currNode, height + 1, timer - 1));
                currNode = temp;
            }
            int min = 5000;
            //Debug.Log("possible moves for min are " + possibleMoves + " /THE END");
            foreach (int value in possibleMoves)
            {
                if (value < min)
                {
                    min = value;
                }
            }
            return min;
        }
    }

    public int getScore()
    {
        /*
        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                if (grid.GetGridObject(i, j).hasDuck) {
                    Debug.Log("score = " + 100 / ManhattanDistance(characterPosition, grid.GetGridObject(i, j)));
                    return 100 / ManhattanDistance(characterPosition, grid.GetGridObject(i, j));
                }
                    
            }
        }
        */
        //Debug.Log("score = " + 100 / ManhattanDistance(characterPosition, duckpos));
        return 100 / ManhattanDistance(characterPosition, duckpos);
    }

    public bool isNoDucks()
    {
        //1-hunter won |||-1 -duck won  ||| 0-not end
        bool flag = true;
        for (int i = 0; i < grid.GetWidth(); i++)
        {
            for (int j = 0; j < grid.GetHeight(); j++)
            {
                if (grid.GetGridObject(i, j).hasDuck)
                    flag = false;
            }
        }
        return flag;
    }

    public class Pair<T1, T2>
    {
        public T1 First { get; set; }
        public T2 Second { get; set; }
    }
    public List<Vector3> findBestMove()
    {

        if (turn == 0 || turn == 1)
        {
            //turn of the hunter
            PathNode temp = characterPosition;
            Pair<PathNode, int> bestNode = new Pair<PathNode, int>();
            bestNode.First = null;
            bestNode.Second = -10000;

            foreach (PathNode neighbour in GetNeighbourList(characterPosition))
            {
                characterPosition = neighbour;
                int moveval = MiniMax(true, neighbour, characterPosition, 1, 15);
                if (moveval > bestNode.Second)
                {
                    bestNode.First = neighbour;
                    bestNode.Second = moveval;
                }
                characterPosition = temp;
            }

            bestNode.First.cameFromNode = characterPosition;

            //HAHAHA
            turn++;
            List<PathNode> path = new List<PathNode>();
            path.Add(bestNode.First);
            if (path == null)
            {
                return null;
            }
            else
            {
                List<Vector3> vectorPath = new List<Vector3>();

                foreach (PathNode pathNode in path)
                {
                    vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
                }
                Debug.Log("Player's turn");
                Debug.Log(path[0]);
                characterPosition = path[0];
                return vectorPath;
            }
        }
        else if (turn == 2)
        {
            PathNode temp = duckpos;
            Pair<PathNode, int> bestNode = new Pair<PathNode, int>();
            bestNode.First = null;
            bestNode.Second = +10000;

            foreach (PathNode neighbour in GetNeighbourList(duckpos))
            {
                duckpos = neighbour;
                int moveval = MiniMax(false, neighbour, neighbour, 1, 15);
                if (moveval < bestNode.Second)
                {
                    bestNode.First = neighbour;
                    bestNode.Second = moveval;
                }
                duckpos = temp;
            }
            Debug.Log("debug1 bestnode first = " + bestNode.First);

            bestNode.First.cameFromNode = duckpos;
            Debug.Log("debug 2");

            //HAHAHA

            List<PathNode> path = new List<PathNode>();
            path.Add(bestNode.First);
            turn = 0;
            if (path == null)
            {
                return null;
            }
            else
            {
                List<Vector3> vectorPath = new List<Vector3>();

                foreach (PathNode pathNode in path)
                {
                    vectorPath.Add(new Vector3(pathNode.x, pathNode.y) * grid.GetCellSize() + Vector3.one * grid.GetCellSize() * .5f);
                }
                Debug.Log("Duck's turn");
                Debug.Log(path[0]);
                duckpos = path[0];
                //return vectorPath;
                return null;
            }
        }
        return null;

    }



}