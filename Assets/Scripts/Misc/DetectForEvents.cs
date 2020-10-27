using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class DetectForEvents : MonoBehaviour
{

    void OnEnable()
    {
        EventManagerDel.StartListening("Sound Detected", playerHeard);
    }

    void OnDisable()
    {
        EventManagerDel.StopListening("Sound Detected", playerHeard);
    }

    void playerHeard()
    {
        //function to run when sound is heard
        Debug.Log("Player was heard");
    }

}
