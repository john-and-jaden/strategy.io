using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingSelector : HUDMenu
{
    [SerializeField] private Image selectorIcon;
    [SerializeField] private TextMeshProUGUI selectorText;

    private BuildingType buildingType;

    public void SetBuildingType(BuildingType buildingType)
    {
        this.buildingType = buildingType;
        selectorIcon.sprite = buildingType.PlacementIndicatorSprite;
        selectorText.text = buildingType.DisplayName;
    }

    public void HandleSelect()
    {
        GameManager.BuildingSystem.SelectBuildingType(buildingType);
    }
}
