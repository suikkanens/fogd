using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectileControl : MonoBehaviour
{
    public GameObject projectileHitPrefab;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag != "BadGuy" )
        {
            if (col.gameObject.tag == "Player" || col.gameObject.tag == "PlayerHips" || col.gameObject.tag == "PlayerBodyPart")
            {
                GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().SetHitByProjectile();
            }
            var hit = Instantiate(projectileHitPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            Destroy(hit, 1f);
        }
    }
}
