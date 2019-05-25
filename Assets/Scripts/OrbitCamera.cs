using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

public class OrbitCamera : MonoBehaviour
{
    public Transform target;
    public float speed;

    private Vector3 _offset;
    private IEnumerator _travelCoroutine;


    // Start is called before the first frame update
    void Start()
    {
        target = FindObjectOfType<Player>().transform;
        Assert.IsNotNull(target,"target != null");
         _offset = transform.position - target.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.

        transform.position = Vector3.Lerp(transform.position, target.transform.position + _offset, speed * Time.deltaTime);
    }
}
