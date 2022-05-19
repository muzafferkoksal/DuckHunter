using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using CodeMonkey;

public class Bomb : MonoBehaviour
{
    public float speed = 0;
    public int id;
    public int x;
    public int y;

    public Vector3 worldCoor;
    public GameObject bombObj;

    public Bomb(int id, int x, int y, Vector3 worldCoor, GameObject bombObj)
    {
        this.id = id;
        this.x = x;
        this.y = y;
        this.worldCoor = worldCoor;
        this.bombObj = bombObj;
    }
    private void Start()
    {

    }
    private void Update()
    {

    }
}