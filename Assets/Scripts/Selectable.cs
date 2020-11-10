using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Selectable : MonoBehaviour
{
    public SpriteRenderer hoverIndicatorPrefab;
    public SpriteRenderer selectIndicatorPrefab;

    protected bool hovered;
    protected bool selected;
    protected SpriteRenderer hoverIndicator;
    protected SpriteRenderer selectIndicator;

    protected void SpawnIndicators()
    {
        hoverIndicator = Instantiate(hoverIndicatorPrefab);
        selectIndicator = Instantiate(selectIndicatorPrefab);
        hoverIndicator.transform.position = transform.position;
        selectIndicator.transform.position = transform.position;
    }

    protected void UpdateIndicators()
    {
        hoverIndicator.enabled = hovered;
        selectIndicator.enabled = selected;
    }

    public void SetHovered(bool hovered)
    {
        this.hovered = hovered;
    }

    public void SetSelected(bool selected)
    {
        this.selected = selected;
    }
}
