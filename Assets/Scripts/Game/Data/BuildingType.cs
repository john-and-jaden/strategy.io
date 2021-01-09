using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BuildingType", menuName = "BuildingType", order = 51)]
public class BuildingType : ScriptableObject
{
    [SerializeField] private Building buildingPrefab;
    public Building BuildingPrefab { get { return buildingPrefab; } }

    [SerializeField] private Sprite placementIndicatorSprite;
    public Sprite PlacementIndicatorSprite { get { return placementIndicatorSprite; } }

    [SerializeField] private int woodCost;
    public int WoodCost { get { return woodCost; } }

    [SerializeField] private int stoneCost;
    public int StoneCost { get { return stoneCost; } }

    [SerializeField] private Vector2 sizeDimensions;
    public Vector2 SizeDimensions { get { return sizeDimensions; } }

    [SerializeField] private string displayName;
    public string DisplayName { get { return displayName; } }
}
