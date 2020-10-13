using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HideState : FSMState
{
    const float hidetimer = 10f;

    public HideState(NPCTankController tank)
    {
        stateID = FSMStateID.Hiding;
        this.tank = tank;
    }

    public override void Reason(Transform player, Transform npc)
    {
        float distToPlayer = Vector3.Distance(npc.position, player.position);
        if (distToPlayer <= tank.Sight.farClipPlane / 2 && tank.HasLineOfSight(player))
        {
            Debug.Log("Switch to Attack state");
            tank.LightOn();
            tank.remainHidden = false; //Ninja tanks should, in the future, chase the player after being revealed
            tank.SetTransition(Transition.ReachPlayer);
        }

        if(!tank.HasLineOfSight(player) && !tank.remainHidden)
        {
            Debug.Log("Switch to patrol state");
            tank.destPath = null;
            tank.SetTransition(Transition.LostPlayer);
        }

        if (waitTimer >= hidetimer)
        {
            tank.LightOn();
            tank.SetTransition(Transition.RestedLongEnough);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        tank.LightOff();
        if(!tank.remainHidden)
            waitTimer += Time.deltaTime;
    }

}
