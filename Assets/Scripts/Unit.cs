using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
    public float moveSpeed = 1f;
    public float selectDistance;

    private Vector2 targetPos;

    void Start()
    {
        SpawnIndicators();
        targetPos = transform.position;
    }

    void Update()
    {
        // Update indicators
        hoverIndicator.transform.position = transform.position;
        selectIndicator.transform.position = transform.position;
        UpdateIndicators();

        transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    public void SetTargetPos(Vector2 targetPos)
    {
        this.targetPos = targetPos;
    }
}
