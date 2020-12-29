using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Selectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer hoverIndicatorPrefab;
    [SerializeField] private SpriteRenderer selectIndicatorPrefab;

    protected SpriteRenderer hoverIndicator;
    protected SpriteRenderer selectIndicator;

    protected void Start()
    {
        SpawnIndicators();
    }

    private void SpawnIndicators()
    {
        hoverIndicator = Instantiate(hoverIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        selectIndicator = Instantiate(selectIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        hoverIndicator.transform.position = transform.position;
        selectIndicator.transform.position = transform.position;
        hoverIndicator.enabled = false;
        selectIndicator.enabled = false;
    }

    protected void OnDestroy()
    {
        if (hoverIndicator == null || selectIndicator == null) return;
        Destroy(hoverIndicator.gameObject);
        Destroy(selectIndicator.gameObject);
    }

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
