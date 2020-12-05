using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TankPathingSystem;

public class TankDutyManager : MonoBehaviour
{
    public static TankDutyManager Instance;

    public int NumberOfFastTanks = 3;

    List<NPCTankController> OnDuty = new List<NPCTankController>();
    List<NPCTankController> AllTanks = new List<NPCTankController>();
    List<NPCTankController> DeadTanks = new List<NPCTankController>();

    private void Start()
    {
        Instance = this;
        AllTanks = new List<NPCTankController>(this.GetComponentsInChildren<NPCTankController>());
        Debug.Log(AllTanks.Count + " tanks found");


        if (NumberOfFastTanks > AllTanks.Count)
            NumberOfFastTanks = AllTanks.Count;

        HashSet<int> fastIndexes = new HashSet<int>();
        for (int i = 0; i < NumberOfFastTanks; i++)
        {
            int index = Random.Range(0, AllTanks.Count-1);
            //This could be dangerous, but because we ensure that the number of fast tanks is less than the total there should always be 'places' to put the class
            while ( !fastIndexes.Add(index) )
            {
                index++;
                if (index > AllTanks.Count)
                    index = 0;
            }

        }

        /*
        for (int i = 0; i < AllTanks.Count; i++)
        {
            if (fastIndexes.Contains(i))
            {
                AllTanks[i].TankClasses.Add("Fast");
                AllTanks[i].AuraLight.color = Color.red;
                fastIndexes.Remove(i);
            }
            else
            {
                AllTanks[i].TankClasses.Add("Normal");
                AllTanks[i].SightLightOff();
            }
        }
        */
    }

    public float GetChanceToHide()
    {
        float tankcount = (float)AllTanks.Count;
        float deadcount = (float)DeadTanks.Count;

        return deadcount / tankcount;
        //return 1; //Only for debugging!
    }

    public void TankHasDied(NPCTankController self)
    {
        if(AllTanks.Contains(self))//Only tanks on the roster should be tracked. AKA, no ninja tanks
            DeadTanks.Add(self);
    }

    public bool canGoOnDuty(NPCTankController self)
    {
        if (!AllTanks.Contains(self))//Only tanks on the roster can go off duty
            return false;

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
