using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RakeControl : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "BadGuy")
        {
            other.gameObject.GetComponent<BadGuyControl>().SetHitByCannonball(true);
            transform.parent.RotateAround(transform.parent.Find("Pivot").position, transform.parent.forward, -90f);
            GetComponent<Collider>().enabled = false;
            Destroy(transform.parent.gameObject, 5f);
        }
    }

}
