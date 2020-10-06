using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankPathingSystem;

public class TankDutyManager : MonoBehaviour
{
    public static TankDutyManager Instance;

    List<NPCTankController> OnDuty = new List<NPCTankController>();
    List<NPCTankController> AllTanks = new List<NPCTankController>();

    private void Start()
    {
        Instance = this;
        AllTanks = new List<NPCTankController>(this.GetComponentsInChildren<NPCTankController>());
        Debug.Log(AllTanks.Count + " tanks found");
    }

    public bool canGoOnDuty(NPCTankController self)
    {
        if((float)OnDuty.Count/(float)AllTanks.Count < .2f && AllTanks.Count > 1)
        {
            OnDuty.Add(self);
            return true;
        }
        return false;
    }

    public bool returnToDuty(NPCTankController self)
    {
        return OnDuty.Remove(self);
    }
}
