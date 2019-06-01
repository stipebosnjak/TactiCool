using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Locomotion : MonoBehaviour
{

    private NavMeshAgent _navMeshAgent;
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void Move(Vector3 position)
    {
        NavMeshPath path = new NavMeshPath();
        bool hasFoundPath = _navMeshAgent.CalculatePath(position, path);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            print("The agent can reach the destionation");
            _navMeshAgent.SetPath(path);
        }
        else if (path.status == NavMeshPathStatus.PathPartial)
        {
            print("The agent can only get close to the destination");
        }
        else if (path.status == NavMeshPathStatus.PathInvalid)
        {
            print("The agent cannot reach the destination");
            print("hasFoundPath will be false");
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
