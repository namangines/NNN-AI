using UnityEngine;
using System.Collections;

public class AttackState : FSMState
{
    public AttackState() 
    { 
        stateID = FSMStateID.Attacking;
        curRotSpeed = 1.0f;
        curSpeed = 100.0f;

    }

    public override void Reason(Transform player, Transform npc)
    {
        //Check the distance with the player tank
        float dist = Vector3.Distance(npc.position, player.position);
        if (dist >= 200.0f && dist < 300.0f)
        {
            MoveStraightTowards(npc, player);

            Debug.Log("Switch to Chase State");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.SawPlayer);
        }
        //Transition to patrol is the tank become too far
        else if (dist >= 300.0f)
        {
            Debug.Log("Switch to Patrol State");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LostPlayer);
        }  
    }

    public override void Act(Transform player, Transform npc)
    {
        //Set the target position as the player position
        Vector3 destPos = player.position;

        //Always Turn the turret towards the player
        Transform turret = npc.GetComponent<NPCTankController>().turret;
        Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

        //Shoot bullet towards the player
        npc.GetComponent<NPCTankController>().ShootBullet();
    }
}
