using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit
{
    [SerializeField] private float gatherRate = 1f;
    [SerializeField] private float maxGatherDist = 2f;

    private Cluster assignedCluster;
    private Resource assignedResource;

    new protected void Update()
    {
        if (state == UnitState.GATHERING) UpdateGather();

        base.Update();
    }

    public override void Interact(Vector3 targetPos)
    {
        if (GameManager.SelectionSystem.HighlightedCluster != null)
        {
            Gather(GameManager.SelectionSystem.HighlightedCluster);
        }
        else
        {
            StopGathering();
            base.Interact(targetPos);
        }
    }

    private void Gather(Cluster cluster)
    {
        assignedCluster = cluster;
        AssignResource();
        state = UnitState.GATHERING;
    }

    private void StopGathering()
    {
        assignedCluster = null;

        if (assignedResource == null) return;
        assignedResource.RemoveDestroyedListener(HandleResourceDestruction);
        assignedResource = null;
        state = UnitState.IDLE;
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

    private void AssignResource()
    {
        assignedResource = Helpers.GetNearestInteractable(assignedCluster.Resources, transform.position);
        assignedResource.AddDestroyedListener(HandleResourceDestruction);
    }

    private void HandleResourceDestruction()
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
