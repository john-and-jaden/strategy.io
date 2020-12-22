using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSystem : MonoBehaviour
{
    public PlacementIndicator placementIndicatorPrefab;

    private Transform buildingParent;
    public Transform BuildingParent { get { return buildingParent; } }

    private BuildingType activeBuildingType;
    private PlacementIndicator placementIndicator;

    void Awake()
    {
        buildingParent = new GameObject("Buildings").transform;
    }

    void Start()
    {
        placementIndicator = Instantiate(placementIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
    }

    void Update()
    {
        if (activeBuildingType == null) return;
        
        // Get mouse position
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Update placement indicator position
        placementIndicator.transform.position = mousePos;

        // On left-click, spawn active building type
        if (Input.GetButtonDown("Fire1"))
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
        placementIndicator.SetSprite(buildingType.PlacementIndicatorSprite);
        placementIndicator.Show();
    }

    public void SpawnBuilding(BuildingType buildingType, Vector2 placementPos)
    {
        Instantiate(buildingType.BuildingPrefab, placementPos, Quaternion.identity, buildingParent);
    }

    private void CancelSelection()
    {
        activeBuildingType = null;
        placementIndicator.Hide();
    }
}
