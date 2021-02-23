using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuildSelector : HUDMenu
{
    [SerializeField] private Image selectorIcon;
    [SerializeField] private TextMeshProUGUI selectorText;

    private InteractableType interactableType;

    public void SetInteractableType(InteractableType interactableType)
    {
        this.interactableType = interactableType;
        selectorIcon.sprite = interactableType.SelectorSprite;
        selectorText.text = interactableType.DisplayName;
        UpdateAvailability();
    }

    public void UpdateAvailability()
    {
        bool hasWood = GameManager.ResourceSystem.Wood >= interactableType.WoodCost;
        bool hasStone = GameManager.ResourceSystem.Stone >= interactableType.StoneCost;
        canvasGroup.interactable = hasWood && hasStone;
    }

    public void HandleSelect()
    {
        interactableType.HandleSelect();
    }
}
