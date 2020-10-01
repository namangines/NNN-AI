using UnityEngine;
using System.Collections.Generic;
using TankPathingSystem;

public class PatrolState : FSMState
{
    private float waitconst = 10.0f;
    public PatrolState() 
    {
        stateID = FSMStateID.Patrolling;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
        waitTimer = waitconst;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //1. Check the distance with player
        float dist = Vector3.Distance(npc.position, player.position);
        if(dist <= chaseDist && dist > attackDist)
        {
            //2. Since the distance is near, transition to chase state
            Debug.Log("Switch to Chase State");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.SawPlayer);
        }
        else if(dist <= attackDist)
        {
            npc.GetComponent<NPCTankController>().SetTransition(Transition.ReachPlayer);
        }

    }

    public override void Act(Transform player, Transform npc)
    {
        float arbitrarydisttopoint = .15f;

        WaypointManager manager = WaypointManager.Instance;
        //1. Find another random patrol point if the current point is reached
		if(destPath == null)
        {
            waitTimer = waitconst;
            destWaypoint = manager.GetRandomWaypoint();
            destPath = manager.Path(manager.GetClosestWaypoint(npc.position),destWaypoint);
            nextWaypoint = destPath.Pop();
        }


        if (Vector3.Distance(npc.position, nextWaypoint.transform.position) > arbitrarydisttopoint)
        {
            MoveStraightTowards(npc, nextWaypoint.transform);
        }
        else if(nextWaypoint == destWaypoint) //if final patrolpoint reached;
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
