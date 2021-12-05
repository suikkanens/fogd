using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionManager : MonoBehaviour
{
    [SerializeField]
    GameObject _player;
    [SerializeField]
    GameObject _instructions;
    // Start is called before the first frame update
    void Start()
    {
        _instructions.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == _player)
        {
            _instructions.SetActive(true);
        }
    }

    void OnTriggerExit()
    {
        _instructions.SetActive(false);
    }
}
