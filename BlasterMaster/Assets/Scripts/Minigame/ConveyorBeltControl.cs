using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBeltControl : MonoBehaviour
{
    [SerializeField]
    [Range(50f,500f)]
    float _speed;
    Vector3 _direction;
    List<GameObject> _onBelt;
    Transform _rightSpawn;
    Transform _leftSpawn;

    // Start is called before the first frame update
    void Start()
    {
        _onBelt = new List<GameObject>();
        _direction = transform.right;
        _rightSpawn = transform.Find("RightSpawn");
        _leftSpawn = transform.Find("LeftSpawn");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (GameObject obj in _onBelt)
        {
            if (obj != null)
            {
                obj.GetComponent<Rigidbody>().velocity = _speed * _direction * Time.deltaTime;
            }
        }
    }

    void OnDisable()
    {
        _onBelt.Clear();
    }

    void OnCollisionEnter(Collision col)
    {
        _onBelt.Add(col.gameObject);
    }

    void OnCollisionExit(Collision col)
    {
        _onBelt.Remove(col.gameObject);
    }

    public void SetRowDirection()
    {

        _direction = _direction * -1f;
    }

    public Transform GetSpawn(bool right)
    {
        if (right)
        {
            _direction = -transform.right;
            return _rightSpawn;
        }
        else
        {
            _direction = transform.right;
            return _leftSpawn;
        }

    }
}
