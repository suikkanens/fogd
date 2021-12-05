using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

enum EnemyState
{
    Idle,
    Patrolling,
    HeardNoise,
    LookingForPlayer,
    PlayerInView,
    Dead
}

public class BadGuyControl : MonoBehaviour
{
    public float turnSpeed = 20f;
    public Transform player;
    public GameObject attackProjectilePrefab;

    //Physics
    Animator _animator;
    Rigidbody _rigidBody;
    CapsuleCollider _activeCollider;
    Vector3 _hitForce;
    Observer _pointOfView;

    // Movement
    Vector3 _movement;
    Quaternion _rotation = Quaternion.identity;

    //Waypoints
    public UnityEngine.AI.NavMeshAgent _navMeshAgent;
    public List<Transform> _waypoints = new List<Transform>();
    int _currentWaypointIndex;
    Vector3 _alertPosition;

    bool _isIdle;
    bool _patrolling;
    bool _heardNoise;
    bool _lookingForPlayer;
    bool _playerInView;
    bool _isDead;

    int _enemyState;
    int _previousState;
    int _requestedState;

    float _prevDistance;
    float _distanceTime;
    int _distCount = 120;
    List<float> _distancesCovered;
    int _distIndex;

    //Attack
    GameObject _attackIndicatorPrefab;
    float _nextAttack;
    bool _canShoot;
    Coroutine _co;

    //music
    bool _musicCrRunning;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        _activeCollider = GetComponent<CapsuleCollider>();
        _pointOfView = transform.Find("PointOfView").gameObject.GetComponent<Observer>();
        SetKinematic(true);
        _enemyState = (int)EnemyState.Idle;
        _previousState = 0;
        _requestedState = 0;
        _currentWaypointIndex = 0;
        _prevDistance = 0f;
        _distanceTime = 0f;
        _distIndex = 0;
        _distancesCovered = new List<float>(_distCount);
        for (int i = 0; i < _distCount; i++)
        {
            _distancesCovered.Add(0f);
        }

        _isIdle = (_enemyState == (int)EnemyState.Idle);
        _patrolling = (_enemyState == (int)EnemyState.Patrolling);
        _heardNoise = (_enemyState == (int)EnemyState.HeardNoise);
        _lookingForPlayer = (_enemyState == (int)EnemyState.LookingForPlayer);
        _playerInView = (_enemyState == (int)EnemyState.PlayerInView);
        _isDead = (_enemyState == (int)EnemyState.Dead);

        _musicCrRunning = false;

        _attackIndicatorPrefab = transform.Find("EnemyAttackIndicator").gameObject;
        _nextAttack = Time.time;
        _canShoot = true;

