using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

enum FiringMode
{
    Normal = 1,
    Explosive,
    Player,
    Length
}

public class CannonControl : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _firingModeText;
    private AudioSource _AudioSource;
    private float _power = 0f;
    private float _index;
    private bool _playerHasFired = false;
    private bool[] _projectilesFired = {false , false};
    private int _modeSelection = 1;
    [SerializeField]
    private PlayerMovement _playerScript;

    void Start()
    {
        _AudioSource = gameObject.GetComponent<AudioSource>();
    }

    void Awake()
    {
        _firingModeText.enabled = false;
    }

    void OnEnable()
    {
        _firingModeText.enabled = true;
    }

    void OnDisable()
    {
        _firingModeText.enabled = false;
    }

    void Update()
    {
        _firingModeText.text = "Mode: " + ((FiringMode)_modeSelection).ToString();
        if(Input.GetKeyDown(KeyCode.Space) && _modeSelection == (int)FiringMode.Normal && _playerScript.HasCollectible("Cannonball"))
        {
            _projectilesFired[_modeSelection-1] = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _modeSelection == (int)FiringMode.Explosive && _playerScript.HasCollectible("ExpCannonball"))
        {
            _projectilesFired[_modeSelection-1] = true;
        }

        if (Input.GetKey(KeyCode.Space) && _modeSelection == (int)FiringMode.Player)
        {
            _playerHasFired = true;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _modeSelection += 1;
            if (_modeSelection % (int)FiringMode.Length == 0)
            {
                _modeSelection = 1;
            }
        }
    }

    public bool PlayerHasFired()
    {
        return _playerHasFired;
    }

    public int ProjectileHasFired()
    {
        foreach (bool fired in _projectilesFired)
        {
            if (fired)
            {
                _AudioSource.Play();
                return (int)((FiringMode)_modeSelection)-1;
            }
        }
        return -1;
    }

    public void SetHasFired(bool value)
    {
        _projectilesFired[0] = value;
        _projectilesFired[1] = value;
        _playerHasFired = value;
    }
}
