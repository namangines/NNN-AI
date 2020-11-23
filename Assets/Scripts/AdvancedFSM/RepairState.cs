using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankPathingSystem;

public class RepairState : FSMState
{
    public RepairState(NPCTankController tank)
    {
        curRotSpeed = 12;
        curSpeed = 250;
        stateID = FSMStateID.Repairing;
        this.tank = tank;
        Transform repairarea = GameObject.FindGameObjectWithTag("Repair Area").transform;
        destination = repairarea.GetComponent<Waypoint>();
    }

    public override void Act(Transform player, Transform npc)
    {
        if (tank.health >= 100)
        {
            tank.destPath = null;
            tank.SetTransition(Transition.Healed);
        }
    }

    public override void Reason(Transform player, Transform npc)
    {
        tank.NavigateToPosition(destination.transform);

        if (Vector3.Distance(npc.position, destination.transform.position) < 75f) //if final patrolpoint reached;
        {
            if (tank.health < 100)
            {
                tank.ChangeLightColor(new Color(.25f, 1f, .5f));
                waitTimer -= Time.deltaTime;
                tank.health += 1;
            }

        }
        else
        {
            tank.ChangeLightColor(Color.yellow);
        }
    }
}
