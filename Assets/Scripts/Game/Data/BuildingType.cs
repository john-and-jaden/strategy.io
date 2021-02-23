using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BuildingType", menuName = "BuildingType", order = 51)]
public class BuildingType : InteractableType
{
    [SerializeField] private Sprite placementIndicatorSprite;
    public Sprite PlacementIndicatorSprite { get { return placementIndicatorSprite; } }

    [SerializeField] private Vector2 sizeDimensions;
    public Vector2 SizeDimensions { get { return sizeDimensions; } }

    public override void HandleSelect()
    {
        GameManager.BuildingSystem.SelectBuildingType(this);
    }
}
