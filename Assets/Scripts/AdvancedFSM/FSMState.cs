using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TankPathingSystem;

/// <summary>
/// This class is adapted and modified from the FSM implementation class available on UnifyCommunity website
/// The license for the code is Creative Commons Attribution Share Alike.
/// It's originally the port of C++ FSM implementation mentioned in Chapter01 of Game Programming Gems 1
/// You're free to use, modify and distribute the code in any projects including commercial ones.
/// Please read the link to know more about CCA license @http://creativecommons.org/licenses/by-sa/3.0/

/// This class represents the States in the Finite State System.
/// Each state has a Dictionary with pairs (transition-state) showing
/// which state the FSM should be if a transition is fired while this state
/// is the current state.
/// Reason method is used to determine which transition should be fired .
/// Act method has the code to perform the actions the NPC is supposed to do if it�s on this state.
/// </summary>
public abstract class FSMState
{
    protected Dictionary<Transition, FSMStateID> map = new Dictionary<Transition, FSMStateID>();
    protected FSMStateID stateID;
    public FSMStateID ID { get { return stateID; } }
    public Waypoint destination; //Optional ability to cache final destinations for states such as offduty and repair
    public Vector3 destPos;
    public NPCTankController tank; //Only public for the Gizmodrawer to see, otherwise should be protected //Same as above
    protected Waypoint nextWaypoint;
    protected float waitTimer;
    protected float curRotSpeed;
    protected float curSpeed;
    protected float sightRadius;

    public void AddTransition(Transition transition, FSMStateID id)
    {
        // Check if anyone of the args is invallid
        if (transition == Transition.None || id == FSMStateID.None)
        {
            Debug.LogWarning("FSMState : Null transition not allowed");
            return;
        }

        //Since this is a Deterministc FSM,
        //Check if the current transition was already inside the map
        if (map.ContainsKey(transition))
        {
            Debug.LogWarning("FSMState ERROR: transition is already inside the map");
            return;
        }

        map.Add(transition, id);
        //Debug.Log("Added : " + transition + " with ID : " + id);
    }

    /// <summary>
    /// This method deletes a pair transition-state from this state�s map.
    /// If the transition was not inside the state�s map, an ERROR message is printed.
    /// </summary>
    public void DeleteTransition(Transition trans)
    {
        // Check for NullTransition
        if (trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return;
        }

        // Check if the pair is inside the map before deleting
        if (map.ContainsKey(trans))
        {
            map.Remove(trans);
            return;
        }
        Debug.LogError("FSMState ERROR: Transition passed was not on this State�s List");
    }


    /// <summary>
    /// This method returns the new state the FSM should be if
    ///    this state receives a transition  
    /// </summary>
    public FSMStateID GetOutputState(Transition trans)
    {
        // Check for NullTransition
        if (trans == Transition.None)
        {
            Debug.LogError("FSMState ERROR: NullTransition is not allowed");
            return FSMStateID.None;
        }

        // Check if the map has this transition
        if (map.ContainsKey(trans))
        {
            return map[trans];
        }

        Debug.LogWarning("FSMState WARNING: " + trans+ " Transition passed to the State was not on the list");
        return FSMStateID.None;
    }

    /// <summary>
    /// Decides if the state should transition to another on its list
    /// NPC is a reference to the npc tha is controlled by this class
    /// </summary>
    public abstract void Reason(Transform player, Transform npc);

    /// <summary>
    /// This method controls the behavior of the NPC in the game World.
    /// Every action, movement or communication the NPC does should be placed here
    /// NPC is a reference to the npc tha is controlled by this class
    /// </summary>
    public abstract void Act(Transform player, Transform npc);

    /// 
    /// Find the next semi-random patrol point
    /// not needed
    ///
    //public void FindNextPoint()
    //{
    //    //Debug.Log("Finding next point");
    //    int rndIndex = Random.Range(0, waypoints.Length);
    //    Vector3 rndPosition = Vector3.zero;
    //    destPos = waypoints[rndIndex].position + rndPosition;
    //}

    protected void MoveTowards(Transform moveable, Vector3 target)
    {
        //2. Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(target - moveable.position);
        moveable.rotation = Quaternion.Slerp(moveable.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //3. Go Forward
        moveable.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }
    /// <summary>
    /// Just here for legacy reasons in case navmesh breaks beyond recognition
    /// </summary>
    protected void MoveStraightTowards(Transform moveable, Vector3 target)
    {
        //2. Rotate to the target point
        Quaternion targetRotation = Quaternion.LookRotation(target - moveable.position);
        moveable.rotation = Quaternion.Slerp(moveable.rotation, targetRotation, Time.deltaTime * curRotSpeed);

        //3. Go Forward
        moveable.Translate(Vector3.forward * Time.deltaTime * curSpeed);
    }
    protected void RotateTurretTowards(Transform turret, Vector3 target)
    {
        Quaternion turretRotation = Quaternion.LookRotation(target - turret.position);
        turret.rotation = Quaternion.Slerp(turret.rotation, turretRotation, Time.deltaTime * 20); //magic number const I know
    }

    /// <summary>
    /// Check whether the next random position is the same as current tank position
    /// </summary>
    /// <param name="pos">position to check</param>
    protected bool IsInCurrentRange(Transform trans, Vector3 pos)
    {
        float xPos = Mathf.Abs(pos.x - trans.position.x);
        float zPos = Mathf.Abs(pos.z - trans.position.z);

        if (xPos <= 50 && zPos <= 50)
            return true;

        return false;
    }
}
