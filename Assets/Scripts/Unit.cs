using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Selectable
{
    private Cluster assignedCluster;
    public Cluster AssignedCluster
    {
        get { return assignedCluster; }
        set { assignedCluster = value; }
    }
    private Cluster previousFrameCluster;
    private Resource assignedResource;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float softCollisionRadius = 0.5f;

    private Vector3 moveTarget;
    private float gatherRadiusSqr;
    private bool isMoving;

    private Collider2D[] softCollisionTargets;
    private float resourceGatherRadius = 1f;

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

    private void AssignResource()
    {
        // If assignedCluster was changed (improves performance) or resource is unassigned
        // Then assign to closest resource in cluster
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

    private void HandleResourceDeath()
    {
        if (assignedCluster != null)
        {
            AssignResource();
        }
    }

    private void GatherResources()
    {
        // Mine resource if close enough
        if (assignedResource != null && Vector3.Distance(assignedResource.transform.position, transform.position) < resourceGatherRadius)
        {
            assignedResource.TakeDamage(1);
        }
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
}
