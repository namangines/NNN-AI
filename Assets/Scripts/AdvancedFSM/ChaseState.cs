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
        if(tank.HasLineOfSight(player) || destPos == null || destPos == Vector3.zero)
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
        if (distToPlayer <= tank.Sight.farClipPlane/2f && tank.HasLineOfSight(player))
        {
            tank.destPath = null;
            Debug.Log("Switch to Attack state");
            tank.SetTransition(Transition.ReachPlayer);
        }

        //Give up if the tank cannot find the player within the predifined time
        if(waitTimer > giveUpTime)
        {
            tank.destPath = null;
            tank.SetTransition(Transition.LostPlayer);
        }


        if (tank.health <= 10)
        {
            tank.destPath = null;
            tank.SetTransition(Transition.Hurt);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        tank.ChangeLightColor(Color.magenta);


        //A bit clunky but essentially checks to see if it can see the player, and if not, uses pathing to find the closest path to the player and heads towards it
        if (tank.HasLineOfSight(destPos, player) || hitLast)
        {
            tank.ChangeLightColor(new Color(1, 0, .65f));
            MoveStraightTowards(npc, destPos);
            RotateTurretTowards(tank.turret, destPos);
        }
        else
        {
            tank.ChangeLightColor(new Color(.65f, 0, 1));

            if (tank.destPath == null)
            {
                tank.destPath = WaypointManager.Instance.Path(npc.position, destPos);
                nextWaypoint = tank.destPath.Pop();
                destination = WaypointManager.Instance.GetClosestWaypoint(destPos);
                hitLast = false;
            }

            if (tank.destPath != null && tank.destPath.Count > 0 &&
                    (nextWaypoint == null || Vector3.Distance(npc.position, nextWaypoint.transform.position) < 75f)
                )
            {
                nextWaypoint = tank.destPath.Pop();
            }

            //Regardless of above,
            if (tank.destPath != null && nextWaypoint != null)
            {
                MoveStraightTowards(npc, nextWaypoint.transform.position);
                RotateTurretTowards(tank.turret, nextWaypoint.transform.position);
            }

            if (nextWaypoint != null && nextWaypoint == destination && Vector3.Distance(npc.position, nextWaypoint.transform.position) < 75f) //If you have reached the final node, just head straight for the player instead
            {
                hitLast = true;
            }
        }


        //Regardless of others, If the final node is no longer the closest valid node to the player, invalidate the path
        //Really only useful if you move. With the 'last seen' position this should never change
        if (WaypointManager.Instance.GetClosestWaypoint(destPos) != destination)
        {
            hitLast = false;
            nextWaypoint = null;
            tank.destPath = null;
        }
    }
}
