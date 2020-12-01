using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
    public float moveSpeed = 1f;
    public float softCollisionRadius = 0.5f;

    private Vector3 moveTarget;
    private float gatherRadiusSqr;
    private bool isMoving;

    private Collider2D[] softCollisionTargets;

    void Start()
    {
        SpawnIndicators();
        moveTarget = transform.position;
    }

    void Update()
    {
        // Update indicators
        hoverIndicator.transform.position = transform.position;
        selectIndicator.transform.position = transform.position;
        UpdateIndicators();

        // Update movement
        if (isMoving)
        {
            transform.position = Vector2.MoveTowards(transform.position, moveTarget, moveSpeed * Time.deltaTime);
            float dist = Vector3.SqrMagnitude(transform.position - moveTarget);
            if (dist < gatherRadiusSqr)
            {
                isMoving = false;
            }
        }
    }

    void FixedUpdate()
    {
        // Shift out of the way of moving units
        if (!isMoving)
        {
            Vector3 shiftDir = Vector3.zero;

            // Implement a soft collision radius
            softCollisionTargets = Physics2D.OverlapCircleAll(transform.position, softCollisionRadius);
            foreach (Collider2D col in softCollisionTargets)
            {
                Vector3 colPos = col.transform.position;
                Vector3 colDir = (transform.position - colPos).normalized;
                shiftDir = (shiftDir + colDir).normalized;
            }

            transform.position += shiftDir * moveSpeed * Time.deltaTime;
        }
    }

    public bool IsMoving()
    {
        return isMoving;
    }

    public void SetMoveTarget(Vector3 moveTarget)
    {
        this.moveTarget = moveTarget;
        isMoving = true;
    }

    public void SetGatherRadiusSqr(float gatherRadiusSqr)
    {
        this.gatherRadiusSqr = gatherRadiusSqr;
    }
}
