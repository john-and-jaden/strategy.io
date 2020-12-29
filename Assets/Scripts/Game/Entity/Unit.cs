using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float softCollisionRadius = 0.5f;
    [SerializeField] private float gatherRate = 1f;
    [SerializeField] private float maxGatherDist = 2f;

    private UnitState state;
    public UnitState State { get { return state; } }

    private Vector3 targetPos;
    public Vector3 TargetPos
    {
        get { return targetPos; }
        set { targetPos = value; }
    }

    private Collider2D[] softCollisionTargets;
    private Cluster assignedCluster;
    private Resource assignedResource;

    new void Start()
    {
        base.Start();
        targetPos = transform.position;
    }

    void Update()
    {
        // Update indicators
        hoverIndicator.transform.position = transform.position;
        selectIndicator.transform.position = transform.position;

        // Do action based on state
        switch (state)
        {
            case UnitState.RELOCATING:
                UpdateRelocate();
                break;
            case UnitState.GATHERING:
                UpdateGather();
                break;
        }
    }

    void FixedUpdate()
    {
        // Shift out of the way of moving units
        if (state == UnitState.IDLE)
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

    public void Relocate(Vector3 targetPos)
    {
        StopGathering();
        this.targetPos = targetPos;
        state = UnitState.RELOCATING;
    }

    public void Gather(Cluster cluster)
    {
        assignedCluster = cluster;
        AssignResource();
        state = UnitState.GATHERING;
    }

    private void UpdateRelocate()
    {
        float targetDistSqr = Vector3.SqrMagnitude(transform.position - targetPos);
        if (targetDistSqr < GameManager.UnitSystem.GroupRadiusSqr)
        {
            state = UnitState.IDLE;
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    private void UpdateGather()
    {
        if (assignedResource == null) return;

        Vector3 resourcePos = assignedResource.transform.position;
        float resourceDistSqr = Vector3.SqrMagnitude(transform.position - resourcePos);
        if (resourceDistSqr < maxGatherDist * maxGatherDist)
        {
            assignedResource.TakeDamage(gatherRate * Time.deltaTime);
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, resourcePos, moveSpeed * Time.deltaTime);
        }
    }

    private void StopGathering()
    {
        if (assignedCluster == null) return;

        assignedResource.RemoveDestroyedListener(HandleResourceDeath);
        assignedResource = null;
        assignedCluster = null;
    }

    private void AssignResource()
    {
        float minDistance = float.MaxValue;
        Resource closestResource = null;
        foreach (Resource resource in assignedCluster.Resources)
        {
            float distanceToNode = Vector3.Distance(resource.transform.position, transform.position);
            if (minDistance > distanceToNode)
            {
                minDistance = distanceToNode;
                closestResource = resource;
            }
        }
        assignedResource = closestResource;
        assignedResource.AddDestroyedListener(HandleResourceDeath);
    }

    private void HandleResourceDeath()
    {
        if (assignedCluster != null && assignedCluster.Resources.Count > 0)
        {
            AssignResource();
        }
        else
        {
            StopGathering();
        }
    }
}
