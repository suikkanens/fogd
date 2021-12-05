using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleControl : MonoBehaviour
{
    float m_index;
    // Update is called once per frame
    void FixedUpdate()
    {
        WaveEffect();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerMovement>().AddCollectible(gameObject.tag, true);
            Destroy(gameObject);
        }
    }

    protected void WaveEffect()
    {
        m_index += Time.deltaTime;
        float y = 0.01f * Mathf.Sin(Mathf.PI * m_index);
        transform.position = transform.position + Vector3.up * y;
    }
}
