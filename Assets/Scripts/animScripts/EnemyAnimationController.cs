using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
    public NavMeshAgent VelocityReader;
    private Animator animator;
    void Start()
    {
        animator = this.GetComponent<Animator>();
    }

    void Update()
    {
        //normalized
        animator.SetFloat("velocity", 
            VelocityReader.velocity.magnitude / VelocityReader.desiredVelocity.magnitude);
    }
}
