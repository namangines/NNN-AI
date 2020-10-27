using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManagerDel : MonoBehaviour
{

    private Dictionary<string, Action> eventDictionary;

    private static EventManagerDel eventManager;

    public static EventManagerDel instance
    {
        get
        {
            if (!eventManager)
            {
                eventManager = FindObjectOfType(typeof(EventManagerDel)) as EventManagerDel;

                if (!eventManager)
                {
                    Debug.LogError("There needs to be one active EventMangerDel script on a GameObject in your scene.");
                }
                else
                {
                    eventManager.Init();
                }
            }

            return eventManager;
        }
    }

    void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action>();
        }
    }

    public static void StartListening(string eventName, Action listener)
    {
        Action thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            //Add more event to the existing one
            thisEvent += listener;

            //Update the Dictionary
            instance.eventDictionary[eventName] = thisEvent;
        }
        else
        {
            //Add event to the Dictionary for the first time
            thisEvent += listener;
            instance.eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, Action listener)
    {
        if (eventManager == null) return;
        Action thisEvent;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            //Remove event from the existing one
            thisEvent -= listener;

            //Update the Dictionary
            instance.eventDictionary[eventName] = thisEvent;
        }
    }

    public static void TriggerEvent(string eventName)
    {
        Action thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
        {
           // OR USE  thisEvent.Invoke();
           instance.eventDictionary[eventName]();
        }
    }
}