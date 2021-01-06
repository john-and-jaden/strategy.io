using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMenu : HUDMenu
{
    [SerializeField] private BuildingSelector[] selectors;

    public void SetBuildingTypes(BuildingType[] buildingTypes)
    {
        if (buildingTypes.Length > selectors.Length)
        {
            Debug.LogError($"Number of building types exceeded menu capacity of {selectors.Length} building selectors!");
            return;
        }

        // Hide all selectors
        for (int i = 0; i < selectors.Length; i++)
        {
            selectors[i].Close();
        }

        // Show and update however many selectors we need
        for (int i = 0; i < buildingTypes.Length; i++)
        {
            selectors[i].SetBuildingType(buildingTypes[i]);
            selectors[i].Open();
        }
    }
}
