using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerIfHit : MonoBehaviour
{
    Rigidbody rb;
    Transform target;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            target = other.transform;
        }
    }

    private void Update()
    {
        if(target != null)
            rb.position = target.position + Vector3.up*200;
    }
}
