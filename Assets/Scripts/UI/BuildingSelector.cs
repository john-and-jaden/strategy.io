using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildingSelector : MonoBehaviour
{
    [SerializeField] private BuildingType buildingType;
    [SerializeField] private Image selectorIcon;
    [SerializeField] private TextMeshProUGUI selectorText;

    void Start()
    {
        selectorIcon.sprite = buildingType.PlacementIndicatorSprite;
        selectorText.text = buildingType.DisplayName;
    }

    public void HandleSelect()
    {
        GameManager.BuildingSystem.SelectBuildingType(buildingType);
    }
}
