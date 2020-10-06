using UnityEngine;
using System.Collections.Generic;
using TankPathingSystem;

public class FSM : MonoBehaviour
{
    //Player Transform
    protected Transform playerTransform;

    //For pathing purposes we keep this here cuz reasons?!?
    //public Waypoint destWaypoint;
    //The path that must be taken to get to the current state's desination from current position
    public Stack<Waypoint> destPath;

    //List of points for patrolling
    //This is now handled by the WaypointManager
    ///protected GameObject[] pointList;

    //Bullet shooting rate
    protected float shootRate;
    protected float elapsedTime;

    protected float timeSinceOffDuty;

    //Tank Turret
    public Transform turret { get; set; }
    public Transform bulletSpawnPoint { get; set; }

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }

    // Use this for initialization
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        FSMUpdate();
    }

    void FixedUpdate()
    {
        FSMFixedUpdate();
    }
}
