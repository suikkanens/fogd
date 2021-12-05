using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonShoot : MonoBehaviour
{
    public GameObject cannonballPrefab;
    public GameObject explosiveCannonballPrefab;
    public float speed = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetMouseButtonDown(0) && GetComponent<PlayerMovement>().cannonballCount > 0)
        {
            GameObject cannonball = Instantiate(cannonballPrefab, transform.position + transform.forward  + Vector3.up * 1.2f, Quaternion.identity);
            cannonball.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * speed;
            GetComponent<PlayerMovement>().AddCollectible(cannonball.tag, false);
        }

        if (Input.GetMouseButtonDown(1) && GetComponent<PlayerMovement>().expCannonballCount > 0)
        {
            GameObject cannonball = Instantiate(explosiveCannonballPrefab, transform.position + transform.forward + Vector3.up * 1.2f, Quaternion.identity);
            cannonball.GetComponent<Rigidbody>().velocity = Camera.main.transform.forward * speed;
            GetComponent<PlayerMovement>().AddCollectible(cannonball.tag, false);
        }
    }
}
