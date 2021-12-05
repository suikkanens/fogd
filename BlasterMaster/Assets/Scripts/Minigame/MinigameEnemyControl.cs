using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameEnemyControl : MonoBehaviour
{
    [SerializeField]
    GameObject _expCannonball;
    [SerializeField]
    bool _isBlue;
    Animator _animator;
    Rigidbody _rigidBody;
    CapsuleCollider _activeCollider;
    bool _isDead;
    int _row;
    bool _isImmortal;
    bool _explosive;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidBody = GetComponent<Rigidbody>();
        _activeCollider = GetComponent<CapsuleCollider>();
        SetKinematic(true);
        _animator.SetBool("IsIdle", true);
        _isDead = false;
        _isImmortal = false;
        _explosive = (_expCannonball != null);
    }

    public void SetHitByCannonball(bool dead)
    {
        if (dead && !_isDead && !_isImmortal)
        {
            _isDead = true;
            Die();
        }
    }

    void Die()
    {
        if (_explosive)
        {
            Instantiate(_expCannonball, transform.position, Quaternion.identity);
        }
        if (_isBlue)
        {
            MinigameScoreControl.Instance.IncrementScore(-1000);
            MinigameScoreControl.Instance.ResetMultiplier();
        }
        else
        {
            MinigameScoreControl.Instance.IncrementScore(50);
        }

        MinigameCycle.Instance.RemoveEnemy(_row, gameObject);
        _animator.enabled = false;
        _rigidBody.isKinematic = true;
        _activeCollider.enabled = false;
        SetKinematic(false);
        Destroy(gameObject, 10);
        //enabled = false;
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

    public void SetRow(int row)
    {
        _row = row;
    }

    public void SetAsImmortal()
    {
        _isImmortal = true;
    }

    public bool IsBlue()
    {
        return _isBlue;
    }
}
