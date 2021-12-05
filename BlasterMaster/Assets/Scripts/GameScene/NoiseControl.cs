using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void MakeNoise(Vector3 position, Vector3 alertPosition, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        int layerMask = 1 << 6;
        foreach (Collider c in colliders)
        {
            if (c.gameObject.tag == "BadGuy")
            {
                Vector3 direction = position - c.transform.position;
                Ray ray = new Ray(c.transform.position + Vector3.up, direction + Vector3.up);
                RaycastHit[] hits = Physics.RaycastAll(ray, direction.magnitude, layerMask);
                var noiseLevel = -Mathf.Log(direction.magnitude / radius);
                noiseLevel = (noiseLevel > 1f) ? 1f : noiseLevel;
                foreach (RaycastHit hit in hits)
                {
                        noiseLevel -= 0.3f;
                }
                if (noiseLevel > 0)
                {
                    c.gameObject.GetComponent<BadGuyControl>().SetAlert(alertPosition);
                }
            }
        }
    }
}
