using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawTrajectory : MonoBehaviour
{
    [SerializeField]
    private LineRenderer _lineRenderer;

    [SerializeField]
    [Range(20,200)]
    private int _linePointCount = 20;
    private List<Vector3> _linePoints = new List<Vector3>();
    private Vector3[] _gizmoPoints;
    private Vector3 _hitPosition;
    [SerializeField]
    private GameObject _hitMarker;
    private bool _showTrajectory;

    #region Singleton

    public static DrawTrajectory Instance
    {
        get
        {
            return _instance;
        }
    }

    private static DrawTrajectory _instance;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            _instance = this;
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    #endregion

    void Start()
    {
        _hitPosition = transform.position;
        _showTrajectory = false;
    }

    void Update()
    {
        if (_showTrajectory)
        {
            _lineRenderer.enabled = true;
            _hitMarker.SetActive(true);
            _showTrajectory = false;
        }
        else
        {
            _lineRenderer.enabled = false;
            _hitMarker.SetActive(false);
        }
    }

    public void UpdateTrajectory(Vector3 velocity, float mass, Vector3 startPoint)
    {
        //Vector3 velocity = (force / mass) * Time.fixedDeltaTime;
        //Vector3 velocity = 20f * force.normalized;
        //float timeOfFlight = (2 * velocity.y) / Physics.gravity.y;
        //float timeStep = timeOfFlight / _linePointCount;

        _linePoints.Clear();
        var start = startPoint;
        for (int i = 0; i < _linePointCount; i++)
        {
            float timePassed = 0.05f * i;
            Vector3 trajectory = new Vector3(
                velocity.x * timePassed,
                velocity.y * timePassed + 0.5f * Physics.gravity.y * timePassed * timePassed,
                velocity.z * timePassed
                );
            _linePoints.Add(startPoint + trajectory);
            
            RaycastHit hit;
            Vector3 direction = _linePoints[i] - start;
            if (Physics.Raycast(start, direction, out hit, direction.magnitude))
            {
                Debug.DrawRay(start,direction);
                _hitPosition = hit.point;
                _hitMarker.transform.position = _hitPosition;
                break;
                //Debug.Log(_hitPosition);
            }
            start = _linePoints[i];
        }
        _lineRenderer.positionCount = _linePoints.Count;
        _lineRenderer.SetPositions(_linePoints.ToArray());
        _gizmoPoints = _linePoints.ToArray();
    }

    public void ShowTrajectory(bool val)
    {
        _showTrajectory = val;
    }

    void OnDrawGizmosSelected()
    {
        if (_gizmoPoints != null)
        {
            for (int i = 0; i < _gizmoPoints.Length; i++)
            {
                //if (i > 0)
                //{
                //    Gizmos.DrawRay(_gizmoPoints[i - 1], (_gizmoPoints[i] - _gizmoPoints[i - 1]));
                //    Debug.Log(_gizmoPoints[i]);
                //}
                //Gizmos.DrawWireSphere(_gizmoPoints[i], 0.5f);
            }
        }

        Gizmos.DrawWireSphere(_hitPosition, 2f);
    }
}