        if (_waypoints.Count != 0)
        {
            _navMeshAgent.SetDestination(_waypoints[0].position);
        }
        else
        {
            var go = new GameObject();
            go.transform.position = transform.position;
            _waypoints.Add(go.transform);
            _navMeshAgent.SetDestination(_waypoints[0].position);
        }
    }

    void Update()
    {
        _attackIndicatorPrefab.transform.position = transform.position + transform.up * 1.75f + transform.forward * 0.35f;
        
        MovementState();

        _isIdle = (_enemyState == (int)EnemyState.Idle);
        _patrolling = (_enemyState == (int)EnemyState.Patrolling);
        _heardNoise = (_enemyState == (int)EnemyState.HeardNoise);
        _lookingForPlayer = (_enemyState == (int)EnemyState.LookingForPlayer);
        _playerInView = (_enemyState == (int)EnemyState.PlayerInView);

        if (!_musicCrRunning)
        {
            StartCoroutine(AssignMusicStatus());
        }

        _animator.SetBool("IsPlayerInRange", _playerInView);
        _animator.SetBool("IsPatrolling", _patrolling);
        _animator.SetBool("IsAlerted", IsAlerted());
        _animator.SetBool("IsIdle", _isIdle);
    }

    void SetKinematic(bool newValue)
    {
        Rigidbody[] bodies = transform.GetChild(0).gameObject.GetComponentsInChildren<Rigidbody>();
        Collider[] colliders = transform.GetChild(0).gameObject.GetComponentsInChildren<Collider>();
        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = newValue;
        }
        foreach (Collider c in colliders)
        {
            c.enabled = !newValue;
        }
    }

    void FollowPlayer()
    {
        var playerTransform = _pointOfView.GetPlayerTransform();
        _navMeshAgent.SetDestination(GameObject.FindWithTag(playerTransform).transform.position);
    }

    void OnAnimatorMove()
    {
        if (Time.timeScale == 1 && !float.IsNaN(_animator.deltaPosition.magnitude / Time.deltaTime))
        {
            _navMeshAgent.speed = _animator.deltaPosition.magnitude / Time.deltaTime;
        }
    }

    void CheckIfStuck()
    {
        var coveredDist = _prevDistance - _navMeshAgent.remainingDistance;
        _distancesCovered[_distIndex] = coveredDist;
        _distIndex = (_distIndex + 1) % _distCount;
        var isStationary = false;
        UnityEngine.AI.NavMeshHit hit;
        if (!UnityEngine.AI.NavMesh.SamplePosition(_navMeshAgent.destination, out hit, 2f, UnityEngine.AI.NavMesh.AllAreas))
        {
            if (!(_enemyState == (int)EnemyState.PlayerInView))
            {
                ResetState();
            }
        }
        else if (!UnityEngine.AI.NavMesh.SamplePosition(_navMeshAgent.destination, out hit, 0.1f, UnityEngine.AI.NavMesh.AllAreas) && _navMeshAgent.remainingDistance < 0.005f && IsAlerted())
        {
            ResetState();
        }
    }

    void GoToDestination()
    {
        CheckIfStuck();

        if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance)
        {
            _enemyState = (int)EnemyState.Idle;
            if (_waypoints.Count > 1)
            {
                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Count;
                _navMeshAgent.SetDestination(_waypoints[_currentWaypointIndex].position);
            }
            
        }
    }

    public void SetHitByCannonball(bool dead)
    {
        if (dead && !_isDead)
        {
            Die();
        }
    }

    public bool HasSeenPlayer()
    {
        return _lookingForPlayer;
    }

    public bool IsPlayerInView()
    {
        return _playerInView;
    }


    public void SetAlert(Vector3 position)
    {
        if (_isIdle || _patrolling || _heardNoise || _lookingForPlayer)
        {
            _enemyState = (int)EnemyState.HeardNoise;
            _alertPosition = position;
        }
    }

    public void SetPlayerInView(bool inView)
    {
        if (inView)
        {
            _enemyState = (int)EnemyState.PlayerInView;
        }
        else if (!inView && _playerInView)
        {
            _enemyState = 0;
        }
    }

    public void ResetState()
    {
        _enemyState = (int)EnemyState.Patrolling;
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(_waypoints[_currentWaypointIndex].position);
    }

    IEnumerator Attack()
    {
        var particles = _attackIndicatorPrefab.GetComponentsInChildren<ParticleSystem>();
        foreach (var p in particles)
        {
            p.Play();
            yield return new WaitForSeconds(p.main.duration);
        }
        ShootProjectile();
        yield return new WaitForSeconds(1f);
        _canShoot = true;
        
    }

    void ShootProjectile()
    {
        var projectile = Instantiate(attackProjectilePrefab, _attackIndicatorPrefab.transform.position + transform.forward*0.5f, Quaternion.identity);
        foreach (Transform child in transform.parent)
        {
            Physics.IgnoreCollision(projectile.GetComponent<Collider>(), child.gameObject.GetComponent<CapsuleCollider>());
        }
        
        var playerTransform = _pointOfView.GetPlayerTransform();
        var hitPoint = GameObject.FindWithTag(playerTransform).transform.position;
        if (playerTransform == "Player")
        {
            hitPoint += Vector3.up;
        }
        projectile.GetComponent<Rigidbody>().velocity = (hitPoint - _attackIndicatorPrefab.transform.position).normalized * 10f;
    }

    void Die()
    {
        if (_co != null)
        {
            StopCoroutine(_co);
        }

        if (_playerInView)
        {
            BackgroundMusicControl.Instance.IncrementPlayerDetected(false);
        }

        if (_heardNoise || _lookingForPlayer)
        {
            BackgroundMusicControl.Instance.IncrementEnemiesAlerted(false);
        }

        ScoreControl.Instance.IncrementScore(100);
        ScoreControl.Instance.IncrementMultiplier(false);

        gameObject.layer = 2;
        transform.Find("SneakAttackTrigger").gameObject.layer = 2;
        _animator.enabled = false;
        _navMeshAgent.enabled = false;
        _rigidBody.isKinematic = true;
        _activeCollider.enabled = false;
        _pointOfView.enabled = false;
        SetKinematic(false);
        enabled = false;
    }

    void MovementState()
    {
        if (!_playerInView)
        {
            if (_requestedState == ((int)EnemyState.LookingForPlayer))
            {
                _enemyState = (int)EnemyState.LookingForPlayer;
                _requestedState = 0;
            }

            if (_co != null)
            {
                StopCoroutine(_co);
            }
            _canShoot = true;
        }

        switch (_enemyState)
        {
            case (int)EnemyState.Idle:
                if (_waypoints.Count > 1 || (_previousState == (int)EnemyState.LookingForPlayer || _previousState == (int)EnemyState.HeardNoise))
                {
                    _navMeshAgent.SetDestination(_waypoints[_currentWaypointIndex].position);
                    _enemyState = (int)EnemyState.Patrolling;
                }
                else
                {
                    _navMeshAgent.enabled = false;
                }
                _previousState = (int)EnemyState.Idle;
                break;

            case (int)EnemyState.Patrolling:
                if (!_navMeshAgent.enabled)
                {
                    _navMeshAgent.enabled = true;
                }
                GoToDestination();
                _previousState = (int)EnemyState.Patrolling;
                break;

            case (int)EnemyState.HeardNoise:
                if (!_navMeshAgent.enabled)
                {
                    _navMeshAgent.enabled = true;
                }

                if (_previousState == (int)EnemyState.Idle || _previousState == (int)EnemyState.Patrolling)
                {
                    ScoreControl.Instance.IncrementMultiplier(true);
                }
                
                _navMeshAgent.SetDestination(_alertPosition);
                GoToDestination();
                _previousState = (int)EnemyState.HeardNoise;
                break;

            case (int)EnemyState.LookingForPlayer:
                if (!_navMeshAgent.enabled)
                {
                    _navMeshAgent.enabled = true;
                }
                _alertPosition = _pointOfView.GetLastPlayerPosition();
                _navMeshAgent.SetDestination(_alertPosition);
                GoToDestination();
                _previousState = (int)EnemyState.LookingForPlayer;
                break;

            case (int)EnemyState.PlayerInView:
                if (!_navMeshAgent.enabled)
                {
                    _navMeshAgent.enabled = true;
                }

                if (_previousState == (int)EnemyState.Idle || _previousState == (int)EnemyState.Patrolling || _previousState == (int)EnemyState.HeardNoise)
                {
                    ScoreControl.Instance.IncrementScore(-100);
                    ScoreControl.Instance.IncrementMultiplier(true);
                }

                FollowPlayer();
                if (_canShoot)
                {
                    _canShoot = false;
                    _co = StartCoroutine(Attack());
                }
                _requestedState = (int)EnemyState.LookingForPlayer;
                _previousState = (int)EnemyState.PlayerInView;
                break;

            case (int)EnemyState.Dead:
                _previousState = (int)EnemyState.Dead;
                break;

            default:
                break;
        }
    }

    void OnDrawGizmosSelected()
    {
        //Gizmos.DrawWireSphere(_pointOfView.GetLastPlayerPosition(), 0.2f);
    }

    IEnumerator AssignMusicStatus()
    {
        _musicCrRunning = true;
        if (_playerInView)
        {
            BackgroundMusicControl.Instance.IncrementPlayerDetected(true);
            yield return new WaitUntil(() => !_playerInView);
            BackgroundMusicControl.Instance.IncrementPlayerDetected(false);
        }

        if (IsAlerted())
        {
            BackgroundMusicControl.Instance.IncrementEnemiesAlerted(true);
            yield return new WaitUntil(() => !IsAlerted());
            BackgroundMusicControl.Instance.IncrementEnemiesAlerted(false);
        }
        _musicCrRunning = false;
    }

    public bool IsDead()
    {
        return _isDead;
    }

    public bool IsAlerted()
    {
        return (_heardNoise || _lookingForPlayer);
    }
}
