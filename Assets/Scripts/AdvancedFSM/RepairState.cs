using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankPathingSystem;

public class RepairState : FSMState
{
    public RepairState(NPCTankController tank)
    {
        curRotSpeed = 10;
        curSpeed = 220;
        stateID = FSMStateID.Repairing;
        this.tank = tank;
        Transform repairarea = GameObject.FindGameObjectWithTag("Repair Area").transform;
        destination = repairarea.GetComponent<Waypoint>();
    }

    public override void Act(Transform player, Transform npc)
    {
        if(tank.health >= 100)
        {
            tank.destPath = null;
            tank.SetTransition(Transition.Healed);
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        if (tank.destPath == null)
        {
            WaypointManager manager = WaypointManager.Instance;
            tank.destPath = manager.Path(manager.GetClosestWaypoint(npc.position), destination);
            nextWaypoint = tank.destPath.Pop();
        }
        else if (nextWaypoint == null)
            nextWaypoint = tank.destPath.Pop();

        if (Vector3.Distance(npc.position, destination.transform.position) < 75f) //if final patrolpoint reached;
        {
            if (tank.health < 100)
            {
                tank.ChangeLightColor(new Color(.25f, 1f, .5f));
                waitTimer -= Time.deltaTime;
                tank.health += 1;
            }

        }
        else if (Vector3.Distance(npc.position, nextWaypoint.transform.position) > 75f)
        {
            tank.ChangeLightColor(Color.yellow);
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
