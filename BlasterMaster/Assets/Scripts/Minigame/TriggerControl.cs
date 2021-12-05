using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerControl : MonoBehaviour
{
    Collider _trigger;
    int _direction;
    ConveyorBeltControl _beltControl;
    // Start is called before the first frame update
    void Start()
    {
        _trigger = GetComponent<Collider>();
        _direction = (transform.position.x <= 0) ? 1 : -1;
        _beltControl = transform.parent.gameObject.GetComponent<ConveyorBeltControl>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    void OnTriggerEnter(Collider obj)
    {
        if (obj.gameObject.tag == "MinigameBadGuy")
        {
            _beltControl.SetRowDirection();
        }
    }
}
