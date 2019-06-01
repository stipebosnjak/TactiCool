using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class LocomotionSimpleAgent : MonoBehaviour
{
    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Vector2 smoothDeltaPosition = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    private LookAt _lookAt;
    private LineRenderer _lineRenderer;
    private bool _moving;

    public enum MoveState { Idle, Walking }
    public MoveState moveState;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _lineRenderer = GetComponent<LineRenderer>();
        _lookAt = GetComponent<LookAt>();       
    }

    void Update()
    {
        //character walks if there is a navigation path set, idle all other times
        if (_navMeshAgent.hasPath) 
             moveState = MoveState.Walking;
        else
            moveState = MoveState.Idle;
        //if (!_moving)
        //{
        //    return;
        //}

        //if (PathComplete())
        //{
        //    _moving = false;
        //    return;
        //}
        _animator.speed = _navMeshAgent.speed;
        Vector3 worldDeltaPosition = _navMeshAgent.nextPosition - transform.position;
        // Debug.DrawLine(transform.position + new Vector3(0, 10f, 0), _navMeshAgent.nextPosition, Color.yellow, 1f);
        // Map 'worldDeltaPosition' to local space
        var dx = Vector3.Dot(transform.right, worldDeltaPosition);
        var dy = Vector3.Dot(transform.forward, worldDeltaPosition);
        var deltaPosition = new Vector2(dx, dy);
        //var shouldMove = deltaPosition.magnitude > 0.5f && _navMeshAgent.remainingDistance > _navMeshAgent.radius;
        //if (gameObject.CompareTag("Player"))
        //{
        //    Debug.Log($"Nav next position {_navMeshAgent.nextPosition}, player pos: {transform.position} , {smoothDeltaPosition}, {velocity}");
        //}
        // Update animation parameters
        var shouldMove = moveState == MoveState.Walking;

        _animator.SetBool("move", shouldMove);
        _animator.SetFloat("velx", deltaPosition.x, 0.1f, Time.deltaTime);
        _animator.SetFloat("vely", deltaPosition.y, 0.1f, Time.deltaTime);

        //if (_lookAt)
        //    _lookAt.lookAtTargetPosition = _navMeshAgent.steeringTarget + transform.forward;

        //		// Pull character towards agent
        //		if (worldDeltaPosition.magnitude > agent.radius)
        //			transform.position = agent.nextPosition - 0.9f*worldDeltaPosition;

        //		// Pull agent towards character
        //		if (worldDeltaPosition.magnitude > agent.radius)
        //			agent.nextPosition = transform.position + 0.9f*worldDeltaPosition;
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
        _navMeshAgent.ResetPath();
        NavMeshPath path = new NavMeshPath();
        bool hasFoundPath = _navMeshAgent.CalculatePath(position, path);
        if (_lineRenderer)
            DrawPath(path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            //  print($"The agent:{gameObject.name} can reach the destionation");
            _navMeshAgent.SetPath(path);
            _moving = true;
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

    //void OnAnimatorMove()
    //{

    //    //only perform if walking
    //    if (moveState == MoveState.Walking)
    //    {
    //        //set the navAgent's velocity to the velocity of the animation clip currently playing
    //        _navMeshAgent.velocity = _animator.deltaPosition / Time.deltaTime;
    //        //smoothly rotate the character in the desired direction of motion
    //        Quaternion lookRotation = Quaternion.LookRotation(_navMeshAgent.desiredVelocity);
    //        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _navMeshAgent.angularSpeed * Time.deltaTime);

    //        //Vector3 position = _animator.rootPosition;
    //        //position.y = _navMeshAgent.nextPosition.y;
    //        //transform.position = position;
    //    }

    //    //// Update postion to agent position
    //    ////		transform.position = agent.nextPosition;

    //    //// Update position based on animation movement using navigation surface height
    //    //Vector3 position = _animator.rootPosition;
    //    //position.y = _navMeshAgent.nextPosition.y;
    //    //transform.position = position;
    //}
}
