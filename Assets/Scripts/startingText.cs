using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class startingText : MonoBehaviour
{
    //tankInRange bool value to tell if the player is in the trigger zone
    public bool tankInRange = false;

    //variables for the starting text 
    public GameObject dialogBox;
    public Text dialogText;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (tankInRange)
        {
            dialogBox.SetActive(true);
        }
    }

    //trigger enter and exit for the starting text
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tankInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tankInRange = false;
            dialogBox.SetActive(false);
        }
    }
}
