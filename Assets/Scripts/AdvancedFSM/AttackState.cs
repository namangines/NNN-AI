using UnityEngine;
using System.Collections;

public class AttackState : FSMState
{
    const float lastSeenTimer = 3f;

    public AttackState(NPCTankController tank)
    {
        stateID = FSMStateID.Attacking;
        curRotSpeed = 1.5f;
        curSpeed = 100.0f;
        this.tank = tank;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //Check the distance with the player tank
        float dist = Vector3.Distance(npc.position, player.position);

        //If it's close enough to see, yet too far to attack, switch to chase. Alternatively, always switch to chase if line of sight is obstructed
        Collider playerc = player.GetComponent<Collider>();
        if (dist >= tank.Sight.farClipPlane / 1.5f || !tank.HasLineOfSight(playerc))
        {
            Debug.Log("Switch to Chase State");
            tank.SetTransition(Transition.SawPlayer);
        }

        //Transition to patrol is the tank become too far
        else if (dist >= tank.Sight.farClipPlane)
        {
            Debug.Log("Switch to Patrol State");
            tank.destPath = null;
            tank.SetTransition(Transition.LostPlayer);
        }

        

        //regardless of all, transition to repair if hurt
        if(tank.health <= 10)
        {
            tank.destPath = null;
            tank.SetTransition(Transition.Hurt);
        }

    }

    public override void Act(Transform player, Transform npc)
    {
        float dist = Vector3.Distance(npc.position, player.position);
        if (dist >= tank.Sight.farClipPlane / 4)
            MoveStraightTowards(npc, player.position);

        //Set the target position as the player position
        Vector3 destPos = player.position;
        tank.ChangeLightColor(Color.red);

        //Always Turn the turret towards the player
        Transform turret = tank.turret;
        Quaternion turretRotation = Quaternion.LookRotation(destPos - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);

        //Shoot bullet towards the player
        npc.GetComponent<NPCTankController>().ShootBullet();
    }
}
