using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

enum InventoryType
{
    Rake,
    Cannonball,
    ExplosiveCannonball,
    Collectible
}

public class PlayerMovement : MonoBehaviour
{
    public float turnSpeed = 40f;
    public float jumpRate = 1.0f;

    public int cannonballCount = 20;
    public int expCannonballCount = 20;
    //public int collectibleCount = 0;
    //public int rakeCount = 10;

    Dictionary<string, int> collectibles = new Dictionary<string, int>();

    public TextMeshProUGUI[] inventoryGui;
    public TextMeshProUGUI spawnText;
    public GameObject badGuyTerminationText;

    public GameObject rakePrefab;

    public AudioClip walkAudio;
    public AudioClip runAudio;
    public AudioClip sprintAudio;
    public AudioClip jumpAudio;

    Animator _animator;
    Rigidbody _rigidBody;
    AudioSource _audioSource;
    Vector3 _movement;
    Quaternion _rotation = Quaternion.identity;
    GameObject _cameraDir;
    Vector3 _cameraOffset;
    public bool _isGrounded;
    float _distToGround;
    float _nextJump;
    NoiseControl _noiseScript;
    Collider _interactionTrigger;
    GameObject _playerHips;


    public bool _hitByProjectile;
    bool _isPlayerDead;
    bool _allowRespawn;
    bool _isRagdoll;
    bool _isInVehicle;
    [SerializeField]
    Transform _spawn;

    RaycastHit _hit;
    bool _hitDetect;

    bool _playerSeen;
    bool _triggerEnemyDead;
    bool _inTriggerRange;
    GameObject _triggerEnemy;
    bool _tTextCoRunning;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        _audioSource = GetComponent<AudioSource>();
        _cameraDir = GameObject.FindWithTag("CameraLookAt");
        _cameraOffset = _cameraDir.transform.position - transform.Find("Dummy").transform.position;
        _interactionTrigger = transform.Find("PlayerInteractionTrigger").gameObject.GetComponent<Collider>();
        SetKinematic(true);
        _playerHips = GameObject.FindWithTag("PlayerHips");

        _hitByProjectile = false;
        _isPlayerDead = false;
        _allowRespawn = false;
        _isRagdoll = false;
        _isInVehicle = false;

        _playerSeen = false;
        _triggerEnemyDead = false;
        _inTriggerRange = false;
        _tTextCoRunning = false;

        _distToGround = GetComponent<Collider>().bounds.extents.y;
        _nextJump = Time.time;

        _noiseScript = GetComponent<NoiseControl>();

        collectibles.Add("Rake", 5);
        collectibles.Add("Cannonball", 5);
        collectibles.Add("ExpCannonball", 1);
        collectibles.Add("Collectible", 0);

        inventoryGui[(int)InventoryType.Rake].text = "Rakes: " + collectibles["Rake"].ToString();
        inventoryGui[(int)InventoryType.Cannonball].text = "Cannonballs: " + collectibles["Cannonball"].ToString();
        inventoryGui[(int)InventoryType.ExplosiveCannonball].text = "ExCannonballs: " + collectibles["ExpCannonball"].ToString();
        inventoryGui[(int)InventoryType.Collectible].text = "Collectibles: " + collectibles["Collectible"].ToString();

