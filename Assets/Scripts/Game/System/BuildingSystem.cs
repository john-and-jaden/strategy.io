﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    [SerializeField] private SpriteRenderer placementIndicatorPrefab;
    [SerializeField] private Color validPlacementColor;
    [SerializeField] private Color invalidPlacementColor;

    private Transform buildingParent;
    public Transform BuildingParent { get { return buildingParent; } }

    private BuildingType activeBuildingType;
    private SpriteRenderer placementIndicator;
    private bool isPlacementValid;

    void Awake()
    {
        buildingParent = new GameObject("Buildings").transform;
    }

    void Start()
    {
        placementIndicator = Instantiate(placementIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        placementIndicator.enabled = false;
    }

    void Update()
    {
        if (activeBuildingType == null) return;
        
        // Get mouse position
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Update placement indicator position
        CheckPlacement(mousePos);

        // On left-click, spawn active building type
        if (Input.GetButtonDown("Fire1") && isPlacementValid && !GameManager.SelectionSystem.IsOverUI)
        {
            SpawnBuilding(activeBuildingType, mousePos);
            CancelSelection();
        }

        // On right-click, cancel current selection
        if (Input.GetButtonDown("Fire2"))
        {
            CancelSelection();
        }
    }

    public void SelectBuildingType(BuildingType buildingType)
    {
        activeBuildingType = buildingType;
        placementIndicator.sprite = buildingType.PlacementIndicatorSprite;
        placementIndicator.enabled = true;
    }

    public void SpawnBuilding(BuildingType buildingType, Vector2 placementPos)
    {
        // Expend the resource costs
        GameManager.ResourceSystem.SpendWood(buildingType.WoodCost);
        GameManager.ResourceSystem.SpendStone(buildingType.StoneCost);

        // Spawn the building object
        Building building = Instantiate(buildingType.InteractablePrefab, placementPos, Quaternion.identity, buildingParent) as Building;
        building.BuildTime = buildingType.BuildTime;

        // Assign the selected worker(s)
        List<Worker> selectedWorkers = GameManager.SelectionSystem.GetSelectionOfType<Worker>();
        foreach (Worker worker in selectedWorkers)
        {
            worker.Build(building);
        }
    }

    public bool IsSelectionActive()
    {
        return activeBuildingType != null;
    }

    private void CheckPlacement(Vector2 placementPos)
    {
        // Perform a physics check at the current location
        isPlacementValid = Physics2D.OverlapBox(placementPos, activeBuildingType.SizeDimensions, 0) == null;

        // Update the placement indicator
        placementIndicator.transform.position = placementPos;
        placementIndicator.color = isPlacementValid ? validPlacementColor : invalidPlacementColor;
    }

    private void CancelSelection()
    {
        activeBuildingType = null;
        placementIndicator.enabled = false;
    }
}
