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
            this.transform.localScale = this.transform.localScale * .1f;
        }
    }

    private void FixedUpdate()
    {
        if (target != null)
        {
            rb.position = target.position + Vector3.up * 1;
            rb.rotation = Quaternion.AngleAxis(15.0f * Time.deltaTime, Vector3.up) * rb.rotation;
        }
    }
}
