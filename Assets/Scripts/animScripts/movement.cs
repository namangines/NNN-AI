using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movement : MonoBehaviour
{

    public float walkSpeed = 10f;
    public float runSpeed = 30f;

    private CharacterController controller;

    public Transform robot;
    Rigidbody m_rigidbody;
    

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //tranform forward with walking speed and run speed if shift and w are pressed 
        if (Input.GetKey("w"))
        {
            m_rigidbody.velocity = transform.forward * walkSpeed;
        }
        //stop moving when not pressing
        if (!Input.GetKey("w"))
        {
            this.m_rigidbody.velocity = new Vector3(0, 0, 0);
        }

        if (Input.GetKey("right shift") && Input.GetKey("w"))
        {
            m_rigidbody.velocity = transform.forward * runSpeed;
        }
        //stop running when not pressing
        if (!Input.GetKey("w") && !Input.GetKey("right shift"))
        {
            this.m_rigidbody.velocity = new Vector3(0, 0, 0);
        }

        //rotaion code
        //positive rotation 
        if (Input.GetKey("a"))
        {
            transform.Rotate(new Vector3(0, 1, 0) * Time.deltaTime * runSpeed, Space.World);
        }

        //negative rotaion
        if (Input.GetKey("d"))
        {
            transform.Rotate(new Vector3(0, -1, 0) * Time.deltaTime * runSpeed, Space.World);
        }


    }
}
