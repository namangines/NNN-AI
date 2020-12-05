using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankPathingSystem;

public class OffDutyState : FSMState
{
    public OffDutyState(Transform npc, NPCTankController tank)
    {
        curRotSpeed = 1.3f;
        curSpeed = 100.0f;
        stateID = FSMStateID.OffDuty;
        waitTimer = 20.0f;
        Transform offdutyArea = GameObject.FindGameObjectWithTag("Offduty Area").transform;
        destination = offdutyArea.GetComponent<Waypoint>();
        this.tank = tank;
        tank.destPath = null;
    }

    public override void Reason(Transform player, Transform npc)
    {
        Collider playerc = player.GetComponent<Collider>();
        if (tank.IsInsideSightFrustrum(playerc) && tank.HasLineOfSight(playerc))
        {
            if (TankDutyManager.Instance.GetChanceToHide() > Random.Range(0, 1.0f))
            {
                waitTimer = 0;
                tank.SetTransition(Transition.WantsToHide);
                tank.destPath = null;
            }
            else
            {
                waitTimer = 0;
                tank.SetTransition(Transition.SawPlayer);
                tank.destPath = null;
            }
        }

        if (waitTimer <= 0)
        {
            Debug.Log("Transition from offduty to patrol");
            TankDutyManager.Instance.returnToDuty(this.tank);
            tank.SetTransition(Transition.RestedLongEnough);
            tank.destPath = null;
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        tank.NavigateToPosition(destination.transform);


        float arbitrarydisttopoint = 4f;

        if (Vector3.Distance(npc.position, destination.transform.position) < arbitrarydisttopoint) //if final patrolpoint reached;
        {
            tank.timeSinceOffduty = 0;
            if (waitTimer >= 0)
            {
                tank.ChangeLightColor(new Color(0f, .5f, 0f));
                waitTimer -= Time.deltaTime;
            }

        }
        else
        {
            tank.ChangeLightColor(Color.green);
            Quaternion turretRotation = Quaternion.LookRotation(destination.transform.position - tank.turret.position);
            tank.turret.rotation = Quaternion.Slerp(tank.turret.rotation, turretRotation, Time.deltaTime * curRotSpeed);
            //FSMGizmoDrawer.DrawLine(npc.position, nextWaypoint.transform.position, Color.blue);
            Debug.Log("Reached endzone");
        }
    }
}

