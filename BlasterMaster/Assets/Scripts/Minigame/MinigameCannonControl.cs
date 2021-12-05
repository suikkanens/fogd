using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum MgFiringMode
{
    Normal,
    Explosive,
    Length
}

public class MinigameCannonControl : MonoBehaviour
{
    public float turnSpeed = 20f;
    public GameObject cannonballPrefab;
    [SerializeField]
    [Range(20f, 100f)]
    private float _projectileSpeed = 5f;
    private int _modeSelection;

    public GameObject sparkPrefab;

    private AudioSource _AudioSource;
    GameObject cannonCamera;
    Quaternion rot = Quaternion.identity;
    Transform turretRestrictor;

    Transform turret;
    private float _cannonballMass;

    float _horizontal;
    float _vertical;

    void Start()
    {
        _AudioSource = gameObject.GetComponent<AudioSource>();
        cannonCamera = transform.Find("CannonCamera").gameObject;
        Camera.main.gameObject.SetActive(false);

        rot = Quaternion.LookRotation(transform.up, cannonCamera.transform.up);
        cannonCamera.transform.position = transform.position - transform.up + transform.forward - transform.right;
        cannonCamera.transform.rotation = rot;
        turret = transform.GetChild(1);
        turretRestrictor = transform.Find("TurretRestrictor");
        turretRestrictor.position = turret.transform.position + turret.transform.up * 1.2f;
        _cannonballMass = cannonballPrefab.GetComponent<Rigidbody>().mass;
    }

    // Update is called once per frame
    void Update()
    {
        MoveInput();
        CheckFireInput();
    }

    void FixedUpdate()
    {
        bool hasHorizontalInput = !Mathf.Approximately(_horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(_vertical, 0f);

        float turretAngle = Mathf.Asin((turretRestrictor.position.y - turret.position.y) / 1.2f) * Mathf.Rad2Deg;

        if (hasVerticalInput)
        {
            if (turretAngle <= -15f && _vertical < 0f)
            {
                _vertical = 0f;
            }

            if (turretAngle >= 50f && _vertical > 0f)
            {
                _vertical = 0f;
            }

            turret.Rotate(_vertical * turnSpeed * Time.deltaTime, 0, 0);
        }

        if (hasHorizontalInput)
        {
            transform.Rotate(0, 0, _horizontal * turnSpeed * Time.deltaTime);
        }
    }

    void CheckFireInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shoot();
        }
    }

    void MoveInput()
    {
        _horizontal = Input.GetAxis("Mouse X");//Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Mouse Y"); //Input.GetAxis("Vertical");
    }

    void Shoot()
    {
        GameObject cannonball = Instantiate(cannonballPrefab, turretRestrictor.position, Quaternion.identity);

        cannonball.GetComponent<Rigidbody>().velocity = turret.transform.up * _projectileSpeed;
        Vector3 desiredForward = Vector3.RotateTowards(Vector3.forward, turret.up, 2 * Mathf.PI, 0f);
        Quaternion rotation = Quaternion.LookRotation(desiredForward);
        var sparks = Instantiate(sparkPrefab, turretRestrictor.position, rotation);
        _AudioSource.Play();
        Destroy(sparks, 1f);
    }

    void LateUpdate()
    {
        turretRestrictor.position = turret.transform.position + turret.transform.up * 1.2f;
        DrawTrajectory.Instance.ShowTrajectory(true);
        DrawTrajectory.Instance.UpdateTrajectory(turret.transform.up * _projectileSpeed, _cannonballMass, turretRestrictor.position);

    }
}
