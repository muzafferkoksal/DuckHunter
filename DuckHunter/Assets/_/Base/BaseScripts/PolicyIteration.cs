using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PolicyIteration
{
    public int numOfActions;
    public int numOfStates;
    public double[] values;
    public List<Tuple<PathNode, PathNode, int, int, double>> transitions;
    public double gamma;
    public int[] policy;
    private Grid<PathNode> grid;
    private PathNode startNode;
    private int height;
    private int width;
    private System.Random rnd;
    

    public PolicyIteration(int height, int width, List<Tuple<PathNode, PathNode, int, int, double>> transitions, double gamma, int[] policy, Grid<PathNode> grid, PathNode startNode)
    {
        numOfActions = 4;
        numOfStates = height * width;
        values = new double[numOfStates];
        this.transitions = transitions;
        this.gamma = gamma;
        this.height = height;
        this.width = width;
        rnd = new System.Random();
        if (this.policy == null)
        {
            this.policy = new int[numOfStates];
            for (int i = 0; i < numOfStates; i++) {
                this.policy[i] = 0;//rnd.Next(0, 4);
                //Debug.Log(this.policy[i]);
            }
        }
        else
            this.policy = policy;
        this.grid = grid;
        this.startNode = startNode;
        for(int i = 0; i < numOfStates; i++)
            values[i] = 0;
    }

    public double OnePolicyEval()
    {
        double delta = 0;
        int i = 0;
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode node = GetNode(startNode.x + x, startNode.y + y);
                //Debug.Log("startNode.x: " + startNode.x + " startNode.y:" + startNode.y);
                double temp = values[i];
                //Debug.Log("x,y "+ x + "  " + y + " i " + i);
                int a = policy[i];
                List<Tuple<PathNode, PathNode, int, int, double>> tuples = FindTuplesFromList(transitions, node, a);

                //foreach (Tuple<PathNode, PathNode, int, int, double>  tpl in tuples)
                //{
                //    Debug.Log(tpl.Item3 + " - "  + tpl.Item4 + " -  " + tpl.Item5);
                //}
                if (tuples.Count == 0)
                {
                    Debug.Log("Continue!!");
                    i++;
                    continue;
                } else if(tuples.Count == 1) {
                    Debug.Log("1 count!!");
                    values[i] = node.GetProb() + gamma * (tuples[0].Item5 * values[i]);
                }
                else if(tuples.Count == 2) {
                    Debug.Log("2 count!! "+ tuples[1].Item4);
                    //Debug.Log("Loc " + LocationFinder(tuples[1].Item4, i, height, width));
                    double asd1 = values[LocationFinder(tuples[1].Item4, i, height, width)];
                    values[i] = node.GetProb() + gamma * (tuples[0].Item5 * values[LocationFinder(tuples[0].Item4, i, height, width)] + tuples[1].Item5 * values[LocationFinder(tuples[1].Item4, i, height, width)]);
                } else if (tuples.Count == 3) {
                    //Debug.Log("3 count!! " + LocationFinder(tuples[1].Item4, i, height, width));
                    double asd1 = values[LocationFinder(tuples[1].Item4, i, height, width)];
                    values[i] = node.GetProb() + gamma * (tuples[0].Item5 * values[LocationFinder(tuples[0].Item4, i, height, width)] + tuples[1].Item5 * values[LocationFinder(tuples[1].Item4, i, height, width)] + tuples[2].Item5 * values[LocationFinder(tuples[2].Item4, i, height, width)]);
                } else
                {
                    Debug.Log("Bu ne amk!!!");
                    foreach (Tuple<PathNode, PathNode, int, int, double> tpl in tuples)
                    {
                        Debug.Log(tpl.Item3 + " - " + tpl.Item4 + " -  " + tpl.Item5 + " ----- " + i);
                    }
                    values[i] = node.GetProb() + gamma * (tuples[0].Item5 * values[LocationFinder(tuples[0].Item4, i, height, width)] + tuples[1].Item5 * values[LocationFinder(tuples[1].Item4, i, height, width)] + tuples[2].Item5 * values[LocationFinder(tuples[2].Item4, i, height, width)] + tuples[3].Item5 * values[LocationFinder(tuples[3].Item4, i, height, width)]);
                }
                delta = Math.Max(delta, Math.Abs(temp - values[i]));
                i++;
            } 
        }
        return delta;
    }

    public int RunPolicyEvaluation()
    {
        int epoch = 0;
        double tol = 0.003;
        double delta = OnePolicyEval();
        List<double> deltaHistory = new List<double>();
        deltaHistory.Add(delta);
        while(epoch < 500)
        {
            epoch++;
            //Debug.Log("Epoch " + epoch + "--------------------------------------------------------------");
            delta = OnePolicyEval();
            deltaHistory.Add(delta);
            //Debug.Log("asdsadsadsadsadas" + delta);
            //if (delta < tol)
            //    break;
        }
        return deltaHistory.Count;
    }

    public int RunPolicyImprovement()
    {
        int updatePolicyCount = 0;
        int i = 0;
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode node = GetNode(startNode.x + x, startNode.y + y);
                int temp = policy[i];
                double[] vlist = new double[numOfActions];
                for (int k = 0; k < numOfActions; k++)
                    vlist[k] = 0;
                for(int a = 0; a < numOfActions; a++)
                {
                    List<Tuple<PathNode, PathNode, int, int, double>> tuples = FindTuplesFromList(transitions, node, a);

                    //foreach (Tuple<PathNode, PathNode, int, int, double> tpl in tuples)
                    //{
                    //    Debug.Log(tpl.Item3 + " - " + tpl.Item4 + " -  " + tpl.Item5);
                    //}

                    if (tuples.Count == 0) { 
                        continue; //TODO
                    }
                    else if (tuples.Count == 1) { 
                        vlist[a] = (tuples[0].Item5 * values[i]);
                    }
                    else if (tuples.Count == 2) {
                        //Debug.Log("2Loc1 " + LocationFinder(tuples[0].Item4, i, height, width));
                        //Debug.Log("2Loc2 " + LocationFinder(tuples[1].Item4, i, height, width));
                        vlist[a] = (tuples[0].Item5 * values[LocationFinder(tuples[0].Item4, i, height, width)] + tuples[1].Item5 * values[LocationFinder(tuples[1].Item4, i, height, width)]);
                    }else if (tuples.Count == 3)
                    {
                        //Debug.Log("3Loc0 " + LocationFinder(tuples[0].Item4, i, height, width));
                        //Debug.Log("3Loc1 " + LocationFinder(tuples[1].Item4, i, height, width));
                        //Debug.Log("3Loc2 " + LocationFinder(tuples[2].Item4, i, height, width));
                        vlist[a] = (tuples[0].Item5 * values[LocationFinder(tuples[0].Item4, i, height, width)] + tuples[1].Item5 * values[LocationFinder(tuples[1].Item4, i, height, width)] + tuples[2].Item5 * values[LocationFinder(tuples[2].Item4, i, height, width)]);
                    } else
                    {
                        vlist[a] = (tuples[0].Item5 * values[LocationFinder(tuples[0].Item4, i, height, width)] + tuples[1].Item5 * values[LocationFinder(tuples[1].Item4, i, height, width)] + tuples[2].Item5 * values[LocationFinder(tuples[2].Item4, i, height, width)] + tuples[3].Item5 * values[LocationFinder(tuples[3].Item4, i, height, width)]);
                    }
                }
                double maxx = vlist[0];
                int maxxInd = 0;
                for (int l = 0; l < numOfActions; l++)
                    if (vlist[l] > maxx)
                    {
                        maxx = vlist[l];
                        maxxInd = l;
                    }
                policy[i] = maxxInd;
                if (temp != policy[i])
                    updatePolicyCount++;
                i++;
            }
        }
        return updatePolicyCount;
    }

    public void train()
    {
        int epoch = 0;
        int evalCount = RunPolicyEvaluation();
        List<int> evalCountHistory = new List<int>();
        evalCountHistory.Add(evalCount);
        int policyChange = RunPolicyImprovement();
        List<int> policyChangeHistory = new List<int>();
        policyChangeHistory.Add(policyChange);
        while(epoch < 500)
        {
            epoch++;
            int newEvalCount = RunPolicyEvaluation();
            int newPolicyChange = RunPolicyImprovement();
            evalCountHistory.Add(newEvalCount);
            policyChangeHistory.Add(newPolicyChange);
            if (newPolicyChange == 0)
                break;
        }
        Debug.Log("epoch: " + epoch);
        Debug.Log("eval count: " + evalCountHistory.Count);
        Debug.Log("policy change: " + policyChangeHistory.Count);
        int o = 0;
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                string str = "";
                if (policy[o] == 1)
                    str = "Right";
                else if (policy[o] == 0)
                    str = "Left";
                else if (policy[o] == 2)
                    str = "up";
                else if (policy[o] == 3)
                    str = "down";
                Debug.Log("X: "+ x + " Y: " + y + " Policy: " + policy[o] + " Yön: " + str);
                o++;
            }
        }

        foreach (double elem in values)
            Debug.Log(elem);


    }

    //bulunduğun node - gideceğin node(real) - gitmek istediğin action - yapabildiğin action - olasılık
    private List<Tuple<PathNode, PathNode, int, int, double>> FindTuplesFromList(List<Tuple<PathNode, PathNode, int, int, double>> transitions, PathNode pathNode, int act)
    {
        List<Tuple<PathNode, PathNode, int, int, double>> tpls = new List<Tuple<PathNode, PathNode, int, int, double>>();
        foreach (Tuple<PathNode, PathNode, int, int, double> tpl in transitions)
        {
            if (tpl.Item1.Equals(pathNode) && tpl.Item3.Equals(act))
            {
                tpls.Add(tpl);
            }
        }
        return tpls;
    }

    private PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    private int LocationFinder(int toWhere, int whereUAre, int height, int width)
    {
        //Debug.Log("Where to " + toWhere + " whre u are " + whereUAre);
        if (toWhere == 0)
        {//left
            if (whereUAre - height >= 0)
                return whereUAre - height;
        }
        else if (toWhere == 1)
        {//right
            if (whereUAre + height <= 11)
                return whereUAre + height;
        }
        else if (toWhere == 2)
        {//up
            if (whereUAre + 1 <= 11)
                return whereUAre + 1;
        }
        else if (toWhere == 3)
        { //down
            if (whereUAre - 1 >= 0)
                return whereUAre - 1;
        }
        else if (toWhere == 5)
            return whereUAre;
        return -999;
    }
}