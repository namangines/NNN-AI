using UnityEngine;
using System.Collections.Generic;
using TankPathingSystem;

public class NPCTankController : AdvancedFSM
{
    //public Waypoint destWaypoint;
    //public Stack<Waypoint> destPath = new Stack<Waypoint>();
    public GameObject Bullet;
    public Light Sightlight;
    public Camera Sight;
    public Transform SightPoint;

    public int health;
    public float timeSinceOffduty = 0.0f;

    // We overwrite the deprecated built-in `rigidbody` variable.
    new private Rigidbody rigidbody;

    //Initialize the Finite state machine for the NPC tank
    protected override void Initialize()
    {
        health = 100;

        elapsedTime = 0.0f;
        shootRate = .5f;

        //Get the target enemy(Player)
        GameObject objPlayer = GameObject.FindGameObjectWithTag("Player");
        playerTransform = objPlayer.transform;

        //Get the rigidbody
        rigidbody = GetComponent<Rigidbody>();

        if (!playerTransform)
            print("Player doesn't exist.. Please add one with Tag named 'Player'");

        //Get the turret of the tank
        turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = turret.GetChild(0).transform;

        //Start Doing the Finite State Machine
        ConstructFSM();
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

        RepairState repair = new RepairState(this);
        repair.AddTransition(Transition.Healed, FSMStateID.Patrolling);
        repair.AddTransition(Transition.NoHealth, FSMStateID.Dead);

        DeadState dead = new DeadState();
        dead.AddTransition(Transition.NoHealth, FSMStateID.Dead);


        AddFSMState(patrol);
        AddFSMState(chase);
        AddFSMState(attack);
        AddFSMState(offduty);
        AddFSMState(repair);
        AddFSMState(dead);
    }

    /// <summary>
    /// Check the collision with the bullet
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter(Collider collision)
    {
        //Reduce health
        if (collision.gameObject.tag == "Bullet")
        {
            health -= collision.gameObject.GetComponent<Bullet>().damage; ;

            if (health <= 0)
            {
                Debug.Log("Switch to Dead State");
                SetTransition(Transition.NoHealth);
                Explode();
            }
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
    /// Shoot the bullet from the turret
    /// </summary>
    public void ShootBullet()
    {
        if (elapsedTime >= shootRate)
        {
            Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation).GetComponent<Bullet>().LifeTime = 4f;
            elapsedTime = 0.0f;
        }
    }

    public void ChangeLightColor(Color color)
    {
        Sightlight.color = color;
    }

    public bool IsInsideSightFrustrum(Collider collider)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Sight);
        return GeometryUtility.TestPlanesAABB(planes, collider.bounds);
    }

    public bool HasLineOfSight(Collider target)
    {
        RaycastHit hit;
        if (Physics.Raycast(SightPoint.position, target.transform.position - SightPoint.position, out hit, Sight.farClipPlane, LayerMask.GetMask("Default"), QueryTriggerInteraction.UseGlobal))
        {
            if(target == hit.collider)
            {
                return true;
            }
        }

        return false;
    }
}
