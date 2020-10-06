using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, ISelectable
{
    public float moveSpeed = 1f;

    public bool Selected { get; set; }

    private Vector2 targetPos;

    void Start()
    {
        Selected = true;
    }

    void Update()
    {
        if (Selected)
        {
            // Change current target on right-click
            if (Input.GetButtonDown("Fire2"))
            {
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }
}
