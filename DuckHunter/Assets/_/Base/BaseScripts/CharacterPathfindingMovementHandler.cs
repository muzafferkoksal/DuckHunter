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
using UnityEngine.UI;
using V_AnimationSystem;
using CodeMonkey.Utils;
using System.Linq;
using System.Diagnostics;
using System;

public class CharacterPathfindingMovementHandler : MonoBehaviour {

    private const float speed = 40f;

    private V_UnitSkeleton unitSkeleton;
    private V_UnitAnimation unitAnimation;
    private AnimatedWalker animatedWalker;
    private int currentPathIndex;
    private List<Vector3> pathVectorList;
    public int mode;
    private DuckCreator ducks;
    private BombCreator bombs;
    private PathfindingVisual pathfinding;
    private bool aiStarted = false;
    private Stopwatch timer;
    public Text timeText;
    private bool searchingDuck;
    private int hunterY = 0, hunterX = 0;
    private GameObject duck = null;
    private Vector3 worldDuck = new Vector3(0, 0, 0);
    private int duckX = 0, duckY = 0;
    public int row;
    public int column;

    private void Start() {
        row = GameObject.Find("Testing").GetComponent<Testing>().row;
        column = GameObject.Find("Testing").GetComponent<Testing>().column;
        Transform bodyTransform = transform.Find("Body");
        unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint, (Mesh mesh) => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
        unitAnimation = new V_UnitAnimation(unitSkeleton);
        animatedWalker = new AnimatedWalker(unitAnimation, UnitAnimType.GetUnitAnimType("dMarine_Idle"), UnitAnimType.GetUnitAnimType("dMarine_Walk"), 1f, 1f);
        mode = GameObject.Find("Testing").GetComponent<Testing>().mode;
        ducks = GameObject.Find("DuckCreator").GetComponent<DuckCreator>();
        bombs = GameObject.Find("BombCreator").GetComponent<BombCreator>();
        pathfinding = GameObject.Find("PathfindingVisual").GetComponent<PathfindingVisual>();
        timer = new Stopwatch();
    }

    private void Restart()
    {
        timer = new Stopwatch();
        ducks.spawnDucks();
        bombs.spawnBombs();
    }

    private void Update() {
        HandleMovement();
        unitSkeleton.Update(Time.deltaTime);

        Vector3 worldHunter = new Vector3(GetPosition().x, GetPosition().y, GetPosition().z);

        pathfinding.grid.GetXY(worldHunter, out hunterX, out hunterY);
        worldHunter = pathfinding.grid.GetWorldPosition(hunterX, hunterY);

        if ((ducks.ducks).Any() && searchingDuck)
        {
            UnityEngine.Debug.Log("here");
            Duck closest = ducks.getClosestDuck(worldHunter);

            duckX = closest.x;
            duckY = closest.y;
            duck = closest.duckObj;
            worldDuck = closest.worldCoor;
            UnityEngine.Debug.Log("Searching duck");
            searchingDuck = false;
        }
        else
        {
            timer.Stop();
            string elapsed = "Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff");
            setTime(timer.Elapsed);
            //UnityEngine.Debug.Log(elapsed);
            aiStarted = false;
        }

        if (aiStarted && !searchingDuck)
        {
            string elapsed = "Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff");
            setTime(timer.Elapsed);
            SetTargetPosition(worldDuck);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            timer.Start();
            aiStarted = true;
            searchingDuck = true;
            SetTargetPosition(worldDuck);
        }
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            aiStarted = false;
            Vector3 v = new Vector3(worldHunter.x, 299, 0);
            searchingDuck = false;
            pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), v);
            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            aiStarted = false;
            Vector3 v = new Vector3(worldHunter.x, 0, 0);
            searchingDuck = false;
            pathVectorList = Pathfinding.Instance.FindPath(GetPosition(), v);
            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            aiStarted = false;
            Vector3 v = new Vector3(0, worldHunter.y, 0);
            searchingDuck = false;
            SetTargetPosition(v);
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            aiStarted = false;
            Vector3 v = new Vector3(199, worldHunter.y, 0);
            searchingDuck = false;
            SetTargetPosition(v);
        }

        if (hunterX == duckX && hunterY == duckY)
        {
            UnityEngine.Debug.Log("found the duck!\n");
            ducks.removeDuck(duck);
            searchingDuck = true;
            aiStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }

        //if (Input.GetMouseButtonDown(0))
        //{
        //    SetTargetPosition(UtilsClass.GetMouseWorldPosition());
        //    UnityEngine.Debug.Log(UtilsClass.GetMouseWorldPosition());
        //}
    }
    
    private void HandleMovement() {
        if (pathVectorList != null) {
            Vector3 targetPosition = pathVectorList[currentPathIndex];
            if (Vector3.Distance(transform.position, targetPosition) > 1f) {
                Vector3 moveDir = (targetPosition - transform.position).normalized;

                float distanceBefore = Vector3.Distance(transform.position, targetPosition);
                animatedWalker.SetMoveVector(moveDir);
                transform.position = transform.position + moveDir * speed * Time.deltaTime;
            } else {
                currentPathIndex++;
                if (currentPathIndex >= pathVectorList.Count) {
                    StopMoving();
                    animatedWalker.SetMoveVector(Vector3.zero);
                }
            }
        } else {
            animatedWalker.SetMoveVector(Vector3.zero);
        }
    }

    private void StopMoving() {
        pathVectorList = null;
    }

    public Vector3 GetPosition() {
        return transform.position;
    }

    public void SetTargetPosition(Vector3 targetPosition) {
        currentPathIndex = 0;
        if(mode == 0)
        {
            //DFS
            pathVectorList = Pathfinding.Instance.SearchDeep(GetPosition(), targetPosition);
            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
            }
        }
        else if(mode == 1)
        {
            UnityEngine.Debug.Log("tttttttttttttttttttttttttttttttttt " + targetPosition);
            //MDP
            Pathfinding.Instance.MDP(GetPosition(), targetPosition);
        } else if (mode == 2)
        {
            //Adversarial
            pathVectorList = Pathfinding.Instance.findBestMove();
            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
            }
        }
        else if (mode == 3)
        {
            //BFS
            pathVectorList = Pathfinding.Instance.BreadthFirstSearch(GetPosition(), targetPosition);
            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
            }
        }
        else if (mode == 4)
        {
            //UCS
            pathVectorList = Pathfinding.Instance.UCS(GetPosition(), targetPosition);
            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
            }
        }
        else if (mode == 5)
        {
            //Q learning
            Pathfinding.Instance.QLearn(GetPosition(), targetPosition);
        }
    }

    private void setTime(TimeSpan t)
    {
        timeText.text = t.ToString("mm':'ss':'ff");
    }
}