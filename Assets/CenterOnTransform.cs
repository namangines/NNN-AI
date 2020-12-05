using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterOnTransform : MonoBehaviour
{
    public Transform Target;

    public float VerticalOffset;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Target.position + Vector3.up * VerticalOffset;

        this.transform.rotation = Target.rotation;
    }
}
