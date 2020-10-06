using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitCollider : MonoBehaviour
{
    public PauseMenu menu;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Checking exit collider with " + other.gameObject.name);
        if (other.CompareTag("Intel"))
        {
            menu.UpdateText("YOU WON!");
            menu.Pause();
        }
    }
}
