using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInput : MonoBehaviour
{
    public Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
        Assert.IsNotNull(player,"Player should not be null");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseClick();
        }
    }

    void MouseClick()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Clicked Collider: {hit.collider.gameObject.name}");

            var go = hit.collider.gameObject;
            if (go.name == "TestFloor")
            {
                var position = hit.point;
                player.Travel(position);
            }
        }
    }
}
