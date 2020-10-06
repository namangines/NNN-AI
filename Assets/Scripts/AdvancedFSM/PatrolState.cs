using UnityEngine;
using System.Collections.Generic;
using TankPathingSystem;
using System;

public class PatrolState : FSMState
{
    private float waitconst = 7.5f;
    public PatrolState(NPCTankController tank) 
    {
        stateID = FSMStateID.Patrolling;
        this.tank = tank;
        curRotSpeed = 1.8f;
        curSpeed = 130.0f;
        waitTimer = waitconst;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //1. Check if the player collider is inside the sight of the tank
        Collider playerc = player.GetComponent<Collider>();
        if (tank.IsInsideSightFrustrum(playerc) && tank.HasLineOfSight(playerc))
        {
            //2. if so, switch to the chase state
            Debug.Log("Switch to Chase State");
            tank.SetTransition(Transition.SawPlayer);
        }

        float secondstilloffduty = 15f;

        if(tank.timeSinceOffduty > secondstilloffduty)
        {
            if (TankDutyManager.Instance.canGoOnDuty(this.tank))
            {
                Debug.Log("Transition to offduty");
                tank.destPath = null; //Make sure you dont leave any pathing data around
                tank.SetTransition(Transition.WantsTimeOff);
            }
        }
        else
        {
            tank.timeSinceOffduty += Time.deltaTime;
        }

        if (tank.health <= 10)
        {
            tank.destPath = null;
            tank.SetTransition(Transition.Hurt);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        float arbitrarydisttopoint = 75f;

        WaypointManager manager = WaypointManager.Instance;
        //1. Find another random patrol point if the current point is reached
        if (tank.destPath == null)
        {
            waitTimer = waitconst;
            destination = manager.GetRandomWaypoint(manager.GetClosestWaypoint(npc.position));
            Debug.Log("Finding path for patrol...");
            tank.destPath = manager.Path(manager.GetClosestWaypoint(npc.position), destination);
            Debug.Log("Patrol path is " + tank.destPath.Count + " nodes long");
            nextWaypoint = tank.destPath.Pop();
        }
        else if (nextWaypoint == null)
            nextWaypoint = tank.destPath.Pop();


        if (Vector3.Distance(npc.position, destination.transform.position) < arbitrarydisttopoint) //if final patrolpoint reached;
        {
            if (waitTimer > 0)
            {
                tank.ChangeLightColor(new Color(0, .6f, .6f));
                Transform turret = npc.GetComponent<NPCTankController>().turret;
                Quaternion turretRotation = Quaternion.AngleAxis(60 * Time.deltaTime, Vector3.up);
                turret.rotation = turretRotation * turret.rotation;
                waitTimer -= Time.deltaTime;
            }
            else
            {
                waitTimer = waitconst;
                tank.destPath = null; //reset destination
            }
        }
        else if (Vector3.Distance(npc.position, nextWaypoint.transform.position) > arbitrarydisttopoint)
        {
            tank.ChangeLightColor(Color.blue);
            MoveStraightTowards(npc, nextWaypoint.transform);
            Quaternion turretRotation = Quaternion.LookRotation(tank.transform.forward, tank.turret.transform.up);
            tank.turret.rotation = Quaternion.Slerp(tank.turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);
        }
        else if (nextWaypoint != destination)
        {
            nextWaypoint = tank.destPath.Pop();
        }
    }
}
