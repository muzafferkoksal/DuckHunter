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
    private PathfindingVisual pathfinding;
    private bool aiStarted = false;
    private Stopwatch timer;
    public Text timeText;

    private void Start() {
        Transform bodyTransform = transform.Find("Body");
        unitSkeleton = new V_UnitSkeleton(1f, bodyTransform.TransformPoint, (Mesh mesh) => bodyTransform.GetComponent<MeshFilter>().mesh = mesh);
        unitAnimation = new V_UnitAnimation(unitSkeleton);
        animatedWalker = new AnimatedWalker(unitAnimation, UnitAnimType.GetUnitAnimType("dMarine_Idle"), UnitAnimType.GetUnitAnimType("dMarine_Walk"), 1f, 1f);
        mode = GameObject.Find("Testing").GetComponent<Testing>().mode;
        ducks = GameObject.Find("DuckCreator").GetComponent<DuckCreator>();
        pathfinding = GameObject.Find("PathfindingVisual").GetComponent<PathfindingVisual>();
        timer = new Stopwatch();
    }

    private void Restart()
    {
        timer = new Stopwatch();
        ducks.spawnDucks();
    }

    private void Update() {
        HandleMovement();
        unitSkeleton.Update(Time.deltaTime);

        //Vector3 worldHunter = new Vector3(GetPosition().x, GetPosition().y, GetPosition().z);
        //int hunterY = 0, hunterX = 0;
        //pathfinding.grid.GetXY(worldHunter, out hunterX, out hunterY);
        //worldHunter = pathfinding.grid.GetWorldPosition(hunterX, hunterY);

        //int duckX = 0, duckY = 0;
        //GameObject duck = null;
        //Vector3 worldDuck = new Vector3(0, 0, 0); ;

        //if ((ducks.ducks).Any())
        //{
        //    UnityEngine.Debug.Log("here");
        //    Duck closest = ducks.getClosestDuck(worldHunter);

        //    duckX = closest.x;
        //    duckY = closest.y;
        //    duck = closest.duckObj;
        //    worldDuck = closest.worldCoor;
        //}
        //else
        //{
        //    timer.Stop();
        //    string elapsed = "Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff");
        //    setTime(timer.Elapsed);
        //    UnityEngine.Debug.Log(elapsed);
        //    aiStarted = false;
        //}

        //if (aiStarted)
        //{
        //    string elapsed = "Time taken: " + timer.Elapsed.ToString(@"m\:ss\.fff");
        //    setTime(timer.Elapsed);
        //    SetTargetPosition(worldDuck);
        //}
        //if (Input.GetKeyDown(KeyCode.Space))
        //{

        //    timer.Start();
        //    aiStarted = true;
        //    SetTargetPosition(worldDuck);
        //}
        //if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        //{
        //    aiStarted = false;
        //    Vector3 v = new Vector3(worldHunter.x, 299, 0);
        //    SetTargetPosition(v);
        //}
        //else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        //{
        //    aiStarted = false;
        //    Vector3 v = new Vector3(worldHunter.x, 0, 0);
        //    SetTargetPosition(v);
        //}
        //else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    aiStarted = false;
        //    Vector3 v = new Vector3(0, worldHunter.y, 0);
        //    SetTargetPosition(v);
        //}
        //else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    aiStarted = false;
        //    Vector3 v = new Vector3(199, worldHunter.y, 0);
        //    SetTargetPosition(v);
        //}

        //if (hunterX == duckX && hunterY == duckY)
        //{
        //    ducks.removeDuck(duck);
        //}

        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    Restart();
        //}

        if (Input.GetMouseButtonDown(0))
        {
            SetTargetPosition(UtilsClass.GetMouseWorldPosition());
            UnityEngine.Debug.Log(UtilsClass.GetMouseWorldPosition());
        }
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
            //pathVectorList = Pathfinding.Instance.findBestMove();
            //DFS
            pathVectorList = Pathfinding.Instance.SearchDeep(GetPosition(), targetPosition);
            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
            }
            //BFS
            //pathVectorList = Pathfinding.Instance.BreadthFirstSearch(GetPosition(), targetPosition);
            //if (pathVectorList != null && pathVectorList.Count > 1)
            //{
            //    pathVectorList.RemoveAt(0);
            //}
        }
        else if(mode == 1)
        {
            Pathfinding.Instance.MDP(GetPosition(), targetPosition);
        } else if (mode == 2)
        {
            pathVectorList = Pathfinding.Instance.findBestMove();
            if (pathVectorList != null && pathVectorList.Count > 1)
            {
                pathVectorList.RemoveAt(0);
            }
        }
    }

    private void setTime(TimeSpan t)
    {
        timeText.text = t.ToString("mm':'ss':'ff");
    }
}