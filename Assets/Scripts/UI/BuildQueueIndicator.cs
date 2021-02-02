using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildQueueIndicator : HUDMenu
{
    [SerializeField] private Image indicatorIcon;
    [SerializeField] private Image indicatorMask;

    public void SetInteractableType(InteractableType interactableType)
    {
        indicatorIcon.sprite = interactableType.SelectorSprite;
        indicatorMask.sprite = interactableType.SelectorSprite;
        UpdateProgress(0);
    }

    public void UpdateProgress(float progress)
    {
        // Set the height of the mask based on progress
        indicatorMask.rectTransform.anchorMax = new Vector2(1, 1 - progress);
    }
}
