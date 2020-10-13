using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// In an ideal world this script could search for all the tanks in the scene, but this quick & dirty solution will work for a limited number of tanks
/// </summary>
public class TankMonitor : MonoBehaviour
{
    public NPCTankController tank;
    private Text text;

    private void Start()
    {
        text = this.GetComponent<Text>();
    }

    private void Update()
    {
        if (tank != null)
            text.text = tank.name + tank.GetHashCode() + " : Health : " + tank.health;
        else
            text.text = "DEAD";
    }
}
