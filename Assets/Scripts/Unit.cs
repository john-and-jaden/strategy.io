using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float softCollisionRadius = 0.5f;
    [SerializeField] private float miningEfficiency = 1f;
    [SerializeField] private float resourceGatherRadius = 2f;

    private Vector3 moveTarget;
    private float gatherRadiusSqr;
    private bool isMoving;
    private Collider2D[] softCollisionTargets;
    private Cluster assignedCluster;
    private Resource assignedResource;

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
            if (dist < gatherRadiusSqr && assignedCluster == null)
            {
                isMoving = false;
            }
        }

        // Gather resources in assigned cluster
        if (assignedCluster != null)
        {
            GatherResources();
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

    public void AssignCluster(Cluster cluster)
    {
        assignedCluster = cluster;
        AssignResource();
    }

    public void UnassignCluster()
    {
        assignedCluster = null;
        assignedResource = null;
    }

    private void AssignResource()
    {
        float minDistance = float.MaxValue;
        foreach (Resource resource in assignedCluster.resources)
        {
            float distanceToNode = Vector3.Distance(resource.transform.position, transform.position);
            if (minDistance > distanceToNode)
            {
                minDistance = distanceToNode;
                assignedResource = resource;
            }
        }
        assignedResource.AddResourceDiedListened(HandleResourceDeath);
        SetMoveTarget(assignedResource.transform.position);
    }

    private void GatherResources()
    {
        // Mine resource if close enough
        if (assignedResource != null && Vector3.Distance(assignedResource.transform.position, transform.position) < resourceGatherRadius)
        {
            assignedResource.TakeDamage(miningEfficiency);
        }
    }

    private void HandleResourceDeath()
    {
        if (assignedCluster != null && assignedCluster.resources.Count > 0)
        {
            AssignResource();
        }
        else
        {
            UnassignCluster();
        }
    }
}
