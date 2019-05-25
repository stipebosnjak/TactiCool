using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class AI : MonoBehaviour
{
    public float speed = 1f;
    public GameObject boundsGo;

    private Vector3 _moveTarget;
    private Bounds _areaBound;
    private float _speedStep;
    private Animator _animator;


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
    }

    void Update()
    {
        _thinkIntervalTemp -= Time.deltaTime;
        if (_thinkIntervalTemp <= 0.0f)
        {
            Think();
            _thinkIntervalTemp = thinkInterval;
        }
    }

    private void Think()
    {
        if (Random.Range(0, 3) == 2)
        {
            return;
        }

        Travel(RandomPositionWithinBounds());
    }

    private Vector3 RandomPositionWithinBounds()
    {
        var x = Random.Range(_areaBound.min.x, _areaBound.max.x);
        var z = Random.Range(_areaBound.min.z, _areaBound.max.z);

        return new Vector3(x, 0.5f, z);
    }

    public void Travel(Vector3 position)
    {
        if (_travelCoroutine != null)
        {
            StopCoroutine(_travelCoroutine);
            _travelCoroutine = null;
        }
        var flatPosition = new  Vector3(transform.position.x,0.5f, transform.position.z);
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
            _animator.SetFloat(Speed,_speedStep);
            objectToMove.LookAt(b);
            yield return new WaitForFixedUpdate(); // Leave the routine and return here in the next frame
        }
        
        objectToMove.position = b;
    }
}