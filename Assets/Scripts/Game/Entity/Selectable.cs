using UnityEngine;

public abstract class Selectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer hoverIndicatorPrefab;
    [SerializeField] private SpriteRenderer selectIndicatorPrefab;

    protected bool hovered;
    protected bool selected;
    protected SpriteRenderer hoverIndicator;
    protected SpriteRenderer selectIndicator;

    protected void SpawnIndicators()
    {
        hoverIndicator = Instantiate(hoverIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        selectIndicator = Instantiate(selectIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        hoverIndicator.transform.position = transform.position;
        selectIndicator.transform.position = transform.position;
    }

    protected void UpdateIndicators()
    {
        hoverIndicator.enabled = hovered;
        selectIndicator.enabled = selected;
    }

    protected void DestroyIndicators()
    {
        Destroy(hoverIndicator.gameObject);
        Destroy(selectIndicator.gameObject);
    }

    protected void Start()
    {
        SpawnIndicators();
    }

    protected void Update()
    {
        UpdateIndicators();
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
