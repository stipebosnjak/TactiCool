using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public float speed;

    private bool _isTraveling;
    private Vector3 _moveTarget;

    private IEnumerator _travelCoroutine;

    
    public void HandleTargetObject(GameObject go)
    {
        if (go.name == "TestFloor")
        {
            var position = go.transform.position;
            Travel(position);
        }
    }

    public void Travel(Vector3 position)
    {
        if (_travelCoroutine != null)
        {
            StopCoroutine(_travelCoroutine);
            _travelCoroutine = null;
        }

        _travelCoroutine = MoveFromTo(gameObject.transform, transform.position, position, speed);

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
            yield return new WaitForFixedUpdate();         // Leave the routine and return here in the next frame
        }
        objectToMove.position = b;
    }
}