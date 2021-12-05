using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SneakControl : MonoBehaviour
{
    Collider _sneakAttackTrigger;
    bool _parentEnabled;
    bool _playerSeen;
    BadGuyControl _badGuyScript;
    // Start is called before the first frame update
    void Start()
    {
        _sneakAttackTrigger = gameObject.GetComponent<Collider>();
        _parentEnabled = true;
        _badGuyScript = transform.parent.gameObject.GetComponent<BadGuyControl>();
    }

    // Update is called once per frame
    void Update()
    {
        _parentEnabled = _badGuyScript.enabled;
        if (_parentEnabled)
        {
            _playerSeen = _badGuyScript.IsPlayerInView();
        }
    }

    public bool IsParentEnabled()
    {
        return _parentEnabled;
    }

    public bool IsPlayerSeen()
    {
        return _playerSeen;
    }
}
