using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;
using System.Linq;

public class DuckCreator : MonoBehaviour {
    public GameObject duckPrefab;
    private PathfindingVisual pathfinding;

    public List<Duck> ducks { get; private set; }
    public int duck_count;
    public int row;
    public int column;
    private bool spawned = false;
    Grid<PathNode> grid;

    private void Start()
    {
        duck_count = GameObject.Find("Testing").GetComponent<Testing>().duck_count;
        row = GameObject.Find("Testing").GetComponent<Testing>().row;
        column = GameObject.Find("Testing").GetComponent<Testing>().column;
        pathfinding = GameObject.Find("PathfindingVisual").GetComponent<PathfindingVisual>();
        ducks = new List<Duck>();
        grid = pathfinding.grid;
    }
    private void Update(){
        if(!spawned){
            spawnDucks();
        }
        
    }
    public void spawnDucks()
    {
        spawned = true;
        PathNode node;
        int randy;
        int randx;
        bool canBE = false;
        for (int i = 0; i < duck_count; i++){
            System.Random rnd = new System.Random();
            print("asssssssssssssssssssssssssssssssss");
            do {
                randx = rnd.Next(0, row);
                randy = rnd.Next(0, column);
                if (grid == null) {
                    grid = pathfinding.grid;
                    continue;
                }
                node = grid.GetGridObject(randx, randy);
                canBE = node.GetCanBeFilled();
                if (canBE) { 
                    node.SetCanBeFilled(false);
                    node.SetProb(3.0);
                }
            } while (!canBE);
            GameObject duck = Instantiate(duckPrefab) as GameObject;
            Vector3 worldCoor = pathfinding.grid.GetWorldPosition(randx, randy);
            worldCoor.x += 5;
            worldCoor.y += 5; 
            duck.transform.position = worldCoor;
            int[] pos = {randx,randy};

            Duck d = new Duck(i, randx, randy, worldCoor, duck);
            ducks.Add(d);
        } 
    }

    public void removeDuck(GameObject duck){
        for(int i = 0; i < ducks.Count; i++){
            if(ducks[i].duckObj == duck){
                ducks.RemoveAt(i);
            }   
        }
        Destroy(duck);
         
    }

    public Duck getClosestDuck(Vector3 pos){
        float close = 1000;
        if(!ducks.Any()){
            return null;
        }
        Duck d = ducks[0];
        for(int i = 0; i < ducks.Count; i++){
           if(manhattanDistance(pos, ducks[i].worldCoor) < close){
               close = manhattanDistance(pos, ducks[i].worldCoor);
               d = ducks[i];
           }
        }
        if(close != 0)
        Debug.Log($"closest: {close}");
        return d;
    }

    public float manhattanDistance(Vector3 a, Vector3 b){
        return Math.Abs(a.x-b.x) + Math.Abs(a.y-b.y);
    }
    
}