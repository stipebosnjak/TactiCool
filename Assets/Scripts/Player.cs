using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        public float speed;

        private bool _isTraveling;
        private Vector3 _moveTarget;

        private IEnumerator _travelCoroutine;
        private LocomotionSimpleAgent _locomotion;

        private void Start()
        {
            _locomotion = GetComponent<LocomotionSimpleAgent>();
        }

        public void HandleTargetObject(GameObject go)
        {
            //Debug.Log()
            //if (go.name == "TestFloor")
            //{
            var position = go.transform.position;
            Travel(position);
            //}
        }

        public void Travel(Vector3 position)
        {
            //if (_travelCoroutine != null)
            //{
            //    StopCoroutine(_travelCoroutine);
            //    _travelCoroutine = null;
            //}


            Vector3 navMeshPosition;
            if (RandomPoint(position, 0f, out navMeshPosition))
            {

                _locomotion.Move(navMeshPosition);
                return;
            }


            // _travelCoroutine = MoveFromTo(gameObject.transform, transform.position, position, speed);

            // StartCoroutine(_travelCoroutine);
        }

        public float range = 10.0f;
        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < 30; i++)
            {
                // Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(center, out hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
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
}