using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Selectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer hoverIndicator;
    [SerializeField] private SpriteRenderer selectIndicator;

    public void Hover()
    {
        if (hoverIndicator == null) return;
        hoverIndicator.enabled = true;
    }

    public void CancelHover()
    {
        if (hoverIndicator == null) return;
        hoverIndicator.enabled = false;
    }

    public void Select()
    {
        if (selectIndicator == null) return;
        selectIndicator.enabled = true;
    }

    public void CancelSelect()
    {
        if (selectIndicator == null) return;
        selectIndicator.enabled = false;
    }
}
