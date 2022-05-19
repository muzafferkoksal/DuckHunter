using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;
using System;
using System.Linq;

public class BombCreator : MonoBehaviour
{
    public GameObject bombPrefab;
    private PathfindingVisual pathfinding;

    public List<Bomb> bombs { get; private set; }
    public int bomb_count;
    public int row;
    public int column;
    private bool spawned = false;
    Grid<PathNode> grid;

    private void Start()
    {
        bomb_count = GameObject.Find("Testing").GetComponent<Testing>().bomb_count;
        row = GameObject.Find("Testing").GetComponent<Testing>().row;
        column = GameObject.Find("Testing").GetComponent<Testing>().column;
        pathfinding = GameObject.Find("PathfindingVisual").GetComponent<PathfindingVisual>();
        bombs = new List<Bomb>();
        grid = pathfinding.grid;
    }
    private void Update()
    {
        if (!spawned)
        {
            spawnBombs();
        }

    }
    public void spawnBombs()
    {
        spawned = true;
        PathNode node;
        int randx;
        int randy;
        bool canBE = false;
        for (int i = 0; i < bomb_count; i++)
        {
            System.Random rnd = new System.Random();
            do
            {
                randx = rnd.Next(0, row);
                randy = rnd.Next(0, column);
                if (grid == null)
                {
                    grid = pathfinding.grid;
                    continue;
                }
                node = grid.GetGridObject(randx, randy);
                canBE = node.GetCanBeFilled();
                if (canBE) { 
                    node.SetCanBeFilled(false);
                    node.SetProb(-5.0);
                }
            } while (!canBE);
            GameObject bomb = Instantiate(bombPrefab) as GameObject;
            Vector3 worldCoor = pathfinding.grid.GetWorldPosition(randx, randy);
            worldCoor.x += 5;
            worldCoor.y += 5;
            bomb.transform.position = worldCoor;
            int[] pos = { randx, randy };

            Bomb d = new Bomb(i, randx, randy, worldCoor, bomb);
            bombs.Add(d);
        }
    }

    public void removeBomb(GameObject bomb)
    {
        for (int i = 0; i < bombs.Count; i++)
        {
            if (bombs[i].bombObj == bomb)
            {
                bombs.RemoveAt(i);
            }
        }
        Destroy(bomb);

    }

    public Bomb getClosestBomb(Vector3 pos)
    {
        float close = 1000;
        if (!bombs.Any())
        {
            return null;
        }
        Bomb d = bombs[0];
        for (int i = 0; i < bombs.Count; i++)
        {
            if (manhattanDistance(pos, bombs[i].worldCoor) < close)
            {
                close = manhattanDistance(pos, bombs[i].worldCoor);
                d = bombs[i];
            }
        }
        if (close != 0)
            Debug.Log($"closest: {close}");
        return d;
    }

    public float manhattanDistance(Vector3 a, Vector3 b)
    {
        return Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
    }

}