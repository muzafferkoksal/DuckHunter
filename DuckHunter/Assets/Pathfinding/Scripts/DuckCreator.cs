using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;
using System.Linq;

public class DuckCreator : MonoBehaviour {
    /*
    public static int duckCount = 4;
    public GameObject duckPrefab;
    private PathfindingVisual pathfindingVisual;
    public Pathfinding pathfinding;
    public List<Duck> ducks { get; private set; } 
    private bool spawned = false;
    private bool[] onSameTile = new bool[duckCount];
    private int[] rand = new int[duckCount];
    public float spee = 0.05f;
    private bool[] walkingMiddle = new bool[duckCount];
    private int[] counter = new int[duckCount];
    */


    public static int duckCount;
    public int row;
    public int column;
    public GameObject duckPrefab;
    private PathfindingVisual pathfindingVisual;
    public Pathfinding pathfinding;
    public List<Duck> ducks { get; private set; }
    private bool spawned = false;
    private bool[] onSameTile;
    private int[] rand;
    public float spee = 0.05f;
    private bool[] walkingMiddle;
    private int[] counter;
    private int mode;
    Grid<PathNode> grid;

    private void Start()
    {
        /*
        pathfindingVisual = GameObject.Find("PathfindingVisual").GetComponent<PathfindingVisual>(); 
        //pathfinding = GameObject.Find("Pathfinding").GetComponent<Pathfinding>(); 
        ducks = new List<Duck>();
        for(int i = 0; i < duckCount; i++){
            onSameTile[i] = false;
            walkingMiddle[i] = false;
            counter[i] = -1;
        }
        */

        mode = GameObject.Find("Testing").GetComponent<Testing>().mode;
        duckCount = GameObject.Find("Testing").GetComponent<Testing>().duck_count;
        row = GameObject.Find("Testing").GetComponent<Testing>().row;
        column = GameObject.Find("Testing").GetComponent<Testing>().column;
        rand = new int[duckCount];
        onSameTile = new bool[duckCount];
        walkingMiddle = new bool[duckCount];
        counter = new int[duckCount];
        pathfindingVisual = GameObject.Find("PathfindingVisual").GetComponent<PathfindingVisual>();
        grid = pathfindingVisual.grid;
        //pathfinding = GameObject.Find("Pathfinding").GetComponent<Pathfinding>(); 
        ducks = new List<Duck>();
        for (int i = 0; i < duckCount; i++)
        {
            onSameTile[i] = false;
            walkingMiddle[i] = false;
            counter[i] = -1;
        }

    }