        badGuyTerminationText.SetActive(false);
        spawnText.enabled = false;
    }

    void FixedUpdate()
    {
        if (transform.position.y <= -10f)
        {
            transform.position = _spawn.position + Vector3.up;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_inTriggerRange && Input.GetKeyDown(KeyCode.F) && _tTextCoRunning)
        {
            _triggerEnemy.transform.parent.GetComponent<BadGuyControl>().SetHitByCannonball(true);
            //badGuyTerminationText.SetActive(false);
        }
        CheckForHits();
        if (IsGrounded() && Input.GetKeyDown(KeyCode.R))
        {
            SetRake();
        }

        if (!_isInVehicle)
        {
            Movement();
        }
        else
        {
            _animator.SetBool("IsWalking", false);
            _animator.SetBool("IsRunning", false);
            _animator.SetBool("IsJumping", false);
            _animator.SetBool("IsInAir", false);
            _animator.SetFloat("VelocityY", _rigidBody.velocity.y);
            _animator.SetBool("IsGrounded", true);
            _animator.SetBool("IsSneaking", false);
            _animator.SetBool("IsDragging", false);
        }
    }

    void CheckForHits()
    {
        _allowRespawn = false;
        if (_hitByProjectile)
        {
            _hitByProjectile = false;
            StartCoroutine(DieRespawnCycle());
        }
        else if ((_isPlayerDead || _isRagdoll) && !_hitByProjectile)
        {
            _allowRespawn = (_allowRespawn || Input.GetKeyDown(KeyCode.Space));
        }
    }

    void Movement()
    {
        _isGrounded = IsGrounded();

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);

        bool isWalking = (hasHorizontalInput || hasVerticalInput) && IsGrounded();

        bool isRunning = (isWalking && Input.GetKey(KeyCode.LeftShift));

        bool isDragging = (GameObject.FindGameObjectsWithTag("PlayerRightHand")[0].GetComponent<SpringJoint>().connectedBody != null);

        bool isSneaking = (isWalking && !isRunning && (isDragging || Input.GetKey(KeyCode.LeftControl)));

        bool isInAir;

        bool isJumping;

        if (IsGrounded() && Input.GetKeyDown(KeyCode.Space) && Time.time > _nextJump && !isDragging)
        {
            isJumping = true;
            _nextJump = Time.time + jumpRate;
        }
        else
        {
            isJumping = false;
        }

        Vector3 camF = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 camR = Camera.main.transform.right;

        _movement = camF * vertical + camR * horizontal + transform.up * System.Convert.ToSingle(isJumping);
        _movement.Normalize();

        if (isJumping)
        {
            _rigidBody.AddForce(_movement * 400.0f, ForceMode.Impulse);
        }

        isInAir = (!IsGrounded() && !Mathf.Approximately(_rigidBody.velocity.y, 0f) || isJumping);

        _animator.SetBool("IsWalking", isWalking);
        _animator.SetBool("IsRunning", isRunning);
        _animator.SetBool("IsJumping", isJumping);
        _animator.SetBool("IsInAir", isInAir);
        _animator.SetFloat("VelocityY", _rigidBody.velocity.y);
        _animator.SetBool("IsGrounded", IsGrounded());
        _animator.SetBool("IsSneaking", isSneaking);
        _animator.SetBool("IsDragging", isDragging);

        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, Vector3.Scale(_movement, new Vector3(1, 0, 1)).normalized, turnSpeed * Time.deltaTime, 0f);
        _rotation = Quaternion.LookRotation(desiredForward);
    }

    bool IsGrounded()
    {
        Vector3 center = new Vector3(GetComponent<CapsuleCollider>().bounds.center.x, GetComponent<CapsuleCollider>().bounds.min.y - 0.1f, GetComponent<CapsuleCollider>().bounds.center.z);
        Collider[] colliders = Physics.OverlapBox(center, new Vector3(GetComponent<CapsuleCollider>().radius * 2, 0.2f, GetComponent<CapsuleCollider>().radius * 2));
        bool hit = false;

        foreach (Collider c in colliders)
        {
            if(c.gameObject.tag != "Player" && c.gameObject.tag != "PlayerInteractionTrigger")
            {
                hit = true;
            }
        }
        return hit;

    }

    void OnDrawGizmosSelected()
    {
        Vector3 center = new Vector3(GetComponent<CapsuleCollider>().bounds.center.x, GetComponent<CapsuleCollider>().bounds.min.y - 0.1f, GetComponent<CapsuleCollider>().bounds.center.z);
        Gizmos.DrawWireCube(center, new Vector3(GetComponent<CapsuleCollider>().radius * 2, 0.2f, GetComponent<CapsuleCollider>().radius * 2));

        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down*2f);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            Gizmos.DrawRay(transform.position + Vector3.up*3f, hit.normal);
        }
    }

    void OnAnimatorMove()
    {
        var currentClip = _audioSource.clip;
        _rigidBody.MovePosition(_rigidBody.position + _movement * _animator.deltaPosition.magnitude);
        _rigidBody.MoveRotation(_rotation);
        OutputNoise();

    }

    void LateUpdate()
    {
        _cameraDir.transform.position = _playerHips.transform.position + _cameraOffset;
        inventoryGui[(int)InventoryType.Rake].text = "Rakes: " + collectibles["Rake"].ToString();
        inventoryGui[(int)InventoryType.Cannonball].text = "Cannonballs: " + collectibles["Cannonball"].ToString();
        inventoryGui[(int)InventoryType.ExplosiveCannonball].text = "ExCannonballs: " + collectibles["ExpCannonball"].ToString();
        inventoryGui[(int)InventoryType.Collectible].text = "Collectibles: " + collectibles["Collectible"].ToString();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "SneakAttackTrigger")
        {
            _playerSeen = other.gameObject.GetComponent<SneakControl>().IsPlayerSeen();
            _triggerEnemyDead = !other.gameObject.GetComponent<SneakControl>().IsParentEnabled();
            _triggerEnemy = other.gameObject;
            _inTriggerRange = true;

            if (!_tTextCoRunning && !_playerSeen && !_triggerEnemyDead)
            {
                StartCoroutine(SetTerminationText());
            }

            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "SneakAttackTrigger")
        {
            _triggerEnemy = null;
            _inTriggerRange = false;
        }
    }

    IEnumerator SetTerminationText()
    {
        _tTextCoRunning = true;
        badGuyTerminationText.SetActive(true);
        //Debug.Log(_playerSeen + ", " + _triggerEnemyDead + ", " + !_inTriggerRange);
        yield return new WaitUntil(() => (_playerSeen || _triggerEnemyDead || !_inTriggerRange));
        badGuyTerminationText.SetActive(false);
        _tTextCoRunning = false;
    }

    void OutputNoise()
    {
        var animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (animatorStateInfo.IsName("BasicMotions@Run01 - Forwards [RM]"))
        {
            _audioSource.clip = runAudio;
            _noiseScript.MakeNoise(transform.position, transform.position, 5f);
        }
        else if (animatorStateInfo.IsName("BasicMotions@Sprint01 - Forwards [RM]"))
        {
            _audioSource.clip = sprintAudio;
            _noiseScript.MakeNoise(transform.position, transform.position, 10f);
        }
        else if (animatorStateInfo.IsName("BasicMotions@Jump01 - Land"))
        {
            _audioSource.clip = jumpAudio;
            _noiseScript.MakeNoise(transform.position, transform.position, 10f);
        }
        else if (animatorStateInfo.IsName("BasicMotions@Walk01 - Forwards [RM]"))
        {
            _audioSource.clip = walkAudio;
        }
        else if (animatorStateInfo.IsName("BasicMotions@Idle01") && _audioSource.isPlaying)
        {
            _audioSource.Stop();
        }
        
        if(!_audioSource.isPlaying && !animatorStateInfo.IsName("BasicMotions@Idle01") && IsGrounded())
        {
            _audioSource.Play();
        }
    }

    void SetKinematic(bool newValue)
    {
        Rigidbody[] bodies = transform.GetChild(0).gameObject.GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = transform.GetChild(0).gameObject.GetComponentsInChildren<Collider>();
        foreach (Rigidbody rb in bodies)
        {
            if (rb.tag != "PlayerRightHand")
            {
                rb.isKinematic = newValue;
            }
        }
        foreach (Collider c in colliders)
        {
            c.enabled = !newValue;
        }
    }

    void Die()
    {
        ScoreControl.Instance.IncrementMultiplier(true);
        GameObject.FindGameObjectsWithTag("PlayerRightHand")[0].GetComponent<SpringJoint>().connectedBody = null;
        _animator.enabled = false;
        _rigidBody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        _interactionTrigger.enabled = false;
        SetKinematic(false);
        _playerHips.transform.SetParent(null);
        _isPlayerDead = true;
        //enabled = false;
    }

    void Ragdoll()
    {
        GameObject.FindGameObjectsWithTag("PlayerRightHand")[0].GetComponent<SpringJoint>().connectedBody = null;
        _animator.enabled = false;
        _rigidBody.isKinematic = true;
        GetComponent<Collider>().enabled = false;
        _interactionTrigger.enabled = false;
        SetKinematic(false);
        _playerHips.transform.SetParent(null);
        _isRagdoll = true;
        //enabled = false;
    }

    void Respawn(bool dead)
    {
        if (_isPlayerDead)
        {
            ScoreControl.Instance.IncrementScore(-1000);
        }
        _animator.enabled = true;
        _rigidBody.isKinematic = false;
        GetComponent<Collider>().enabled = true;
        _interactionTrigger.enabled = true;
        var child = _playerHips.transform;
        transform.position = child.position;
        child.SetParent(transform.Find("Dummy"));
        SetKinematic(true);
        _hitByProjectile = false;
        _isPlayerDead = false;
        _isRagdoll = false;
        if (!dead)
        {
            transform.position = transform.position + Vector3.up;
        }
        else
        {
            transform.position = _spawn.position + Vector3.up;
        }
        spawnText.enabled = false;
        //enabled = true;
    }

    IEnumerator DieRespawnCycle()
    {
        Die();
        spawnText.enabled = true;
        spawnText.text = "Press space to respawn";
        yield return new WaitUntil(() => _allowRespawn);
        Respawn(true);
    }

    public IEnumerator LaunchRagdoll(Vector3 direction, float launchVelocity)
    {
        Ragdoll();
        Rigidbody[] bodies = _playerHips.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in bodies)
        {
            rb.velocity = direction * launchVelocity;
        }
        spawnText.enabled = true;
        spawnText.text = "Press space to get up";
        yield return new WaitUntil(() => _allowRespawn);
        Respawn(false);
    }

    public void SetHitByProjectile()
    {
        if (!_isPlayerDead)
        {
            _hitByProjectile = true;
        }
    }

    public void AddCollectible(string tag, bool increment)
    {
        if (collectibles[tag] > 0 || increment)
        {
            collectibles[tag] += ((increment) ? 1 : -1);
        }
        //collectibleCount += 1;
    }

    public bool IsDead()
    {
        return _isPlayerDead;
    }

    public int GetCollectibles()
    {
        return collectibles["Collectible"];
    }

    public bool HasCollectible(string tag)
    {
        return (collectibles[tag] > 0);
    }

    public void SetIsInVehicle(bool value)
    {
        _isInVehicle = value;
    }

    public bool IsRagdoll()
    {
        return _isRagdoll;
    }

    void SetRake()
    {
        if (collectibles["Rake"] > 0)
        {
            Ray ray = new Ray(transform.position + Vector3.up, Vector3.down*2f);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray, out hit))
            {
                var rake = Instantiate(rakePrefab, transform.position + Vector3.up * 0.1f, Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation * Quaternion.Euler(0, -90, 0));
                AddCollectible(rake.tag, false);
            }
            
        }
    }

    public void StopPlayer()
    {
        _animator.enabled = false;
    }
}