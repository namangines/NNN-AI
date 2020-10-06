using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyDoorMover : MonoBehaviour
{
    public Rigidbody Door;
    public Transform DownPosition, UpPosition;
    private List<hittype> hit = new List<hittype>();

    private enum hittype
    {
        player, enemy, other
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            hit.Add(hittype.enemy);
        }
        else if (other.CompareTag("Player"))
        {
            hit.Add(hittype.player);
        }
        else
        {
            hit.Add(hittype.other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            hit.Remove(hittype.enemy);
        }
        else if (other.CompareTag("Player"))
        {
            hit.Remove(hittype.player);
        }
        else
        {
            hit.Remove(hittype.other);
        }
    }


    private void FixedUpdate()
    {
        bool hitenemy = false, hitplayer = false;
        foreach(hittype t in hit)
        {
            if (t == hittype.enemy)
                hitenemy = true;
            if (t == hittype.player)
                hitplayer = true;
        }

        float changeby = .1f;
        if (hitenemy && !hitplayer)
            Door.position = Vector3.Lerp(Door.position, UpPosition.position, changeby);
        else
            Door.position = Vector3.Lerp(Door.position, DownPosition.position, changeby*2);
    }

    /*private void Update()
    {
        StartCoroutine(EndOfFrame());
    }

    private IEnumerator EndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        hit.Clear();
    }*/
}
