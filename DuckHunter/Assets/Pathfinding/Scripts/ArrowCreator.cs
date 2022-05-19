using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCreator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite up;
    public Sprite down;
    public Sprite left;
    public Sprite right;
    public GameObject arrowPrefab;
    public int row;
    public int column;
    private PathfindingVisual pathfinding;
    private bool spawned = false;
    private Grid<PathNode> grid;
    private void Start()
    {
        row = GameObject.Find("Testing").GetComponent<Testing>().row;
        column = GameObject.Find("Testing").GetComponent<Testing>().column;
        pathfinding = GameObject.Find("PathfindingVisual").GetComponent<PathfindingVisual>();
        grid = pathfinding.grid;
    }

    private void Update()
    {

    }
    public void spawnArrows(int[] arr)
    {
        spawned = true;
        int a = 0;
        for (int i = 0; i < row; i++)
        {
            for(int j = 0; j < column; j++)
            {
                PathNode pathNode = grid.GetGridObject(i, j);
                if (!pathNode.isWalkable || !pathNode.GetCanBeFilled())
                    continue;
                //System.Random rnd = new System.Random();
                //int randx = rnd.Next(0, row);
                //int randy = rnd.Next(0, column);
                GameObject arrow = Instantiate(arrowPrefab) as GameObject;
                Vector3 worldCoor = pathfinding.grid.GetWorldPosition(i, j);
                worldCoor.x += 5;
                worldCoor.y += 5;
                arrow.transform.position = worldCoor;
                spriteRenderer = arrow.GetComponent<SpriteRenderer>();
                if (arr[a] == 2)
                {
                    spriteRenderer.sprite = up;
                }
                else if (arr[a] == 3)
                {
                    spriteRenderer.sprite = down;
                }
                else if (arr[a] == 0)
                {
                    spriteRenderer.sprite = left;
                }
                else if (arr[a] == 1)
                {
                    spriteRenderer.sprite = right;
                }
                a++;
            }
        }
    }
}
