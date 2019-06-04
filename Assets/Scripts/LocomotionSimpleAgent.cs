using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class LocomotionSimpleAgent : MonoBehaviour
{

    private GameObject _navEndPointCircle;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Vector2 smoothDeltaPosition = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    private LookAt _lookAt;
    private LineRenderer _lineRenderer;
    private AnimatorNavSync _animatorNavSync;
    

    void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _lineRenderer = GetComponent<LineRenderer>();
        _animatorNavSync = GetComponent<AnimatorNavSync>();
        _lookAt = GetComponent<LookAt>();

        _navEndPointCircle = new GameObject("NavEndPointCircle");
        _navEndPointCircle.DrawCircle(1,0.1f);
        _navEndPointCircle.SetActive(false);
    }

    protected bool PathComplete()
    {
        if (Vector3.Distance(_navMeshAgent.destination, _navMeshAgent.transform.position) <= _navMeshAgent.stoppingDistance)
        {
            if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
            {
                return true;
            }
        }

        return false;
    }
    public void Move(Vector3 position)
    {
        _lineRenderer.startWidth = 0.1f;
        _lineRenderer.endWidth = 0.1f;
        _navMeshAgent.ResetPath();
        NavMeshPath path = new NavMeshPath();
        bool hasFoundPath = _navMeshAgent.CalculatePath(position, path);
        if (_lineRenderer)
            DrawPath(path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            //  print($"The agent:{gameObject.name} can reach the destionation");
            _navMeshAgent.SetPath(path);
        }
        else if (path.status == NavMeshPathStatus.PathPartial)
        {
            // print("The agent can only get close to the destination");
        }
        else if (path.status == NavMeshPathStatus.PathInvalid)
        {
            // print("The agent cannot reach the destination");
            // print("hasFoundPath will be false");
        }


    }

    private void DrawPath(NavMeshPath path)
    {
        _lineRenderer.SetPosition(0, transform.position);
        if (path.corners.Length < 2) //if the path has 1 or no corners, there is no need
            return;

        _lineRenderer.positionCount = path.corners.Length;

        var endPosition = path.corners.Last();
        _navEndPointCircle.transform.position = endPosition;
        _navEndPointCircle.SetActive(true);

        for (var i = 1; i < path.corners.Length; i++)
        {
            _lineRenderer.SetPosition(i, path.corners[i]); //go through each corner and set that to the line renderer's position
        }
    }

    public void Stop(bool immediately = true)
    {
        if (immediately)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.stoppingDistance = 0f;
        }
        else
        {
            _navMeshAgent.isStopped = true;
        }
    }
    public void Resume()
    {
        _navMeshAgent.isStopped = false;
    }
}
