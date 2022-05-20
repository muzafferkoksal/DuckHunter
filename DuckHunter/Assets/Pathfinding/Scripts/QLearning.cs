using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QLearning
{
    public int numOfActions;
    public int numOfStates;
    public double[] values;
    public List<Tuple<PathNode, PathNode, int, int, double>> transitions;
    public double gamma;
    private Grid<PathNode> grid;
    private PathNode startNode;
    private int height;
    private int width;
    private System.Random rnd;
    private ArrowCreator ac;
    private double lr;
    private double exp_rate;
    private int[] states;
    private List<double> qvalues;
    private PathNode currentNode;

    public QLearning(List<double> qvalues, int height, int width, List<Tuple<PathNode, PathNode, int, int, double>> transitions, double gamma, double exp_rate, Grid<PathNode> grid, double lr, PathNode startNode)
    {
        ac = GameObject.Find("ArrowCreator").GetComponent<ArrowCreator>();
        numOfActions = 4;
        numOfStates = height * width;
        Debug.Log("numOfStates:  " + numOfStates);
        values = new double[numOfStates];
        this.transitions = transitions;
        this.gamma = gamma;
        this.height = height;
        this.width = width;
        this.lr = lr;
        this.exp_rate = exp_rate;
        rnd = new System.Random();
        this.grid = grid;
        this.startNode = startNode;
        this.qvalues = qvalues;
        states = new int[numOfStates];
        for (int i = 0; i < numOfStates; i++) { 
            values[i] = 0;
            states[i] = 0;
        }
        currentNode = startNode;
    }

    public int ChooseAction(PathNode node, List<Tuple<PathNode, PathNode, int, int, double>> transitions)
    {
        double max_next_rew = 0;
        int action = -1;
        double rndm = rnd.Next(0, 10);
        if (rndm <= exp_rate * 10)
        {
            int act = rnd.Next(0, 4);
            action = act;
        }
        else
        {
            for (int a = 0; a < numOfActions; a++)
            {
                int index = FindTuplesFromList(transitions, node, a);

                if (index == -1)
                {
                    continue;
                }
                else
                {
                    double next_reward = qvalues[index];
                    if(next_reward >= max_next_rew)
                    {
                        action = a;
                        max_next_rew = next_reward;
                    }
                }
            }
        }
        return action;
    }

    public void Learn(double reward, int action, List<Tuple<PathNode, PathNode, int, int, double>> transitions, PathNode newNode, PathNode oldNode)
    {
        List<double> qtab = new List<double>();
        for (int i = 0; i < numOfActions; i++)
        {
            int indexx = FindTuplesFromList(transitions, newNode, i);
            if (indexx == -1)
                continue;
            else
                qtab.Add(qvalues[indexx]);
        }

        double maxx = 0;
        for (int i = 0; i < qtab.Count; i++)
        {
            if (qtab[i] >= maxx)
            {
                maxx = qtab[i]; // max q value of next state 
            }
        }

        int index = FindTuplesFromList(transitions, oldNode, action);
        if (index != -1){ 
            double currentQ = qvalues[index];
            qvalues[index] = (1 - lr) * currentQ + lr * (reward + gamma * maxx); 
        }
    }

    public List<double> Train()
    {
        int trials = 500;
        int maxStep = 1000;
        List<int> road = new List<int>();
        List<double> arr = new List<double>();
        for(int i = 0; i < trials; i++)
        {
            double cumRew = 0;
            int step = 0;
            bool game_over = false;
            road.Clear();
            currentNode = startNode;
            while(step < maxStep && game_over != true)
            {
                PathNode oldNode = currentNode;
                int action = ChooseAction(oldNode, transitions);
                FindNextTuple(transitions, oldNode, action);
                double reward = currentNode.GetProb();
                Debug.Log(reward);
                PathNode newNode = currentNode;
                road.Add(action);
                Learn(reward, action, transitions, newNode, oldNode);
                cumRew += reward;
                step++;
                Debug.Log("cum: " + cumRew);
                if (reward == 3.0)
                    game_over = true;
            }
            Debug.Log("-------------------------------------------");
            arr.Add(cumRew);
        }
        Debug.Log("Steps to Goal:");
        foreach (double item in road)
            Debug.Log(item);

        int[] drawArr = new int[height * width];
        for (int m = 0; m < height * width; m++)
            drawArr[m] = 5;
        int whereuare = 0;
        for (int k = 0; k < road.Count; k++)
        {
            int loc = LocationFinder(road[k], whereuare, height, width);
            //Debug.Log("loc: " + loc + " roadk: " + road[k]);
            drawArr[whereuare] = road[k];
            whereuare = loc;

        }
        ac.spawnArrows(drawArr);
        return arr;
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
            if (whereUAre + height <= height * width - 1)
                return whereUAre + height;
        }
        else if (toWhere == 2)
        {//up
            if (whereUAre + 1 <= height * width - 1)
                return whereUAre + 1;
        }
        else if (toWhere == 3)
        { //down
            if (whereUAre - 1 >= 0)
                return whereUAre - 1;
        }
        else if (toWhere == 5)
            return whereUAre;
        Debug.Log("To where " + toWhere);
        return -999;
    }



    private void FindNextTuple(List<Tuple<PathNode, PathNode, int, int, double>> transitions, PathNode pathNode, int act)
    {
        List<double> probs = new List<double>();
        List<Tuple<PathNode, PathNode, int, int, double>> trans = new List<Tuple<PathNode, PathNode, int, int, double>>();
        foreach (Tuple<PathNode, PathNode, int, int, double> tpl in transitions)
        {
            if (tpl.Item1.Equals(pathNode) && tpl.Item3.Equals(act))
            {
                probs.Add(tpl.Item5);
                trans.Add(tpl);
            }
        }

        double maxval = -999;
        for (int i = 0; i < probs.Count; i++)
            if (probs[i] >= maxval)
                maxval = probs[i];
        if (maxval == -999)
            return;
        int maxInd = probs.IndexOf(maxval);

        Tuple<PathNode, PathNode, int, int, double> tp = trans[maxInd];
        currentNode = tp.Item2;
    }

    //bulunduğun node - gideceğin node(real) - gitmek istediğin action - yapabildiğin action - olasılık
    private int FindTuplesFromList(List<Tuple<PathNode, PathNode, int, int, double>> transitions, PathNode pathNode, int act)
    {
        List<int> rslt = new List<int>();
        List<double> probs = new List<double>();
        foreach (Tuple<PathNode, PathNode, int, int, double> tpl in transitions)
        {
            if (tpl.Item1.Equals(pathNode) && tpl.Item3.Equals(act))
            {
                rslt.Add(transitions.IndexOf(tpl));
                probs.Add(tpl.Item5);
            }
        }
        if (rslt.Count == 0)
            return -1;

        double maxval = -999;
        for (int i = 0; i < probs.Count; i++)
            if (probs[i] >= maxval)
                maxval = probs[i];
        int maxInd = probs.IndexOf(maxval);
        return rslt[maxInd];
    }

    private PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }
}
