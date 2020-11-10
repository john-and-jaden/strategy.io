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
        /**
            Movement controls
            1. Units have priority: the first one to have set a movement order wins (lowest timestamp)
            2. Units store expected path by frame (measured by speed) and use this to check collisions when a move order is given
            3. When a potential collision is detected, update the path with an intermediate point for that unit
            4. Group orders just set an individual movepath for each unit

            Collision buffering:
             - Each unit stores the path ahead of it (each step based on chosen gapWidth (probably between 1-2) * unit size)
             - Array indexed by step number for all units used to detect collisions at a particular step
             - Buffer of steps detected based on a fixed length (only store the next x steps)
        **/

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
