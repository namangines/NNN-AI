using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankPathingSystem;

public class OffDutyState : FSMState
{
    public OffDutyState(Transform npc)
    {
        waitTimer = 20.0f;
        Transform offdutyArea = GameObject.FindGameObjectWithTag("Offduty Area").transform;
        WaypointManager manager = WaypointManager.Instance;
        Stack<Waypoint> pathToOffduty = manager.Path(manager.GetClosestWaypoint(npc.position), manager.GetClosestWaypoint(offdutyArea.transform.position));
    }

    public override void Reason(Transform player, Transform npc)
    {
        //if(seenPlayer)
    }

    public override void Act(Transform player, Transform npc)
    {
        float arbitrarydisttopoint = .3f;
        if (Vector3.Distance(npc.position, nextWaypoint.transform.position) > arbitrarydisttopoint)
        {
            MoveStraightTowards(npc, nextWaypoint.transform);
        }
        else if (nextWaypoint == destWaypoint) //if final patrolpoint reached;
        {
            if (waitTimer > 0)
            {
                waitTimer -= Time.deltaTime;
            }
            else
            {
                destPath = null; //reset destination
            }
        }
        else if (nextWaypoint != destWaypoint)
        {
            nextWaypoint = destPath.Pop();
        }
    }

    
}
