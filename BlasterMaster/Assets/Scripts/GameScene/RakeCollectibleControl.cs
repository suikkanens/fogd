using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RakeCollectibleControl : CollectibleControl
{
    //float _index;
    // Start is called before the first frame update
    void Start()
    {
    }

    void FixedUpdate()
    {
        //Vector3 desiredForward = Vector3.RotateTowards(transform.forward, Quaternion.Euler(0,90,0)* transform.forward, 5f * Time.deltaTime, 0f);
        //var rotation = Quaternion.LookRotation(desiredForward);
        base.WaveEffect();
        transform.Rotate(0f, 90f* Time.deltaTime, 0f, Space.World);
    }
}
