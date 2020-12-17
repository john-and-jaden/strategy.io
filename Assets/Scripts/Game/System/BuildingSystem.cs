using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public SpriteRenderer placementIndicatorPrefab;

    private Transform buildingParent;
    public Transform BuildingParent { get { return buildingParent; } }

    private BuildingType activeBuildingType;
    private SpriteRenderer placementIndicator;

    void Awake()
    {
        buildingParent = new GameObject("Buildings").transform;
        placementIndicator = Instantiate(placementIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        placementIndicator.enabled = false;
    }

    void Update()
    {
        if (activeBuildingType == null) return;

        // Update placement indicator position
        placementIndicator.transform.position = transform.position;

        // On left-click, spawn active building type
        if (Input.GetButtonDown("Fire1"))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
        Instantiate(buildingType.BuildingPrefab, placementPos, Quaternion.identity, buildingParent);
    }

    private void CancelSelection()
    {
        activeBuildingType = null;
        placementIndicator.enabled = false;
    }
}
