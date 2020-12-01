using UnityEngine;
using System.Collections.Generic;
using TankPathingSystem;

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
        destPos = tank.transform.position;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //1. Check if the player collider is inside the sight of the tank
        Collider playerc = player.GetComponent<Collider>();

        if (tank.IsInsideSightFrustrum(playerc) && tank.HasLineOfSight(playerc) && !tank.TankClasses.Contains("Normal"))
        {
            if (TankDutyManager.Instance.GetChanceToHide() > Random.Range(0, 1.0f))
            {
                waitTimer = 0f;
                tank.destPath = null;
                Debug.Log("Switch to Hiding State");
                tank.SetTransition(Transition.WantsToHide);
            }
            else
            {
                Debug.Log("Switch to Chase State");
                waitTimer = 0;
                tank.SetTransition(Transition.SawPlayer);
            }
        }

        float secondstilloffduty = 15f;

        if(tank.timeSinceOffduty > secondstilloffduty)
        {
            if (TankDutyManager.Instance.canGoOnDuty(this.tank))
            {
                //Debug.Log("Transition to offduty");
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
        if (tank.TankClasses.Contains("Guard"))
        {
            tank.NavigateToPosition(destPos);
        }
        else if (tank.TankClasses.Contains("Patrol"))
        {
            tank.NavigateToPosition(tank.Waypoints[tank.currentWaypoint]);
            if(Vector3.Distance(tank.transform.position, tank.Waypoints[tank.currentWaypoint].position) < 75f)
            {
                if (waitTimer > 10)
                {
                    waitTimer = 0;
                    tank.currentWaypoint++;
                    if (tank.currentWaypoint >= tank.Waypoints.Count)
                        tank.currentWaypoint = 0;
                }
                else
                    waitTimer += Time.deltaTime;
            }
        }
    }

    //Just keeping these around for now in case something breaks with the navmesh implementation
    public  void Reason_OLD(Transform player, Transform npc)
    {
        //1. Check if the player collider is inside the sight of the tank
        Collider playerc = player.GetComponent<Collider>();

        if (tank.IsInsideSightFrustrum(playerc) && tank.HasLineOfSight(playerc) && !tank.TankClasses.Contains("Normal"))
        {
            if (TankDutyManager.Instance.GetChanceToHide() > Random.Range(0, 1.0f))
            {
                waitTimer = 0f;
                tank.destPath = null;
                Debug.Log("Switch to Hiding State");
                tank.SetTransition(Transition.WantsToHide);
            }
            else
            {
                Debug.Log("Switch to Chase State");
                waitTimer = 0;
                tank.SetTransition(Transition.SawPlayer);
            }
        }

        float secondstilloffduty = 15f;

        if (tank.timeSinceOffduty > secondstilloffduty)
        {
            if (TankDutyManager.Instance.canGoOnDuty(this.tank))
            {
                //Debug.Log("Transition to offduty");
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
    public void Act_OLD(Transform player, Transform npc)
    {
        float arbitrarydisttopoint = 75f;

        WaypointManager manager = WaypointManager.Instance;
        //1. Find another random patrol point if the current point is reached
        if (destination == null)
        {
            waitTimer = waitconst;
            destination = manager.GetRandomWaypoint(manager.GetClosestWaypoint(npc.position));
            tank.NavigateToPosition(destination.transform);
        }


        if (Vector3.Distance(npc.position, destination.transform.position) < arbitrarydisttopoint) //if final patrolpoint reached;
        {
            if (waitTimer > 0 && !tank.TankClasses.Contains("Normal"))
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
                destination = null; //reset destination
            }
        }
        else
        {
            tank.ChangeLightColor(Color.blue);
            Quaternion turretRotation = Quaternion.LookRotation(tank.transform.forward, tank.turret.transform.up);
            tank.turret.rotation = Quaternion.Slerp(tank.turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);
        }
    }
}
