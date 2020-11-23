using UnityEngine;
using System.Collections;
using TankPathingSystem;
using System.Collections.Generic;
using System.IO;
using System.Collections.Specialized;

public class ChaseState : FSMState
{
    private const float giveUpTime = 8.0f;
    private bool hitLast = false;

    public ChaseState(NPCTankController tank)
    {
        stateID = FSMStateID.Chasing;
        this.tank = tank;
        curRotSpeed = 2.0f;
        curSpeed = 200.0f;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //Look for the player
        if (tank.HasLineOfSight(player) || destPos == null || destPos == Vector3.zero)
        {
            //Set the time since last seen the player to be zero
            //Debug.Log("Player Seen");
            waitTimer = 0.0f;
            destPos = player.position;
        }
        else
        {
            waitTimer += Time.deltaTime;
        }


        float distToPlayer = Vector3.Distance(npc.position, player.position);
        if (distToPlayer <= tank.Sight.farClipPlane / 2f && tank.HasLineOfSight(player))
        {
            tank.destPath = null;
            Debug.Log("Switch to Attack state");
            tank.SetTransition(Transition.ReachPlayer);
        }

        //Give up if the tank cannot find the player within the predifined time
        if (waitTimer > giveUpTime)
        {
            tank.destPath = null;
            tank.SetTransition(Transition.LostPlayer);
            if (tank.TankClasses.Contains("Normal"))
                tank.SightLightOff();
        }


        if (tank.health <= 10)
        {
            tank.destPath = null;
            tank.SetTransition(Transition.Hurt);
            if (tank.TankClasses.Contains("Normal"))
                tank.SightLightOff();
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        tank.ChangeLightColor(Color.magenta);
        tank.SightLightOn();


        tank.ChangeLightColor(new Color(1, 0, .65f));
        tank.NavigateToPosition(player);
        RotateTurretTowards(tank.turret, player.transform.position);
    }
}
