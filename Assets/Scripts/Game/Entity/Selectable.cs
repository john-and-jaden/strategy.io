using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Selectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer hoverIndicatorPrefab;
    [SerializeField] private SpriteRenderer selectIndicatorPrefab;
    [SerializeField] private SpriteRenderer healthBarPrefab;

    protected bool hovered;
    protected bool selected;
    private float durability;
    public float Durability { get { return durability; } }
    [SerializeField] protected float initialDurability = 10;
    [System.Serializable] public class DestroyedEvent : UnityEvent { }
    protected DestroyedEvent onDestroyed = new DestroyedEvent();
    protected bool damaged;
    protected SpriteRenderer hoverIndicator;
    protected SpriteRenderer selectIndicator;
    protected SpriteRenderer healthBar;

    protected void SpawnIndicators()
    {
        hoverIndicator = Instantiate(hoverIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        selectIndicator = Instantiate(selectIndicatorPrefab, GameManager.SelectionSystem.IndicatorParent);
        hoverIndicator.transform.position = transform.position;
        selectIndicator.transform.position = transform.position;
        healthBar = Instantiate(healthBarPrefab, GameManager.SelectionSystem.IndicatorParent);
        healthBar.transform.position = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
    }

    protected void UpdateIndicators()
    {
        hoverIndicator.enabled = hovered;
        selectIndicator.enabled = selected;
        healthBar.enabled = damaged;
    }

    protected void DestroyIndicators()
    {
        Destroy(hoverIndicator.gameObject);
        Destroy(selectIndicator.gameObject);
        Destroy(healthBar.gameObject);
    }

    protected void Start()
    {
        durability = initialDurability;
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

    public void TakeDamage(float damage)
    {
        damaged = true;
        durability -= damage;
        Vector3 healthBarScale = healthBar.transform.localScale;
        healthBar.size = new Vector2(durability / initialDurability, healthBar.size.y);
        if (durability <= 0)
        {
            DestroySelf();
        }
    }

    protected virtual void DestroySelf()
    {
        DestroyIndicators();
        onDestroyed.Invoke();
        Destroy(gameObject);
    }

    public void AddDestroyedListener(UnityAction listener)
    {
        onDestroyed.AddListener(listener);
    }

    public void RemoveDestroyedListener(UnityAction listener)
    {
        onDestroyed.RemoveListener(listener);
    }
}
