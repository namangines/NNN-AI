using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerTankController : MonoBehaviour
{
    public GameObject Bullet;
    public PauseMenu menu;
	
    private Transform Turret;
    private Transform bulletSpawnPoint;    
    private float curSpeed, targetSpeed, rotSpeed;
    private float turretRotSpeed = 10.0f;
    private float maxForwardSpeed = 250.0f;
    private float maxBackwardSpeed = -200.0f;

    //Bullet shooting rate
    protected float shootRate = 1.2f;
    protected float elapsedTime;

    //health bar
    public int health = 100;
    public Text healthBar;

    void Start()
    {
        //Tank Settings
        rotSpeed = 150.0f;

        //Get the turret of the tank
        Turret = gameObject.transform.GetChild(0).transform;
        bulletSpawnPoint = Turret.GetChild(0).transform;
    }

    void OnEndGame()
    {
        // Don't allow any more control changes when the game ends
        this.enabled = false;
    }

    void Update()
    {
        UpdateControl();
        UpdateWeapon();

        //player health so they die on 0 
        if(health <= 0)
        {
            health = 0;
            menu.UpdateText("YOU DIED");
            menu.Pause();
            this.gameObject.SetActive(false);
        }
        elapsedTime += Time.deltaTime;
    }
    
    void UpdateControl()
    {
        //AIMING WITH THE MOUSE
        // Generate a plane that intersects the transform's position with an upwards normal.
        Plane playerPlane = new Plane(Vector3.up, transform.position + new Vector3(0, 0, 0));

        // Generate a ray from the cursor position
        Ray RayCast = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Determine the point where the cursor ray intersects the plane.
        float HitDist;

        // If the ray is parallel to the plane, Raycast will return false.
        if (playerPlane.Raycast(RayCast, out HitDist))
        {
            // Get the point along the ray that hits the calculated distance.
            Vector3 RayHitPoint = RayCast.GetPoint(HitDist);

            Quaternion targetRotation = Quaternion.LookRotation(RayHitPoint - transform.position);
            Turret.transform.rotation = Quaternion.Slerp(Turret.transform.rotation, targetRotation, Time.deltaTime * turretRotSpeed);
        }

        if (Input.GetKey(KeyCode.W))
        {
            targetSpeed = maxForwardSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            targetSpeed = maxBackwardSpeed;
        }
        else
        {
            targetSpeed = 0;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -rotSpeed * Time.deltaTime, 0.0f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, rotSpeed * Time.deltaTime, 0.0f);
        }

        //Determine current speed
        curSpeed = Mathf.Lerp(curSpeed, targetSpeed, 7.0f * Time.deltaTime);
        transform.Translate(Vector3.forward * Time.deltaTime * curSpeed);    
    }

    void UpdateWeapon()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (elapsedTime >= shootRate)
            {
                //Reset the time
                elapsedTime = 0.0f;

                //Also Instantiate over the PhotonNetwork
                Instantiate(Bullet, bulletSpawnPoint.position, bulletSpawnPoint.rotation).layer = LayerMask.GetMask("Default");

            }
        }
    }


    //minus player health like the enemies 
    void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Debug.LogWarning("Player hit!!");
            health -= collision.gameObject.GetComponent<Bullet>().damage;
        }
    }

}