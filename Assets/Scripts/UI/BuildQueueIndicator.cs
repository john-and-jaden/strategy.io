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
        SetProgress(0);
    }

    public void SetProgress(float progress)
    {
        // Set the width of the mask based on progress
        indicatorMask.rectTransform.anchorMax = new Vector2(progress, 0);
    }
}
