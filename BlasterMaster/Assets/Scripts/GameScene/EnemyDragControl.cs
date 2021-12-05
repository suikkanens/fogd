using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDragControl : MonoBehaviour
{
    GameObject m_interactionTrigger;
    List<string> colliderTags = new List<string>() { "LeftFoot", "RightFoot", "LeftArm", "RightArm" };
    GameObject selectedLimb;
    GameObject attachedLimb;
    float shortestDistance;
    float m_cooldown;
    GameObject playerHand;
    SpringJoint dragJoint;

    // Start is called before the first frame update
    void Start()
    {
        m_interactionTrigger = transform.Find("PlayerInteractionTrigger").gameObject;
        playerHand = GameObject.FindGameObjectsWithTag("PlayerRightHand")[0];
        dragJoint = playerHand.GetComponent<SpringJoint>();
    }

    // Update is called once per frame
    void Update()
    {
        if (selectedLimb && !dragJoint.connectedBody)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log(selectedLimb.tag + " connected");
                attachedLimb = selectedLimb;
                dragJoint.connectedBody = selectedLimb.GetComponent<Rigidbody>();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q) && dragJoint.connectedBody)
        {
            Debug.Log(attachedLimb.tag + " detached");
            dragJoint.connectedBody = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 1);
        selectedLimb = null;
        shortestDistance = 100f;
        foreach (Collider col in colliders)
        {
            if (colliderTags.Contains(col.gameObject.tag))
            {
                var dist = (col.transform.position - transform.position).magnitude;
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    selectedLimb = col.gameObject;
                }
            }
        }

        
    }
}
