using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingMenu : MonoBehaviour
{
    [SerializeField] private BuildingSelector[] selectors;

    public void SetBuildingTypes(List<BuildingType> buildingTypes)
    {
        if (buildingTypes.Count > selectors.Length)
        {
            Debug.LogError($"Number of building types exceeded menu capacity of {selectors.Length} building selectors!");
            return;
        }

        // Hide all selectors
        for (int i = 0; i < selectors.Length; i++)
        {
            selectors[i].SetSelectorActive(false);
        }

        // Show and update however many selectors we need
        for (int i = 0; i < buildingTypes.Count; i++)
        {
            selectors[i].SetBuildingType(buildingTypes[i]);
            selectors[i].SetSelectorActive(true);
        }
    }
}