    private void Update(){
        if(!spawned){
            spawnDucks();
        }
        
        else if(ducks.Any()){
            
            for (int i = 0; ducks.Any() && i < ducks.Count; i++){
                    var d  = (ducks[i].duckObj).GetComponent<Duck>();
                    System.Random rnd = null;
                    

                    if(!onSameTile[i]){
                        rnd = new System.Random();
                        rand[i] = rnd.Next(0,4);
                        onSameTile[i] = true;
                        //Debug.Log("on same tile");
                    }
                    if(walkingMiddle[i]){
                        counter[i] = (int)(10 / spee) / 2;
                        walkingMiddle[i] = false;
                    }
                    
                    int oldX= 0, oldY = 0; 
                    pathfindingVisual.grid.GetXY(new Vector3(d.worldCoor.x  , d.worldCoor.y , d.worldCoor.z), out oldX, out oldY);
                    int newX = 0, newY = 0;

                    if(rand[i] == 0){  //right
                        if(counter[i] > 0){
                            counter[i]--;
                        }
                        if(counter[i] == 0){
                            onSameTile[i] = false;
                            counter[i] = -1;
                        }   
                        if((d.worldCoor + new Vector3(spee,0,0)).x < pathfindingVisual.grid.GetWidth()*10 - 5){
                            d.worldCoor = d.worldCoor + new Vector3(spee,0,0);
                            d.direction = "right";
                        }    
                        else{
                            onSameTile[i] = false;
                        }      
                    }
                    else if(rand[i] == 1){ //left
                        if(counter[i] > 0){
                            counter[i]--; 
                        }
                        if(counter[i] == 0){
                            onSameTile[i] = false;
                            counter[i] = -1;
                        }
                        if((d.worldCoor + new Vector3(-spee,0,0)).x > 5){
                            d.worldCoor = d.worldCoor + new Vector3(-spee,0,0);
                            d.direction = "left";
                        }
                        else{
                            onSameTile[i] = false;
                        }    
                    }
                    else if(rand[i] == 2){ //top
                        if(counter[i] > 0){
                            counter[i]--;
                        }
                        if(counter[i] == 0){
                            onSameTile[i] = false;
                            counter[i] = -1;
                        }
                        if((d.worldCoor + new Vector3(0,spee,0)).y < pathfindingVisual.grid.GetHeight()*10 - 5){
                            d.worldCoor = d.worldCoor + new Vector3(0,spee,0);
                            d.direction = "top";
                        }      
                        else{
                            onSameTile[i] = false;
                        } 
                    }
                    else if(rand[i] == 3){ //down
                        if(counter[i] > 0){
                            counter[i]--;
                        }
                        if(counter[i] == 0){
                            onSameTile[i] = false;
                            counter[i] = -1;
                        }
                        if((d.worldCoor + new Vector3(0,spee,0)).y > 5){
                            d.worldCoor = d.worldCoor + new Vector3(0,-spee,0);
                            d.direction = "down";
                        }
                        else{
                            onSameTile[i] = false;
                        } 
                    }
                    pathfindingVisual.grid.GetXY(new Vector3(d.worldCoor.x , d.worldCoor.y , d.worldCoor.z), out newX, out newY);
                        
                    if(oldX != newX || oldY != newY){
                        d.x = newX;
                        d.y = newY;
                        ducks[i].x = newX;
                        ducks[i].y = newY;
                        walkingMiddle[i] = true;

                    }

                    d.transform.position = d.worldCoor;
                    ducks[i].worldCoor = d.worldCoor;
                    ducks[i].duckObj = d.duckObj;
            }
        }
    }

