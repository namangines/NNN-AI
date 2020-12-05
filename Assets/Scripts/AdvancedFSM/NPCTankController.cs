using UnityEngine;
using System.Collections.Generic;
using TankPathingSystem;
using UnityEngine.AI;

public class NPCTankController : AdvancedFSM
{
    public List<string> TankClasses;

    public GameObject Bullet;
    public GameObject fastBullet;

    public Light Sightlight;
    public Camera Sight;
    public Transform SightPoint;

    public Light AuraLight;

    public int health;
    public float timeSinceOffduty = 0.0f;
    public bool remainHidden = false;

    // We overwrite the deprecated built-in `rigidbody` variable.
    new private Rigidbody rigidbody;

    private NavMeshAgent navAgent;

    public List<Transform> Waypoints;
    public int currentWaypoint = 0;

    //Initialize the Finite state machine for the NPC tank
    protected override void Initialize()
    {
        health = 100;

        elapsedTime = 0.0f;
        shootRate = .5f;

        //Get the target enemy(Player)
        GameObject[] objPlayer = GameObject.FindGameObjectsWithTag("Player");
        if (objPlayer.Length > 1)
            Debug.LogError("More than one player object tagged");
        playerTransform = objPlayer[0].transform;

        //Get the rigidbody and nav mesh agent attached to this instance of tank NPC
        rigidbody = this.GetComponent<Rigidbody>();
        navAgent = this.GetComponent<NavMeshAgent>();

        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

        //Get the turret of the tank
        turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;


        EventManagerDel.StartListening("Sound Detected",
            delegate
            {
                SetTransition(Transition.SawPlayer);
            }
        );


        //Start Doing the Finite State Machine
        ConstructFSM();

        if (remainHidden)
            SetTransition(Transition.WantsToHide);
    }

    //Update each frame
    protected override void FSMUpdate()
    {
        //Check for health
        elapsedTime += Time.deltaTime;
    }

    protected override void FSMFixedUpdate()
    {
        CurrentState.Reason(playerTransform, transform);
        CurrentState.Act(playerTransform, transform);
    }

    public void SetTransition(Transition t)
    {
        PerformTransition(t);
    }

