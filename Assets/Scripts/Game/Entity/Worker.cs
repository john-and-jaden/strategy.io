using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Worker : Unit
{
    [SerializeField] private BuildingType[] buildingTypes;
    [SerializeField] private float buildRate = 1f;
    [SerializeField] private float maxBuildDist = 2f;
    [SerializeField] private float gatherRate = 1f;
    [SerializeField] private float maxGatherDist = 2f;

    private Building assignedBuilding;
    private Cluster assignedCluster;
    private Resource assignedResource;

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

    public void Build(Building building)
    {
        assignedBuilding = building;
        state = UnitState.BUILDING;
    }

    private void UpdateBuild()
    {
        if (assignedBuilding == null) return;

        Vector3 buildingPos = assignedBuilding.transform.position;
        float buildDistSqr = Vector3.SqrMagnitude(transform.position - buildingPos);
        if (buildDistSqr < maxBuildDist * maxBuildDist)
        {
            bool finished = assignedBuilding.GainHealth(buildRate * Time.deltaTime);
            if (finished)
            {
                StopBuilding();
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, buildingPos, moveSpeed * Time.deltaTime);
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
        assignedCluster = null;

        if (assignedResource == null) return;
        assignedResource.RemoveDeathListener(HandleResourceDestruction);
        assignedResource = null;
    }

    private void AssignResource()
    {
        assignedResource = Helper.GetNearestInList(assignedCluster.Resources, transform.position);
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
