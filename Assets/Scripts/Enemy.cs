using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;

[RequireComponent(typeof(BehaviorTree))]
public class Enemy : MonoBehaviour
{
    public SharedGameObjectList patrolPoints;

    private BehaviorTree _behavioralTree;

    // Start is called before the first frame update
    void Start()
    {
        _behavioralTree = GetComponent<BehaviorTree>();


        //if(patrolPoints ==null || patrolPoints.Length == 0)
        //{
        //    Debug.LogWarning("Patrol points are not set");
        //}
        _behavioralTree.SetVariableValue("PatrolPoints", patrolPoints);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
