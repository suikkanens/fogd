using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ExplosiveCannonballControl : MonoBehaviour
{
    public float radius = 5.0f;
    public float power = 10.0f;
    public GameObject explosionPrefab;
    
    private NoiseControl _noiseScript;
    private bool _hasExploded = false;
    private Vector3 _turretPos;

    Vector3 _direction;

    void Start()
    {
        _noiseScript = GetComponent<NoiseControl>();
        _direction = new Vector3(0, 0, 0);
    }

    void OnCollisionEnter(Collision col)
    {
        if (!_hasExploded)
        {
            _hasExploded = true;
            StartCoroutine(EnemyHit());
        }
    }

    IEnumerator EnemyHit()
    {
        Vector3 explosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        var ps = explosionPrefab.GetComponentsInChildren<ParticleSystem>();

        var explosion = Instantiate(explosionPrefab, explosionPos, Quaternion.identity);

        foreach (Collider hit in colliders)
        {
            if (hit.gameObject.tag == "BadGuy" || hit.gameObject.tag == "MinigameBadGuy")
            {
                var layerMask = 1 << 6;
                _direction = hit.transform.position + Vector3.up - transform.position;
                bool wallHit = Physics.Raycast(new Ray(transform.position, _direction), _direction.magnitude, layerMask);

                //bool wallHit = false;
                //foreach (RaycastHit rcHit in hits)
                //{
                //    //Debug.Log(hit.collider.tag);
                //    if (rcHit.collider.tag == "SoundBarrier")
                //    {
                //        wallHit = true;
                //        break;
                //    }
                //}

                if(!wallHit)
                {
                    if (hit.gameObject.tag == "BadGuy")
                    {
                        hit.gameObject.GetComponent<BadGuyControl>().SetHitByCannonball(true);
                    }
                    else if (hit.gameObject.tag == "MinigameBadGuy")
                    {
                        hit.gameObject.GetComponent<MinigameEnemyControl>().SetHitByCannonball(true);
                    }
                }
            }
        }

        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        foreach (var p in ps)
        {
            p.Play();
        }

        colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddExplosionForce(power, explosionPos, radius, 3.0f);
            }

        }

        Destroy(gameObject);
        Destroy(explosion, 1f);

        try
        {
            _noiseScript.MakeNoise(explosionPos, _turretPos, 40f);
        }
        catch(Exception e)
        {

        }
        
    }

    public void SetTurretPosition(Vector3 pos)
    {
        _turretPos = pos;
    }
}
