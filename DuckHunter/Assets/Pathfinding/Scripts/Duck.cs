using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class Duck : MonoBehaviour {
    public float speed = 0;
    public int id;
    public int x;
    public int y;

    public string direction;

    public Vector3 worldCoor; 
    public GameObject duckObj;
    private int randomState; 
    private PathfindingVisual pathfindingVisual;

    private Pathfinding pathfinding;
    private int spee = 1;
    public SpriteRenderer spriteRenderer { get; private set; }
    //public Sprite[] sprites = new Sprite[0];
    public float animationTime = 0.25f;
    public int animationFrame { get; private set; }
    public bool loop = true;

    public Sprite[] up = new Sprite[0];
    public Sprite[] down = new Sprite[0];
    public Sprite[] left = new Sprite[0];
    public Sprite[] right = new Sprite[0];

    public Duck(int id, int x, int y, Vector3 worldCoor, GameObject duckObj, string direction){
        this.id = id;
        this.x = x;
        this.y = y;
        this.worldCoor = worldCoor;
        this.duckObj = duckObj;
        this.direction = direction;
    }
    private void Start()
    {
        pathfindingVisual = GameObject.Find("PathfindingVisual").GetComponent<PathfindingVisual>(); 
        InvokeRepeating(nameof(Advance), animationTime, animationTime);
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Update(){
        
    }
    private void Advance()
    {
        if (!spriteRenderer.enabled) {
            return;
        }

        animationFrame++;

        if (animationFrame >= up.Length && loop) {
            animationFrame = 0;
        }

        if (animationFrame >= 0 && animationFrame < up.Length) {
            if (direction == "top") {
                spriteRenderer.sprite = up[animationFrame];
            }
            else if (direction == "down") {
                spriteRenderer.sprite = down[animationFrame];
            }
            else if (direction == "left") {
                spriteRenderer.sprite = left[animationFrame];
            }
            else if (direction == "right") {
                spriteRenderer.sprite = right[animationFrame];
            }
        }
    }
    public void Restart()
    {
        animationFrame = -1;

        Advance();
    }
}