    /*
    public void spawnDucks(){

        spawned = true;
        for(int i = 0; i < duckCount; i++){
            System.Random rnd = new System.Random();
            int randx = rnd.Next(0,pathfindingVisual.grid.GetWidth());
            int randy = rnd.Next(0, pathfindingVisual.grid.GetHeight());
            GameObject duck = Instantiate(duckPrefab) as GameObject;
            Vector3 worldCoor = pathfindingVisual.grid.GetWorldPosition(randx, randy);
            worldCoor.x += 5;
            worldCoor.y += 5; 
            duck.transform.position = worldCoor;
            int[] pos = {randx,randy};

            duck.GetComponent<Duck>().x = randx;
            duck.GetComponent<Duck>().y = randy;
            duck.GetComponent<Duck>().id = i;
            duck.GetComponent<Duck>().worldCoor = worldCoor;
            duck.GetComponent<Duck>().duckObj = duck;

            Duck d = new Duck(i, randx, randy, worldCoor, duck, "down");
            ducks.Add(d);
        } 
    }
    */
    public void spawnDucks()
    {
        if (mode == 1 || mode == 5)
        {
            spawned = true;
            PathNode node;
            int randy;
            int randx;
            bool canBE = false;
            for (int i = 0; i < duckCount; i++)
            {
                System.Random rnd = new System.Random();
                print("asssssssssssssssssssssssssssssssss");
                do
                {
                    randx = rnd.Next(0, row);
                    randy = rnd.Next(0, column);
                    if (grid == null)
                    {
                        grid = pathfindingVisual.grid;
                        continue;
                    }
                    Debug.Log("asdssssssssssssssssss " + row + " " + column);
                    Debug.Log("asdssssssssssssssssss " + randx + " " + randy);
                    node = grid.GetGridObject(randx, randy);
                    try
                    {
                        Debug.Log("asdssssssssssssssssss " + node.canBeFilled);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                    canBE = node.canBeFilled;
                    if (canBE)
                    {
                        node.SetCanBeFilled(false);
                        node.SetProb(3.0);
                    }
                } while (!canBE);
                GameObject duck = Instantiate(duckPrefab) as GameObject;
                Vector3 worldCoor = pathfindingVisual.grid.GetWorldPosition(randx, randy);
                worldCoor.x += 5;
                worldCoor.y += 5;
                duck.GetComponent<Duck>().x = randx;
                duck.GetComponent<Duck>().y = randy;
                duck.GetComponent<Duck>().id = i;
                duck.GetComponent<Duck>().worldCoor = worldCoor;
                duck.GetComponent<Duck>().duckObj = duck;
                duck.transform.position = worldCoor;
                int[] pos = { randx, randy };
                Duck d = new Duck(i, randx, randy, worldCoor, duck, "right");
                ducks.Add(d);
            }
        }
        else
        {
            spawned = true;
            for (int i = 0; i < duckCount; i++)
            {
                System.Random rnd = new System.Random();
                int randx = rnd.Next(0, 20);
                int randy = rnd.Next(0, 30);
                GameObject duck = Instantiate(duckPrefab) as GameObject;
                Vector3 worldCoor = pathfindingVisual.grid.GetWorldPosition(randx, randy);
                worldCoor.x += 5;
                worldCoor.y += 5;
                duck.transform.position = worldCoor;
                int[] pos = { randx, randy };

                duck.GetComponent<Duck>().x = randx;
                duck.GetComponent<Duck>().y = randy;
                duck.GetComponent<Duck>().id = i;
                duck.GetComponent<Duck>().worldCoor = worldCoor;
                duck.GetComponent<Duck>().duckObj = duck;

                Duck d = new Duck(i, randx, randy, worldCoor, duck, "down");
                ducks.Add(d);
            }
        }
    }

    public void removeDuck(GameObject duck){
        GameObject d = null;
        int index = -1;
        for(int i = 0; ducks.Any() && i < ducks.Count; i++){
            if(ducks[i].duckObj == duck){
                index = i;
                d =ducks[i].duckObj;
            }   
        }
        
       ducks.RemoveAt(index); 
       Destroy(d);     
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
        //if(close != 0)
       //Debug.Log($"closest: {close}");
        return d;
    }

    public float manhattanDistance(Vector3 a, Vector3 b){
        return Math.Abs(a.x-b.x) + Math.Abs(a.y-b.y);
    }

    public void moveDuck(GameObject duck, int dire)
    {
        int distan = (int)(10 / spee) / 2;
        Vector3 v = new Vector3(0, 0, 0);
        int index = 0;
        for (int i = 0; i < ducks.Count(); i++)
        {
            if (ducks[i].duckObj == duck)
            {
                index = i;
            }
        }
        var d = (ducks[index].duckObj).GetComponent<Duck>();

        if (dire == 0)
        {
            if ((d.worldCoor + new Vector3(0, 10, 0)).y < 299 - 5)
            {
                d.worldCoor = d.worldCoor + new Vector3(0, 10, 0);
                d.direction = "top";
                d.y += 1;
                ducks[index].y = d.y;
            }
            
        }
        if (dire == 1)
        {
            if ((d.worldCoor + new Vector3(0, -10, 0)).y > 5)
            {
                d.worldCoor = d.worldCoor + new Vector3(0, -10, 0);
                d.direction = "down";
                d.y -= 1;
                ducks[index].y = d.y;
            }
                
        }
        if (dire == 2)
        {
            if ((d.worldCoor + new Vector3(10, 0, 0)).x < 199 - 5)
            {
                d.worldCoor = d.worldCoor + new Vector3(10, 0, 0);
                d.direction = "right";
                d.x += 1;
                ducks[index].x = d.x;
            }
            
        }
        if (dire == 3)
        {
            if ((d.worldCoor + new Vector3(-10, 0, 0)).x > 5)
            {
                d.worldCoor = d.worldCoor + new Vector3(-10, 0, 0);
                d.direction = "left";
                d.x -= 1;
                ducks[index].x = d.x;
            }
            
        }
        d.transform.position = d.worldCoor;

        ducks[index].worldCoor = d.worldCoor;

    }

}