using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnterVehicle : MonoBehaviour
{
    public bool _inVehicle = false;
    public GameObject guiObj;
    public float turnSpeed = 20f;
    public GameObject[] cannonballPrefabs;
    [SerializeField]
    [Range(1f, 50f)]
    private float projectileSpeed = 5f;
    public float projectileForce = 2000f;
    public GameObject mainCamera;

    public GameObject sparkPrefab;

    CannonControl cannonScript;
    GameObject player;
    PlayerMovement _playerScript;
    Rigidbody rigidBody;
    GameObject cannonCamera;
    Quaternion rot = Quaternion.identity;
    Transform turretRestrictor;

    Transform turret;

    Vector3 forceVector;
    float cannonballMass;

    float _horizontal;
    float _vertical;
    float _enterCooldown;

    void Start()
    {
        cannonScript = GetComponent<CannonControl>();
        cannonScript.enabled = false;
        player = GameObject.FindWithTag("Player");
        _playerScript = player.GetComponent<PlayerMovement>();
        rigidBody = player.GetComponent<Rigidbody>();
        cannonCamera = transform.Find("CannonCamera").gameObject;

        rot = Quaternion.LookRotation(transform.up, cannonCamera.transform.up);
        cannonCamera.transform.position = transform.position - transform.up + transform.forward - transform.right;
        cannonCamera.transform.rotation = rot;
        cannonCamera.SetActive(false);
        guiObj.SetActive(false);
        turret = transform.GetChild(1);
        turretRestrictor = transform.Find("TurretRestrictor");
        turretRestrictor.position = turret.transform.position + turret.transform.up * 1.2f;
        forceVector = turret.up;
        cannonballMass = cannonballPrefabs[0].GetComponent<Rigidbody>().mass;
        _enterCooldown = 0f;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && _inVehicle == false && _enterCooldown <= Time.time)
        {
            guiObj.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                guiObj.SetActive(false);
                SetPlayerTransform(false);
                _enterCooldown = Time.time + 0.5f;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            guiObj.SetActive(false);
        }
    }

    void MoveInput()
    {
        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
    }

    void Update()
    {
        MoveInput();
        if (_inVehicle && Input.GetKeyDown(KeyCode.E) && _enterCooldown <= Time.time)
        {
            SetPlayerTransform(true);
            _enterCooldown = Time.time + 0.5f;
        }

        if (_inVehicle && cannonScript.PlayerHasFired())
        {
            SetPlayerTransform(true);
            _enterCooldown = Time.time + 0.5f;

            StartCoroutine(player.GetComponent<PlayerMovement>().LaunchRagdoll(turret.up, projectileSpeed));

            cannonScript.SetHasFired(false);
            cannonScript.enabled = false;
        }

        if (_inVehicle && player.GetComponent<PlayerMovement>().IsDead())
        {
            SetPlayerTransform(true);
        }
    }

    void FixedUpdate()
    {
        float turretAngle = Mathf.Asin((turretRestrictor.position.y - turret.position.y) / 1.2f) * Mathf.Rad2Deg;
        bool hasHorizontalInput = !Mathf.Approximately(_horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(_vertical, 0f);

        if (_inVehicle && hasVerticalInput)
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

        if (_inVehicle && hasHorizontalInput)
        {
            transform.Rotate(0, 0, _horizontal * turnSpeed * Time.deltaTime);
        }

        int firingMode = cannonScript.ProjectileHasFired();

        if (_inVehicle && firingMode >= 0)
        {
            GameObject cannonball = Instantiate(cannonballPrefabs[firingMode], turretRestrictor.position, Quaternion.identity);
            if (firingMode == 0)
            {
                cannonball.GetComponent<CannonballControl>().SetTurretPosition(transform.position);
            }
            else
            {
                cannonball.GetComponent<ExplosiveCannonballControl>().SetTurretPosition(transform.position);
            }

            cannonball.GetComponent<Rigidbody>().velocity = turret.transform.up * projectileSpeed;
            _playerScript.AddCollectible(cannonball.tag, false);

            Vector3 desiredForward = Vector3.RotateTowards(Vector3.forward, turret.up, 2 * Mathf.PI, 0f);
            Quaternion rotation = Quaternion.LookRotation(desiredForward);

            var sparks = Instantiate(sparkPrefab, turretRestrictor.position, rotation);

            cannonScript.SetHasFired(false);
            Destroy(sparks, 1f);
        }
        
    }

    void SetPlayerTransform(bool value)
    {
        mainCamera.SetActive(value);
        player.transform.Find("MaleDummy_Mesh").gameObject.SetActive(value);
        player.transform.parent = (value) ? null : gameObject.transform;
        _inVehicle = !value;
        cannonScript.enabled = !value;
        player.GetComponent<PlayerMovement>().SetIsInVehicle(_inVehicle);
        cannonCamera.SetActive(!value);
    }

    void LateUpdate()
    {
        turretRestrictor.position = turret.transform.position + turret.transform.up * 1.2f;
        if (_inVehicle)
        {
            DrawTrajectory.Instance.ShowTrajectory(true);
            DrawTrajectory.Instance.UpdateTrajectory(turret.transform.up * projectileSpeed, cannonballMass, turretRestrictor.position);
        }
        
    }

}
