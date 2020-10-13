using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    //Explosion Effect
    public GameObject Explosion;

    public float Speed = 600.0f;
    public float LifeTime = 3.0f;
    public int damage = 49;

    void Start()
    {
        Destroy(gameObject, LifeTime);
    }

    void Update()
    {
        //The bullet by default slightly shoots downwards because the turret itself points downwards towards the player's center
        //the crossing here just makes sure the bullet actually shoots properly forwards
        Vector3 forwards = transform.forward;
        Vector3 right = Vector3.Cross(forwards, Vector3.down);
        Vector3 relativeforwards = Vector3.Cross(right, Vector3.up);
        transform.position += 
			relativeforwards * Speed * Time.deltaTime;       
    }

    void OnTriggerEnter(Collider collision)
    {
        //Instantiate(Explosion, contact.point, Quaternion.identity);
        if(!collision.gameObject.CompareTag("Bullet"))
            Destroy(gameObject);
    }
}