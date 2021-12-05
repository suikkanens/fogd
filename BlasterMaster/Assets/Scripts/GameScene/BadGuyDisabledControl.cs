using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BadGuyDisabledControl : MonoBehaviour
{
    public bool seenWhileDead;

    // Start is called before the first frame update
    void Start()
    {
        seenWhileDead = false;
    }

    public bool SeenWhileDead()
    {
        return seenWhileDead;
    }

    public void SetSeenWhileDead(bool value)
    {
        seenWhileDead = value;
    }
}
