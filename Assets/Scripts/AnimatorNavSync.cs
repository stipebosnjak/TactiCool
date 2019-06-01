using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimatorNavSync : MonoBehaviour
{
    // Start is called before the first frame update

    public enum MoveState { Idle, Walking }
    public MoveState moveState;

    private Animator _animator;
    private NavMeshAgent _navMeshAgent;
    private Vector2 smoothDeltaPosition;
    private Vector2 velocity;


    void Start()
    {
        _animator = GetComponent<Animator>();
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.updatePosition = false;
        _navMeshAgent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_navMeshAgent.hasPath)
            moveState = MoveState.Walking;
        else
            moveState = MoveState.Idle;

        _animator.speed = _navMeshAgent.speed;
        var deltaX = Vector3.Dot(transform.right, _navMeshAgent.velocity);
        var deltaZ = Vector3.Dot(transform.forward, _navMeshAgent.velocity);

        _animator.SetFloat("velx", deltaX, 0.1f, Time.deltaTime);
        _animator.SetFloat("velz", deltaZ, 0.1f, Time.deltaTime);
        _animator.SetBool("move", moveState == MoveState.Walking);
    }

    void OnAnimatorMove()
    {
        //only perform if walking
        if (moveState == MoveState.Walking)
        {
            // this works well and correct            
            transform.position = _navMeshAgent.nextPosition;

            // this part does not update it well enough
            // causes the transform to be faster than nav mesh agent !?
            //Vector3 position = _animator.rootPosition;
            //position.y = _navMeshAgent.nextPosition.y;
            //transform.position = position;
            
            //set the navAgent's velocity to the velocity of the animation clip currently playing
             //_navMeshAgent.velocity = _animator.deltaPosition / Time.deltaTime;
            //smoothly rotate the character in the desired direction of motion
            Quaternion lookRotation = Quaternion.LookRotation(_navMeshAgent.desiredVelocity);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, _navMeshAgent.angularSpeed * Time.deltaTime);
        }
    }
}