    private void ConstructFSM()
    {

        PatrolState patrol = new PatrolState(this);
        patrol.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        patrol.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        patrol.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        patrol.AddTransition(Transition.GotBored, FSMStateID.Bored);
        patrol.AddTransition(Transition.WantsTimeOff, FSMStateID.OffDuty);
        patrol.AddTransition(Transition.Hurt, FSMStateID.Repairing);
        patrol.AddTransition(Transition.WantsToHide, FSMStateID.Hiding);

        ChaseState chase = new ChaseState(this);
        chase.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        chase.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        chase.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        chase.AddTransition(Transition.Hurt, FSMStateID.Repairing);

        AttackState attack = new AttackState(this);
        attack.AddTransition(Transition.LostPlayer, FSMStateID.Patrolling);
        attack.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        attack.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        attack.AddTransition(Transition.Hurt, FSMStateID.Repairing);

        OffDutyState offduty = new OffDutyState(rigidbody.transform, this);
        offduty.AddTransition(Transition.SawPlayer, FSMStateID.Chasing);
        offduty.AddTransition(Transition.RestedLongEnough, FSMStateID.Patrolling);
        offduty.AddTransition(Transition.NoHealth, FSMStateID.Dead);
        offduty.AddTransition(Transition.WantsToHide, FSMStateID.Hiding);

        RepairState repair = new RepairState(this);
        repair.AddTransition(Transition.Healed, FSMStateID.Patrolling);
        repair.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        HideState hide = new HideState(this);
        hide.AddTransition(Transition.ReachPlayer, FSMStateID.Attacking);
        hide.AddTransition(Transition.RestedLongEnough, FSMStateID.Patrolling);

        DeadState dead = new DeadState();
        dead.AddTransition(Transition.NoHealth, FSMStateID.Dead);


        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(attack);
        AddFSMState(offduty);
        AddFSMState(repair);
        AddFSMState(hide);
        AddFSMState(dead);
    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter(Collider collision)
    {
        //Reduce health
        if (collision.gameObject.CompareTag("Bullet"))
        {
            health -= collision.gameObject.GetComponent<Bullet>().damage;

            if (health <= 0)
            {
                Debug.Log("Switch to Dead State");
                SetTransition(Transition.NoHealth);
                TankDutyManager.Instance.TankHasDied(this);
                Explode();
            }
            else
            {
                SetTransition(Transition.SawPlayer); //If they've been hit they shouldn't just sit and take it
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SetTransition(Transition.SawPlayer);
        }
    }

    protected void Explode()
    {
        float rndX = Random.Range(10.0f, 30.0f);
        float rndZ = Random.Range(10.0f, 30.0f);
        for (int i = 0; i < 3; i++)
        {
            rigidbody.AddExplosionForce(10000.0f, transform.position - new Vector3(rndX, 10.0f, rndZ), 40.0f, 10.0f);
            rigidbody.velocity = transform.TransformDirection(new Vector3(rndX, 20.0f, rndZ));
        }

        Destroy(gameObject, 1.5f);
    }

    /// <summary>
    /// Shoots a bullet. Has a chance to shoot 'fast' 
    /// </summary>
    public void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            //These could be inspector values but for now I just put the chance here
            float odds = .15f;

            if (TankClasses.Contains("Fast"))
                odds = .85f;

            if (Random.value < odds)
                shootFast();
            else
                shootNormal();
        }
    }
    private void shootNormal()
    {
        Debug.Log("Shoot normal");
        Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<Bullet>().LifeTime = 4f;
        elapsedTime = 0.0f;
    }
    private void shootFast()
    {
        Debug.Log("Shoot fast");
        Instantiate(fastBullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<Bullet>().LifeTime = 4f;
        Quaternion angle = Quaternion.AngleAxis(5, Vector3.up);
        Instantiate(fastBullet, bulletSpawnPoint.position, angle * bulletSpawnPoint.rotation).GetComponent<Bullet>().LifeTime = 4f;
        angle = Quaternion.AngleAxis(-5, Vector3.up);
        Instantiate(fastBullet, bulletSpawnPoint.position, angle * bulletSpawnPoint.rotation).GetComponent<Bullet>().LifeTime = 4f;
        elapsedTime = 0.25f; //So the next shot is even faster
    }

    public void ChangeLightColor(Color color)
    {
        Sightlight.color = color;
        if (TankClasses.Contains("Normal"))
        {
            AuraLight.color = color;
        }
    }

    public void AllLightOn()
    {
        SightLightOn();
        AuraLightOn();
    }
    public void AllLightOff()
    {
        SightLightOff();
        AuraLightOff();
    }
    public void SightLightOn()
    {
        Sightlight.enabled = true;
    }
    public void SightLightOff()
    {
        Sightlight.enabled = false;
    }
    public void AuraLightOn()
    {
        AuraLight.enabled = true;
    }
    public void AuraLightOff()
    {
        AuraLight.enabled = false;
    }

    public bool IsInsideSightFrustrum(Collider collider)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Sight);
        return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
    }

    public bool HasLineOfSight(Collider target)
    {
        RaycastHit hit;
        if (Physics.Raycast(SightPoint.position, target.transform.position - SightPoint.position, out hit, Sight.farClipPlane, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal))
        {
            if (target == hit.collider)
            {
                return true;
            }
        }
        return false;
    }
    public bool HasLineOfSight(Transform target)
    {
        RaycastHit hit;
        if (Physics.Raycast(SightPoint.position, target.position - SightPoint.position, out hit, Sight.farClipPlane, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal))
        {
            if (target == hit.transform)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// Works slightly differently to the others: instead of checking if you hit a provided target, this checks to see if you can reach the position or not without hitting another object aside from 'exclude'
    /// </summary>
    /// <param name="exclude">The transform to exclude from the raycast search: usually the target or something close to the target: ie, finding out if the last known vector3 position of the player is within sight</param>s
    public bool HasLineOfSight(Vector3 target, Transform exclude = null)
    {
        RaycastHit hit;
        if (Physics.Raycast(SightPoint.position, target - SightPoint.position, out hit, Sight.farClipPlane, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal))
        {
            if (hit.transform != exclude)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Navigates using the NavMesh system
    /// </summary>
    /// <param name="position"></param>
    public void NavigateToPosition(Vector3 position)
    {
        if (position == Vector3.zero)
            navAgent.ResetPath();
        navAgent.destination = position;
    }
    public void NavigateToPosition(Transform transform)
    {
        NavigateToPosition(transform.position);
    }
}
