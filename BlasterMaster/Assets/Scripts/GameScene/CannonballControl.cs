using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CannonballControl : MonoBehaviour
{
    Rigidbody _rigidbody;
    private Vector3 _turretPos;
    private NoiseControl _noiseScript;
    private bool _isMinigame;
    private bool _hasCollided;

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _noiseScript = GetComponent<NoiseControl>();
        try
        {
            _isMinigame = MinigameCycle.Instance != null;
        }
        catch (Exception e)
        {
            _isMinigame = false;
        }
        _hasCollided = false;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "BadGuy")
        {
            col.gameObject.GetComponent<BadGuyControl>().SetHitByCannonball(true);
        }
        else if (col.gameObject.tag == "MinigameBadGuy" && !_hasCollided)
        {
            var enemyScript = col.gameObject.GetComponent<MinigameEnemyControl>();
            enemyScript.SetHitByCannonball(true);
            if (!enemyScript.IsBlue())
            {
                MinigameScoreControl.Instance.IncrementMultiplier();
            }
            _hasCollided = true;
        }
        else if (_isMinigame && !_hasCollided)
        {
            MinigameScoreControl.Instance.ResetMultiplier();
            _hasCollided = true;
        }

        if (!_isMinigame)
        {
            _noiseScript.MakeNoise(transform.position, _turretPos, 5f);
        }
        
        Destroy(gameObject, 10f);
    }

    public void SetTurretPosition(Vector3 pos)
    {
        _turretPos = pos;
    }

}
