using UnityEngine;
using System.Collections;

public class ChaseState : FSMState
{
    public const float giveUpTime = 8.0f;
    public const float seeDist = 250f;
    public ChaseState(NPCTankController tank) 
    { 
        stateID = FSMStateID.Chasing;
        this.tank = tank;
        curRotSpeed = 1.0f;
        curSpeed = 190.0f;

    }

    public override void Reason(Transform player, Transform npc)
    {
        //Look for the player
        RaycastHit hit;
        Ray ray = new Ray(npc.position, player.position - npc.position);
        if(Physics.Raycast(npc.position, player.position - npc.position, out hit, seeDist, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal))
        {
            //Set the time since last seen the player to be zero
            waitTimer = 0.0f;
        }
        
        float distToPlayer = Vector3.Distance(npc.position, player.position);
        if (distToPlayer <= tank.Sight.farClipPlane/2.5)
        {
            Debug.Log("Switch to Attack state");
            npc.GetComponent<NPCTankController>().SetTransition(Transition.ReachPlayer);
        }

        //Give up if the tank cannot find the player within the predifined time
        if(waitTimer > giveUpTime)
        {
            npc.GetComponent<NPCTankController>().SetTransition(Transition.LostPlayer);
        }

        waitTimer += Time.deltaTime;


        if (tank.health <= 10)
        {
            tank.destPath = null;
            tank.SetTransition(Transition.Hurt);
        }
    }

    public override void Act(Transform player, Transform npc)
    {
        //Rotate to the target point
        Vector3 destPos = player.position;
        tank.ChangeLightColor(Color.magenta);

        Quaternion targetRotation = Quaternion.LookRotation(destPos - npc.position);
        npc.rotation = Quaternion.Slerp(npc.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        Quaternion turretRotation = Quaternion.LookRotation(destPos - tank.turret.position);
        tank.turret.rotation = Quaternion.Slerp(tank.turret.rotation, turretRotation, Time.deltaTime * curRotSpeed*3);

        //Go Forward
        npc.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }
}
