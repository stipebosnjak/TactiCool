using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

[Obsolete("This was a prototype class, Use Enemy class instead")]
public class AI : MonoBehaviour
{
    public float speed = 1f;
    public GameObject boundsGo;
    public Player player;


    private Vector3 _moveTarget;
    private Bounds _areaBound;
    private float _speedStep;
    private Animator _animator;
    private LocomotionSimpleAgent _locomotion;

    public float thinkInterval = 3f;
    private float _thinkIntervalTemp;
    private float _step;

    private IEnumerator _travelCoroutine;
    private static readonly int Speed = Animator.StringToHash("Speed");

    void Start()
    {
        Assert.IsNotNull(boundsGo);
        _thinkIntervalTemp = thinkInterval;
        _areaBound = boundsGo.GetComponent<Renderer>().bounds;
        _animator = GetComponent<Animator>();
        _speedStep = 0f;
        player = FindObjectOfType<Player>();
        _thinkIntervalTemp = UnityEngine.Random.Range(2f, 6f);
        _locomotion = GetComponent<LocomotionSimpleAgent>();
    }

    void Update()
    {
        if (Watch())
        {

        }

        _thinkIntervalTemp -= Time.deltaTime;
        if (_thinkIntervalTemp <= 0.0f)
        {
            Think();
            _thinkIntervalTemp = thinkInterval;
        }
    }
    private bool Watch()
    {
        var distance = Vector3.Distance(transform.position, player.transform.position);
        var direction = player.transform.position - transform.position;
        var angle = Vector3.Angle(direction, transform.forward);
        //Debug.Log($"Sight angle : {angle}");


        if (angle < 30)
        {
            // Debug.Log("I see the player");
            return true;
        }
        return false;
    }




    private void Think()
    {

        if (UnityEngine.Random.Range(0, 3) == 2)
        {
            return;
        }
        Vector3 navMeshPosition;
        if (RandomPoint(RandomPositionWithinBounds(_areaBound), range, out navMeshPosition))
        {

            _locomotion.Move(navMeshPosition);
            return;
        }

        Debug.LogWarning("Failed to find a random position in mesh");
        // Travel(RandomPositionWithinBounds(_areaBound));
    }


    public float range = 10.0f;
    private bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = center + UnityEngine.Random.insideUnitSphere * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }
    private Vector3 RandomPositionWithinBounds(Bounds bounds)
    {
        var x = UnityEngine.Random.Range(bounds.min.x, bounds.max.x);
        var z = UnityEngine.Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(x, 0.5f, z);
    }

    public void Travel(Vector3 position)
    {
        if (_travelCoroutine != null)
        {
            StopCoroutine(_travelCoroutine);
            _travelCoroutine = null;
        }
        var flatPosition = new Vector3(transform.position.x, 0.5f, transform.position.z);
        _travelCoroutine = MoveFromTo(gameObject.transform, flatPosition, position, speed);

        StartCoroutine(_travelCoroutine);
    }


    IEnumerator MoveFromTo(Transform objectToMove, Vector3 a, Vector3 b, float speed)
    {
        var step = (speed / (a - b).magnitude) * Time.fixedDeltaTime;
        var t = 0f;
        while (t <= 1.0f)
        {
            t += step; // Goes from 0 to 1, incrementing by step each time
            objectToMove.position = Vector3.Lerp(a, b, t); // Move objectToMove closer to b
            _speedStep = t;
            _animator.SetFloat(Speed, _speedStep);
            objectToMove.LookAt(b);
            yield return new WaitForFixedUpdate(); // Leave the routine and return here in the next frame
        }

        objectToMove.position = b;
    }
}