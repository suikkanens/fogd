using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDestroyTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider obj)
    {
        Debug.Log(obj.gameObject.tag);
        if (obj.gameObject.tag == "MinigameBadGuy")
        {
            Destroy(obj.gameObject);
        }
    }
}
