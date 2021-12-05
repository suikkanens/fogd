using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonballCollectibleControl : MonoBehaviour
{
    float m_index;
    // Update is called once per frame
    void Update()
    {
        m_index += Time.deltaTime;
        float y = 0.005f * Mathf.Sin(Mathf.PI* m_index);
        transform.position = transform.position + transform.up * y;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerMovement>().AddCollectible(gameObject.tag, true);
            Destroy(gameObject);
        }
    }
}
