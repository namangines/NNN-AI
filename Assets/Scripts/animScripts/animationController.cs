using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animationController : MonoBehaviour
{

    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");

    }

    // Update is called once per frame
    void Update()
    {

        bool isrunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool wPressed = Input.GetKey("w");
        bool shiftPressed = Input.GetKey("left shift");

        //walking animator code
        if (!isWalking && wPressed)
        {
            animator.SetBool(isWalkingHash, true);
            
        }

        if (isWalking && !wPressed)
        {
            animator.SetBool(isWalkingHash, false);
        }

        //running animator code
        if(!isrunning && (wPressed && shiftPressed))
        {
            animator.SetBool(isRunningHash, true);
            
        }

        if (isrunning && (!wPressed || !shiftPressed))
        {
            animator.SetBool(isRunningHash, false);
        }

    }
}
