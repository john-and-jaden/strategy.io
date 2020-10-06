using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour, ISelectable
{
    public float moveSpeed = 1f;
    public float selectDistance;

    public bool Selected { get; set; }

    private Vector2 targetPos;

    void Start()
    {
        targetPos = transform.position;
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
        else
        {
            // Select this object on left-click
            if (Input.GetButtonDown("Fire1"))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                float clickDist = Vector2.Distance(transform.position, mousePos);
                if (clickDist < selectDistance)
                {
                    Selected = true;
                }
                else
                {
                    Selected = false;
                }
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }
}
