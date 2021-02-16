using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableType : ScriptableObject
{
    [SerializeField] private string displayName;
    public string DisplayName { get { return displayName; } }

    [SerializeField] private Interactable interactablePrefab;
    public Interactable InteractablePrefab { get { return interactablePrefab; } }

    [SerializeField] private Sprite selectorSprite;
    public Sprite SelectorSprite { get { return selectorSprite; } }

    [SerializeField] private int woodCost;
    public int WoodCost { get { return woodCost; } }

    [SerializeField] private int stoneCost;
    public int StoneCost { get { return stoneCost; } }

    [SerializeField] private float buildTime;
    public float BuildTime { get { return buildTime; } }

    public abstract void HandleSelect();
}
