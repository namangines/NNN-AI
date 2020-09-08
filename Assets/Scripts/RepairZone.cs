using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairZone : MonoBehaviour
{
    public float curTime = 0f;
    //How long in between healing procs
    public float healTime = 1f;
    public int healAmount = 15;
    //Store the core file to reduce redundancy
    Dictionary<Collider, SimpleFSM> validCollided;

    private void Start()
    {
        validCollided = new Dictionary<Collider, SimpleFSM>();
    }

    private void OnTriggerStay(Collider other)
    {
        SimpleFSM core;
        //to reduce expensive getComponent calls every frame, a simple test is implemented to see if the object is already validated
        if (validCollided.TryGetValue(other, out core))
        {}
        else if (other.gameObject.TryGetComponent<SimpleFSM>(out core))
        {
            validCollided.Add(other, core); //Add the valid collider to the list
        }
        else return; //If both checks have failed, the object is not valid


        if(core != null)
        {
            curTime += Time.deltaTime;
            if(curTime >= healTime)
            {
                curTime = 0f;
                core.health += healAmount;
                if (core.health > core.maxHealth) core.health = core.maxHealth;
            }
        }
    }
}
