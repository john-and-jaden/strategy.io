using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worker : Unit
{
    [SerializeField] private BuildingType[] buildingTypes;
    [SerializeField] private float buildSpeedMultiplier = 1f;
    [SerializeField] private float maxBuildDist = 2f;
    [SerializeField] private float gatherRate = 1f;
    [SerializeField] private float maxGatherDist = 2f;

    private Building assignedBuilding;
    private Cluster assignedCluster;
    private Resource assignedResource;
    private float buildRate;

    protected override void Update()
    {
        if (state == UnitState.GATHERING) UpdateGather();
        if (state == UnitState.BUILDING) UpdateBuild();

        base.Update();
    }

    public override void Interact(Vector3 targetPos)
    {
        List<Resource> hoveredResources = GameManager.SelectionSystem.GetHoverTargetsOfType<Resource>();
        List<Building> hoveredBuildings = GameManager.SelectionSystem.GetHoverTargetsOfType<Building>();

        if (hoveredBuildings.Count == 1)
        {
            Build(hoveredBuildings.Single());
        }
        else if (hoveredResources.Count > 0)
        {
            Gather(hoveredResources.First().Cluster);
        }
        else
        {
            StopBuilding();
            StopGathering();
            base.Interact(targetPos);
        }
    }

    public override void Select()
    {
        if (!interactive) return;

        HUD.BuildMenu.SetBuildList(buildingTypes);
        HUD.BuildMenu.Open();

        base.Select();
    }

    public override void Deselect()
    {
        if (!interactive) return;

        HUD.BuildMenu.Close();

        base.Deselect();
    }

    public void Build(Building building)
    {
        assignedBuilding = building;
        buildRate = building.MaxHealth / building.BuildTime;
        state = UnitState.BUILDING;
    }

    private void UpdateBuild()
    {
        if (assignedBuilding == null) return;

        float buildDistSqr = Utils.GetSqrDistance(this, assignedBuilding);
        if (buildDistSqr < maxBuildDist * maxBuildDist)
        {
            bool finished = assignedBuilding.GainHealth(buildRate * buildSpeedMultiplier * Time.deltaTime);
            if (finished)
            {
                StopBuilding();
            }
        }
        else
        {
            Vector3 targetPos = assignedBuilding.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    private void StopBuilding()
    {
        assignedBuilding = null;
        state = UnitState.IDLE;
    }

    private void Gather(Cluster cluster)
    {
        assignedCluster = cluster;
        AssignResource();
        state = UnitState.GATHERING;
    }

    private void UpdateGather()
    {
        if (assignedResource == null) return;

        float resourceDistSqr = Utils.GetSqrDistance(this, assignedResource);
        if (resourceDistSqr < maxGatherDist * maxGatherDist)
        {
            assignedResource.TakeDamage(gatherRate * Time.deltaTime);
        }
        else
        {
            Vector3 targetPos = assignedResource.transform.position;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        }
    }

    private void StopGathering()
    {
        assignedCluster = null;

        if (assignedResource == null) return;
        assignedResource.RemoveDeathListener(HandleResourceDestruction);
        assignedResource = null;
    }

    private void AssignResource()
    {
        assignedResource = GetNearestResource();
        assignedResource.AddDeathListener(HandleResourceDestruction);
    }

    public Resource GetNearestResource()
    {
        float shortestDistSqr = float.MaxValue;
        Resource nearest = null;
        foreach (Resource r in assignedCluster.Resources)
        {
            float rDistSqr = (r.transform.position - transform.position).sqrMagnitude;
            if (rDistSqr < shortestDistSqr)
            {
                shortestDistSqr = rDistSqr;
                nearest = r;
            }
        }
        return nearest;
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
