using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Worker : Unit
{
    [SerializeField] private BuildingType[] buildingTypes;
    [SerializeField] private float gatherRate = 1f;
    [SerializeField] private float maxGatherDist = 2f;

    private Cluster assignedCluster;
    private Resource assignedResource;

    protected override void Update()
    {
        if (state == UnitState.GATHERING) UpdateGather();
        
        base.Update();
    }

    public override void Interact(Vector3 targetPos)
    {
        if (GameManager.SelectionSystem.HasHoverTargetOfType<Resource>())
        {
            Gather(GameManager.SelectionSystem.GetHoverTargetsOfType<Resource>()[0].Cluster);
        }
        else
        {
            StopGathering();
            base.Interact(targetPos);
        }
    }

    public override void Select()
    {
        if (!interactive) return;

        HUD.BuildingMenu.SetBuildingTypes(buildingTypes);
        HUD.BuildingMenu.Open();

        base.Select();
    }

    public override void Deselect()
    {
        if (!interactive) return;
        
        HUD.BuildingMenu.Close();

        base.Deselect();
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
        assignedResource.RemoveDeathListener(HandleResourceDestruction);
        assignedResource = null;
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
        assignedResource.AddDeathListener(HandleResourceDestruction);
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
