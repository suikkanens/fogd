using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform playerTransform;

    private Vector3 m_cameraOffset;

    [Range(0.01f, 1.0f)]
    public float smoothFactor = 0.5f;

    public bool rotateAroundPlayer = true;

    public float rotationSpeed = 5.0f;


    // Start is called before the first frame update
    void Start()
    {
        m_cameraOffset = transform.position - playerTransform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (rotateAroundPlayer)
        {
            Quaternion camTurnAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeed, Vector3.up);

            m_cameraOffset = camTurnAngle * m_cameraOffset;
        }

        Vector3 newPos = playerTransform.position + m_cameraOffset;

        transform.position = Vector3.Slerp(transform.position, newPos, smoothFactor);

        if (rotateAroundPlayer)
        {
            transform.LookAt(playerTransform);
        }
    }
}