using UnityEngine;
using System.Collections;

public class PatrolState : FSMState
{
    public PatrolState(Transform[] wp) 
    { 
        waypoints = wp;
        stateID = FSMStateID.Patrolling;

        curRotSpeed = 1.0f;
        curSpeed = 100.0f;
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
        //1. Find another random patrol point if the current point is reached
		

        //2. Rotate to the target point


        //3. Go Forward

    }
}