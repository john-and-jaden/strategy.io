using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class Interactable : MonoBehaviour
{
    [System.Serializable] public class DestroyedEvent : UnityEvent { }

    [SerializeField] private SpriteRenderer hoverIndicatorPrefab;
    [SerializeField] private SpriteRenderer selectIndicatorPrefab;
    [SerializeField] private SpriteRenderer healthBarPrefab;
    [SerializeField] private float healthBarFadeTime = 2f;
    [SerializeField] private float initialDurability = 10;

    private float durability;
    public float Durability { get { return durability; } }

    protected bool hovered;
    protected bool selected;
    protected DestroyedEvent onDestroyed = new DestroyedEvent();
    protected SpriteRenderer hoverIndicator;
    protected SpriteRenderer selectIndicator;
    protected SpriteRenderer healthBar;

    private float healthBarFadeTimer = 0f;

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
        healthBar.enabled = durability != initialDurability;
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
        if ((healthBarFadeTimer += Time.deltaTime) >= healthBarFadeTime) durability = initialDurability;
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
        durability -= damage;
        healthBar.size = new Vector2(durability / initialDurability, healthBar.size.y);
        healthBarFadeTimer = 0f;

        if (durability <= 0) DestroySelf();
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
