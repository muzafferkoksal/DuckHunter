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

    public Vector3 worldCoor; 
    public GameObject duckObj;

    public Duck(int id, int x, int y, Vector3 worldCoor, GameObject duckObj){
        this.id = id;
        this.x = x;
        this.y = y;
        this.worldCoor = worldCoor;
        this.duckObj = duckObj;
    }
    private void Start()
    {

    }
    private void Update(){

    }
}