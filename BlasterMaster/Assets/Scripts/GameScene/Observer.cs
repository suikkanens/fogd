using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public float _radius = 10f;
    bool _isPlayerInRange;
    Vector3 _lastPlayerPosition;
    Vector3 _direction;
    string _playerTag;
    BadGuyControl _badGuyScript;
    GameObject _player;
    PlayerMovement _playerScript;

    void Start()
    {
        transform.position = transform.parent.position + transform.parent.forward * _radius;
        _isPlayerInRange = false;
        _badGuyScript = transform.parent.gameObject.GetComponent<BadGuyControl>();
        _player = GameObject.FindWithTag("Player");
        _playerScript = _player.GetComponent<PlayerMovement>();
    }

    void FixedUpdate()
    {
        var current_radius = (_isPlayerInRange || _badGuyScript.IsAlerted()) ? _radius * 1.5f : _radius;
        var rangeAngle = 0.5f;
        var playerFound = false;

        if (_isPlayerInRange)
        {
            rangeAngle = -1f;
        }
        else if (_badGuyScript.IsAlerted())
        {
            rangeAngle = -0.2f;
        }

        playerFound = LookForPlayer(current_radius, rangeAngle);
        LookForDeadEnemies(current_radius);
        

        if (!playerFound)
        {
            _isPlayerInRange = false;
            _badGuyScript.SetPlayerInView(false);
        }
    }

    bool LookForPlayer(float current_radius, float rangeAngle)
    {
        var layerMask = (1 << 8);
        Collider[] colliders = Physics.OverlapSphere(transform.position, current_radius, layerMask);
        foreach (Collider col in colliders)
        {
            if (_playerScript.IsDead())
            {
                _badGuyScript.ResetState();
                break;
            }
            var range = GetRange(col.transform.position) > rangeAngle;
            var wallHit = LookForWalls(col.transform.position);

            if (range && !wallHit)
            {
                _isPlayerInRange = true;
                _badGuyScript.SetPlayerInView(true);
                _lastPlayerPosition = col.transform.position;
                _playerTag = col.gameObject.tag;
                return true;
            }
        }
        return false;
    }

    void LookForDeadEnemies(float current_radius)
    {
        var layerMask = (1 << 9);
        Collider[] colliders = Physics.OverlapSphere(transform.position, current_radius, layerMask);
        foreach (Collider col in colliders)
        {
            var badGuy = col.gameObject;
            var wallHit = LookForWalls(badGuy.transform.position);
            var bgRange = GetRange(badGuy.transform.position) > 0.5f;
            if (CheckForBodies(badGuy, bgRange) && !_isPlayerInRange && !_badGuyScript.IsAlerted() && !wallHit)
            {
                _badGuyScript.SetAlert(badGuy.transform.position);
            }
        }
    }

    void OnDisable()
    {
        _isPlayerInRange = false;
    }

    float GetRange(Vector3 playerPosition)
    {
        Vector3 A = Vector3.Scale(playerPosition - transform.parent.position, new Vector3(1, 0, 1)).normalized;
        return Vector3.Dot(A, transform.parent.forward.normalized);
    }

    void OnDrawGizmosSelected()
    {
        var current_radius = _radius;
        if (_isPlayerInRange)
        {
            current_radius = _radius * 1.5f;
        }
        Gizmos.DrawWireSphere(transform.position, current_radius);
        Gizmos.DrawRay(transform.parent.position + Vector3.up + transform.forward * 0.3f, _direction);
        Gizmos.DrawWireSphere(GetLastPlayerPosition(), 0.5f);
    }

    bool CheckForBodies(GameObject badGuy, bool bgRange)
    {
        var bgScript = badGuy.GetComponent<BadGuyDisabledControl>();
        if (!bgScript.SeenWhileDead() && bgRange)
        {
            bgScript.SetSeenWhileDead(true);
            return true;
        }
        return false;
    }

    bool LookForWalls(Vector3 targetPosition)
    {
        var layerMask = 1 << 6;
        _direction = targetPosition - transform.parent.position;
        return Physics.Raycast(new Ray(transform.parent.position + Vector3.up + transform.forward, _direction), _direction.magnitude, layerMask);
    }

    public bool IsPlayerInRange()
    {
        return _isPlayerInRange;
    }

    public Vector3 GetLastPlayerPosition()
    {
        return _lastPlayerPosition;
    }

    public string GetPlayerTransform()
    {
        return _playerTag;
    }
}
